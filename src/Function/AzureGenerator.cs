using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Fractals;
using ApplicationCore.Helpers;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Function
{
    public class AzureGenerator : FractalGenerator
    {
        public AzureGenerator(FractalOptions options, IDurableOrchestrationContext context, int chunks = 8) : base(options)
        {
            _context = context;
            _chunks = chunks;
        }

        private readonly IDurableOrchestrationContext _context;

        private readonly int _chunks;
        
        protected override async Task<NewtonResult[][]> GenerateNewtonArrayAsync()
        {
            var chunkSize = Options.PixelSize.Width / _chunks;
            var collection = new List<ChunkOptions>();
            var width = 0;
            for (var i = 0; i < _chunks; ++i)
            {
                var nextWidth = i == _chunks - 1 ? Options.PixelSize.Width : width + chunkSize;
                collection.Add(new ChunkOptions
                {
                    FractalOptions = Options,
                    MinWidth = width,
                    MaxWidth = nextWidth,
                });
                width = nextWidth;
            }

            var tasks = collection.Select(x => _context.CallActivityAsync<NewtonResult[][]>(nameof(EntryPoint.ExecuteChunk), x));
            var rawResult = await Task.WhenAll(tasks);

            var result = new NewtonResult[Options.PixelSize.Width][];
            foreach (var (item, options) in rawResult.Zip(collection, (x, y) => (x, y)))
            {
                for (var px = options.MinWidth; px < options.MaxWidth; ++px)
                {
                    result[px] = item[px - options.MinWidth];
                }
            }

            return result;
        }
    }
}