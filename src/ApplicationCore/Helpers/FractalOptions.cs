using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace ApplicationCore.Helpers
{
    public class FractalOptions
    {
        public PixelSize PixelSize { get; set; }
        
        public DomainSize DomainSize { get; set; }
        
        public int MaxIterations { get; set; }
        
        public double Precision { get; set; }
        
        public float Depth { get; set; }
        
        public float Gradient { get; set; }
        
        public int Threshold { get; set; }
        
        public Hsv FillColor { get; set; }
    }
}