using System.Collections.Generic;
using SixLabors.ImageSharp.ColorSpaces;

namespace ApplicationCore.Helpers
{
    public class FractalResult
    {
        public Hsv[,] Contents { get; set; }
        
        public double MeanIterations { get; set; }
        
        public double StDevIterations { get; set; }
        
        public ICollection<HsvColorSpec> ColorSpecs { get; set; }
    }
}