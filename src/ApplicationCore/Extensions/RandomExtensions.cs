using System;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace ApplicationCore.Extensions
{
    public static class RandomExtensions
    {
        public static float NextFloat(this Random self)
            => (float) self.NextDouble();

        public static byte NextByte(this Random self)
            => (byte) self.Next(256);
        
        public static Rgba32 NextRgba32(this Random self, bool alpha = false)
            => new Rgba32(
                self.NextByte(), 
                self.NextByte(), 
                self.NextByte(),
                alpha ? self.NextByte() : 255);

        public static Hsv NextHsv(this Random self)
            => new Hsv(self.NextFloat() * 360F, self.NextFloat(), self.NextFloat());
    }
}