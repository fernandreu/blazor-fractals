using System;
using System.Threading.Tasks;
using ApplicationCore;
using NUnit.Framework;
using SixLabors.ImageSharp.PixelFormats;

namespace CoreTests
{
    public class ImageTests
    {
        [TestCase(100, 100, "#123456")]
        [TestCase(2000, 2000, "#001122")]
        public async Task CanCreateBlankImage(int width, int height, string color)
        {
            Console.WriteLine($"Vector.IsHardwareAccelerated: {System.Numerics.Vector.IsHardwareAccelerated}");
            var image = await ImageUtils.GenerateSolidImageAsync(width, height, Rgba32.ParseHex(color));
            Assert.NotNull(image);
            Assert.NotNull(image.Contents);
        }
    }
}