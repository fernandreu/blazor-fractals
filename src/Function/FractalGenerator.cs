using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Extensions;
using ApplicationCore.Helpers;
using ApplicationCore.Maths;
using ApplicationCore.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Function
{
    public static class FractalGenerator
    {
        [FunctionName(nameof(FractalGenerator))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var content = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                var options = JsonConvert.DeserializeObject<ExtendedFractalOptions>(content);

                var fractal = MathUtils.Fractal(options);
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
    }
}
