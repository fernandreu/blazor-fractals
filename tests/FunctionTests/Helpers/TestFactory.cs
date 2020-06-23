using System.Collections.Generic;
using System.IO;
using System.Text;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace FunctionTests.Helpers
{
    public class TestFactory
    {
        private static Dictionary<string, StringValues> CreateDictionary(string key, string value)
        {
            var qs = new Dictionary<string, StringValues>
            {
                { key, value }
            };
            return qs;
        }

        public static DefaultHttpRequest CreateHttpRequest(FractalOptions options)
        {
            var serialized = JsonConvert.SerializeObject(options);
            var bytes = Encoding.UTF8.GetBytes(serialized);
            var request = new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = new MemoryStream(bytes),
            };
            return request;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}