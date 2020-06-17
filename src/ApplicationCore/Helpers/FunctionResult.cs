using System.Collections.Generic;

namespace ApplicationCore.Helpers
{
    public class FunctionResult
    {
        public string ErrorMessage { get; set; }
        
        public string ImageSource { get; set; }
        
        public ICollection<HexColorSpec> ColorSpecs { get; set; }
    }
}