using ApplicationCore.Helpers;

namespace Function
{
    public class ChunkOptions
    {
        public FractalOptions FractalOptions { get; set; }
        
        public int MinWidth { get; set; }
        
        public int MaxWidth { get; set; }
    }
}