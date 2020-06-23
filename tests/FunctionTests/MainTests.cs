using System.Threading.Tasks;
using ApplicationCore.Helpers;
using Function;
using FunctionTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace FunctionTests
{
    public class MainTests
    {
        private readonly ILogger _logger = TestFactory.CreateLogger();
        
        [Test]
        public async Task CanGenerateFractalAsync()
        {
            var options = new FractalOptions
            {
                Expression = "z^3+1",
            };

            var result = await CallFunction(options);
            Assert.Null(result.ErrorMessage);
        }

        [TestCase("z+")]
        [TestCase("(z")]
        [TestCase("z)")]
        [TestCase("sin()")]
        public async Task CanDetectParseErrors(string expression)
        {
            var options = new FractalOptions
            {
                Expression = expression,
            };

            var result = await CallFunction(options);
            Assert.NotNull(result.ErrorMessage);
        }
        
        private async Task<FunctionResult> CallFunction(FractalOptions options)
        {
            var request = TestFactory.CreateHttpRequest(options);
            var result = (OkObjectResult) await EntryPoint.RunLocally(request, _logger);
            return (FunctionResult) result.Value;;
        }
    }
}