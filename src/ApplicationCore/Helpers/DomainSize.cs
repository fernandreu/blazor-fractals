namespace ApplicationCore.Helpers
{
    public class DomainSize
    {
        public DomainSize()
        {
        }
        
        public DomainSize(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public double MinX { get; set; } = -1;

        public double MaxX { get; set; } = 1;

        public double MinY { get; set; } = -1;

        public double MaxY { get; set; } = 1;
    }
}