using System.Collections.Generic;
using SixLabors.ImageSharp.ColorSpaces;

namespace ApplicationCore.Helpers
{
    public class BasicFractalOptions
    {
        public PixelSize PixelSize { get; set; } = new PixelSize();
        
        public DomainSize DomainSize { get; set; } = new DomainSize();

        public int MaxIterations { get; set; } = 50;

        public double Precision { get; set; } = 1e-5;
        
        public float Depth { get; set; }
        
        public float Gradient { get; set; }
        
        public int Threshold { get; set; }
        
        public Hsv FillColor { get; set; } = new Hsv(0, 0, 0);
        
        public ICollection<HexColorSpec> ColorSpecs { get; set; }
    }
}