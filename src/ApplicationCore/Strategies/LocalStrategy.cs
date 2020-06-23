using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Helpers;

namespace ApplicationCore.Strategies
{
    public class LocalStrategy : IParallelStrategy
    {
        public int ProcessCount { get; set; } = Environment.ProcessorCount;
        
        public Task<NewtonResult[][]> NewtonArrayAsync(Func<Complex, Complex> func, BasicFractalOptions options)
        {
            var semaphore = new SemaphoreSlim(ProcessCount, ProcessCount);
            var tasks = Enumerable
                .Range(0, options.PixelSize.Width)
                .Select(px => NewtonBandAsync(func, options, px, semaphore));

            return Task.WhenAll(tasks);
        }
        
        private static async Task<NewtonResult[]> NewtonBandAsync(
            Func<Complex, Complex> func,
            BasicFractalOptions options, 
            int px,
            SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();

            try
            {
                return await Task.Run(() => NewtonBandInternal(func, options, px));
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static NewtonResult[] NewtonBandInternal(Func<Complex, Complex> func, BasicFractalOptions options, int px)
        {
            var x = options.DomainSize.MinX + (options.DomainSize.MaxX - options.DomainSize.MinX) * px / (options.PixelSize.Width - 1);
            var result = new NewtonResult[options.PixelSize.Height];
            for (var py = 0; py < options.PixelSize.Height; ++py)
            {
                var y = options.DomainSize.MaxY - (options.DomainSize.MaxY - options.DomainSize.MinY) * py / (options.PixelSize.Height - 1);
                var newtonOptions = new NewtonOptions
                {
                    Precision = options.Precision,
                    StartingPoint = new Complex(x, y),
                    MaxIterations = options.MaxIterations,
                };
                result[py] = MathUtils.NewtonMethod(func, newtonOptions);
            }

            return result;
        }
    }
}