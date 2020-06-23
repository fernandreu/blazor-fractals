using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ApplicationCore.Extensions;
using ApplicationCore.Helpers;
using ApplicationCore.Maths;
using SixLabors.ImageSharp.ColorSpaces;

namespace ApplicationCore.Fractals
{
    public abstract class FractalGenerator
    {
        protected FractalGenerator(FractalOptions options)
        {
            Options = options;
            var element = MathElement.Parse(Options.Expression, Options.VariableName);
            Function = element.ToNewtonFunction(Options.Multiplicity).ToFunc();
        }
        
        public Func<Complex, Complex> Function { get; }
        
        public FractalOptions Options { get; }

        public async Task<FractalResult> GenerateAsync()
        {
            var solutions = await GenerateNewtonArrayAsync();
            return await GenerateAsync(solutions);
        }

        protected virtual Task<NewtonResult[][]> GenerateNewtonArrayAsync()
        {
            var result = new NewtonResult[Options.PixelSize.Width][];
            for (var px = 0; px < Options.PixelSize.Width; ++px)
            {
                var x = Options.DomainSize.MinX + (Options.DomainSize.MaxX - Options.DomainSize.MinX) * px / (Options.PixelSize.Width - 1);
                result[px] = new NewtonResult[Options.PixelSize.Height];
                for (var py = 0; py < Options.PixelSize.Height; ++py)
                {
                    var y = Options.DomainSize.MaxY - (Options.DomainSize.MaxY - Options.DomainSize.MinY) * py / (Options.PixelSize.Height - 1);
                    var newtonOptions = new NewtonOptions
                    {
                        Precision = Options.Precision,
                        StartingPoint = new Complex(x, y),
                        MaxIterations = Options.MaxIterations,
                    };
                    result[px][py] = MathUtils.NewtonMethod(Function, newtonOptions);
                }
            }

            return Task.FromResult(result);
        }
        
        private Task<FractalResult> GenerateAsync(NewtonResult[][] solutions)
        {
            var result = new FractalResult
            {
                Contents = new Hsv[Options.PixelSize.Width, Options.PixelSize.Height],
            };
            
            var rnd = new Random();
            
            // Stored as a double to avoid castings later
            double totalPoints = Options.PixelSize.Width * Options.PixelSize.Height;

            result.ColorSpecs = new List<HsvColorSpec>(Options.ColorSpecs?.AsHsvSpecs() ?? Enumerable.Empty<HsvColorSpec>());

            var logT = MathF.Log((float) Options.Precision);

            for (var px = 0; px < Options.PixelSize.Width; ++px)
            {
                for (var py = 0; py < Options.PixelSize.Height; ++py)
                {
                    var solution = solutions[px][py];
                    result.MeanIterations += solution.Iterations / totalPoints;
                    result.StDevIterations += solution.Iterations * solution.Iterations / totalPoints;

                    if (solution.Status != SolutionStatus.Found)
                    {
                        result.Contents[px, py] = Options.FillColor;
                        continue;
                    }

                    var spec = FindSpec(result.ColorSpecs, solution.Solution, Options.Precision * 10);
                    if (spec == null)
                    {
                        spec = new HsvColorSpec
                        {
                            Root = solution.Solution,
                            Color = new Hsv(rnd.NextFloat() * 360f, 1f, Options.Depth < 0 ? 0.1f : 1f),
                        };
                        result.ColorSpecs.Add(spec);
                    }

                    if (Options.Depth == 0)
                    {
                        result.Contents[px, py] = spec.Color;
                        continue;
                    }

                    var logD0 = MathF.Log((float) Complex.Abs(solution.PreviousSolution - spec.Root));
                    var logD1 = MathF.Log((float) Complex.Abs(solution.Solution - spec.Root));
                    var value = spec.Color.V;
                    
                    if (solution.Iterations > Options.Threshold)
                    {
                        if (Options.Gradient == 0)
                        {
                            var factor = (solution.Iterations - 1 - Options.Threshold) * Options.Depth * 0.01f;
                            if (Options.Depth > 0)
                            {
                                value /= 1 + factor;
                            }
                            else
                            {
                                value += (1 - value) * (1 - 1 / (1 - factor));
                            }
                        }
                        else
                        {
                            var factor = (solution.Iterations - 1 - Options.Threshold) * Options.Depth * 0.01f;
                            var factorPlus = (solution.Iterations - Options.Threshold) * Options.Depth * 0.01f;
                            float lowValue;
                            float highValue;
                            if (Options.Depth > 0)
                            {
                                lowValue = value / (1 + factor);
                                highValue = value / (1 + factorPlus);
                            }
                            else
                            {
                                lowValue = value + (1 - value) * (1 - 1 / (1 - factor));
                                highValue = value + (1 - value) * (1 - 1 / (1 - factorPlus));
                            }

                            if (Math.Abs(logD1 - logD0) < 0.001)
                            {
                                value = highValue;
                            }
                            else
                            {
                                value = highValue + Options.Gradient * (highValue - lowValue) * (logT - logD0) / (logD1 - logD0);
                            }
                        }
                    }

                    value = MathF.Max(0.025F, MathF.Min(0.975F, value));
                    result.Contents[px, py] = new Hsv(spec.Color.H, spec.Color.S, value);
                }
            }

            result.StDevIterations = Math.Sqrt(result.StDevIterations - result.MeanIterations * result.MeanIterations);
            
            return Task.FromResult(result);
        }

        private static HsvColorSpec FindSpec(IEnumerable<HsvColorSpec> list, Complex root, double precision)
        {
            foreach (var (item, index) in list.Enumerated())
            {
                if (Complex.Abs(item.Root - root) < precision)
                {
                    return item;
                }
            }

            return null;
        }
    }
}