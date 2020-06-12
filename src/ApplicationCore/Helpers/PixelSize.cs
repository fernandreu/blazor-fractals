namespace ApplicationCore.Helpers
{
    public class PixelSize
    {
        public PixelSize()
        {
        }
        
        public PixelSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Width { get; set; } = 100;

        public int Height { get; set; } = 100;
    }
}