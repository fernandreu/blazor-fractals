using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Helpers;

namespace ApplicationCore.Fractals
{
    public class LocalFractalGenerator : FractalGenerator
    {
        public LocalFractalGenerator(FractalOptions options, int? processCount = null) : base(options)
        {
            ProcessCount = processCount ?? Environment.ProcessorCount;
        }

        public int ProcessCount { get; }
        
        protected override Task<NewtonResult[][]> GenerateNewtonArrayAsync()
        {
            var semaphore = new SemaphoreSlim(ProcessCount, ProcessCount);
            var tasks = Enumerable
                .Range(0, Options.PixelSize.Width)
                .Select(px => NewtonBandAsync(px, semaphore));

            return Task.WhenAll(tasks);
        }
        
        private async Task<NewtonResult[]> NewtonBandAsync(int px, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();

            try
            {
                return await Task.Run(() => NewtonBandInternal(px));
            }
            finally
            {
                semaphore.Release();
            }
        }
        
        private NewtonResult[] NewtonBandInternal(int px)
        {
            var x = Options.DomainSize.MinX + (Options.DomainSize.MaxX - Options.DomainSize.MinX) * px / (Options.PixelSize.Width - 1);
            var result = new NewtonResult[Options.PixelSize.Height];
            for (var py = 0; py < Options.PixelSize.Height; ++py)
            {
                var y = Options.DomainSize.MaxY - (Options.DomainSize.MaxY - Options.DomainSize.MinY) * py / (Options.PixelSize.Height - 1);
                var newtonOptions = new NewtonOptions
                {
                    Precision = Options.Precision,
                    StartingPoint = new Complex(x, y),
                    MaxIterations = Options.MaxIterations,
                };
                result[py] = MathUtils.NewtonMethod(Function, newtonOptions);
            }

            return result;
        }
    }
}