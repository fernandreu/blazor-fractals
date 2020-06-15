using System.Numerics;

namespace ApplicationCore.Helpers
{
    public class ExtendedFractalOptions : BasicFractalOptions
    {
        public string Expression { get; set; }

        public string VariableName { get; set; } = "z";
        
        public Complex Multiplicity { get; set; } = Complex.One;
    }
}