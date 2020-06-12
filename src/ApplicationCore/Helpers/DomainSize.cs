namespace ApplicationCore.Helpers
{
    public class DomainSize
    {
        public DomainSize(double minX, double maxX, double minY, double maxY)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }
        
        public double MinX { get; }
        
        public double MaxX { get; }
        
        public double MinY { get; }
        
        public double MaxY { get; }
    }
}