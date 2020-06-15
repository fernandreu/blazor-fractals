using System.IO;
using System.Threading.Tasks;
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
            var options = JsonConvert.DeserializeObject<ExtendedFractalOptions>(content);

            var fractal = MathUtils.Fractal(options);
            var image = ImageUtils.GenerateImage(fractal.Contents);
            var webImage = image.ToWebImage();
            var response = webImage.Source;
            
            return new OkObjectResult(response);
        }
    }
}
