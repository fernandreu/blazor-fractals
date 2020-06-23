using System;
using System.Numerics;
using System.Threading.Tasks;
using ApplicationCore.Helpers;

namespace ApplicationCore.Strategies
{
    public class SingleCoreStrategy : IParallelStrategy
    {
        public Task<NewtonResult[][]> NewtonArrayAsync(Func<Complex, Complex> func, BasicFractalOptions options)
        {
            var result = new NewtonResult[][options.PixelSize.Width];
            for (var px = 0; px < options.PixelSize.Width; ++px)
            {
                var x = options.DomainSize.MinX + (options.DomainSize.MaxX - options.DomainSize.MinX) * px / (options.PixelSize.Width - 1);
                result[px] = new NewtonResult[options.PixelSize.Height];
                for (var py = 0; py < options.PixelSize.Height; ++py)
                {
                    var y = options.DomainSize.MaxY - (options.DomainSize.MaxY - options.DomainSize.MinY) * py / (options.PixelSize.Height - 1);
                    var newtonOptions = new NewtonOptions
                    {
                        Precision = options.Precision,
                        StartingPoint = new Complex(x, y),
                        MaxIterations = options.MaxIterations,
                    };
                    result[px][py] = MathUtils.NewtonMethod(func, newtonOptions);
                }
            }

            return Task.FromResult(result);
        }
    }
}