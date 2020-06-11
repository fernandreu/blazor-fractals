using System.Numerics;

namespace ApplicationCore.Helpers
{
    public class NewtonOptions
    {
        public Complex StartingPoint { get; set; }
        
        public int MaxIterations { get; set; }
        
        public double Precision { get; set; }
    }
}