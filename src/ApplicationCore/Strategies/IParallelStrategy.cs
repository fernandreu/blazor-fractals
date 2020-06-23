using System;
using System.Numerics;
using System.Threading.Tasks;
using ApplicationCore.Helpers;

namespace ApplicationCore.Strategies
{
    public interface IParallelStrategy
    {
        Task<NewtonResult[][]> NewtonArrayAsync(Func<Complex, Complex> func, BasicFractalOptions options);
    }
}