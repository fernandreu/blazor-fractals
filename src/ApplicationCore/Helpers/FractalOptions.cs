﻿using System.Collections.Generic;
 using System.Numerics;
 using SixLabors.ImageSharp.ColorSpaces;

 namespace ApplicationCore.Helpers
{
    public class FractalOptions
    {
        public string Expression { get; set; }

        public string VariableName { get; set; } = "z";
        
        public Complex Multiplicity { get; set; } = Complex.One;
        
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