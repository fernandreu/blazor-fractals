namespace ApplicationCore.Helpers
{
    public class PixelSize
    {
        public PixelSize(int width, int height)
        {
            Width = width;
            Height = height;
        }
        
        public int Width { get; }
        
        public int Height { get; }
    }
}