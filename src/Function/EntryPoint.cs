using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ApplicationCore.Extensions;
using ApplicationCore.Fractals;
using ApplicationCore.Helpers;
using ApplicationCore.Maths;
using ApplicationCore.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Function
{
    public static class EntryPoint
    {
        [FunctionName(nameof(RunLocally))]
        public static async Task<IActionResult> RunLocally(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            log.LogInformation($"Processor count: {Environment.ProcessorCount}");

            var content = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                var options = JsonConvert.DeserializeObject<FractalOptions>(content);

                var generator = new LocalFractalGenerator(options);
                var fractal = await generator.GenerateAsync();
                var image = ImageUtils.GenerateImage(fractal.Contents);
                var webImage = image.ToWebImage();
                var response = new FunctionResult
                {
                    ImageSource = webImage.Source,
                    ColorSpecs = fractal.ColorSpecs.AsHexSpecs().ToList(),
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                var response = new FunctionResult
                {
                    ErrorMessage = $"{ex.GetType().Name}: {ex.Message}",
                };
                
                return new OkObjectResult(response);
            }
        }

        [FunctionName(nameof(RunOrchestrated))]
        public static async Task<IActionResult> RunOrchestrated(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            
            try
            {
                var options = JsonConvert.DeserializeObject<FractalOptions>(content);
                var instanceId = await starter.StartNewAsync(nameof(RunOrchestrator), null, options);
                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
                return await starter.WaitForCompletionOrCreateCheckStatusResponseAsync(req, instanceId, TimeSpan.FromSeconds(60));
            }
            catch (Exception ex)
            {
                var response = new FunctionResult
                {
                    ErrorMessage = $"{ex.GetType().Name}: {ex.Message}",
                };
                
                return new OkObjectResult(response);
            }
        }
        
        [FunctionName(nameof(RunOrchestrator))]
        public static async Task<FunctionResult> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var options = context.GetInput<FractalOptions>();
            var generator = new AzureGenerator(options, context);
            var fractal = await generator.GenerateAsync();
            var image = ImageUtils.GenerateImage(fractal.Contents);
            var webImage = image.ToWebImage();
            var response = new FunctionResult
            {
                ImageSource = webImage.Source,
                ColorSpecs = fractal.ColorSpecs.AsHexSpecs().ToList(),
            };
            return response;
        }

        [FunctionName(nameof(ExecuteChunk))]
        public static NewtonResult[][] ExecuteChunk([ActivityTrigger] ChunkOptions options, ILogger log)
        {
            var fractalOptions = options.FractalOptions;
            var element = MathElement.Parse(fractalOptions.Expression, fractalOptions.VariableName);
            var func = element.ToNewtonFunction(fractalOptions.Multiplicity).ToFunc();
            
            var result = new NewtonResult[options.MaxWidth - options.MinWidth][];
            for (var px = options.MinWidth; px < options.MaxWidth; ++px)
            {
                var x = fractalOptions.DomainSize.MinX + (fractalOptions.DomainSize.MaxX - fractalOptions.DomainSize.MinX) * px / (fractalOptions.PixelSize.Width - 1);
                result[px - options.MinWidth] = new NewtonResult[fractalOptions.PixelSize.Height];
                for (var py = 0; py < fractalOptions.PixelSize.Height; ++py)
                {
                    var y = fractalOptions.DomainSize.MaxY - (fractalOptions.DomainSize.MaxY - fractalOptions.DomainSize.MinY) * py / (fractalOptions.PixelSize.Height - 1);
                    var newtonOptions = new NewtonOptions
                    {
                        Precision = fractalOptions.Precision,
                        StartingPoint = new Complex(x, y),
                        MaxIterations = fractalOptions.MaxIterations,
                    };
                    result[px - options.MinWidth][py] = MathUtils.NewtonMethod(func, newtonOptions);
                }
            }

            return result;
        }
    }
}
