using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using ApplicationCore.Fractals;
using ApplicationCore.Helpers;
using ApplicationCore.Maths;
using ApplicationCore.Media;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;

namespace CoreTests
{
    public class ImageTests
    {
        [TestCase(100, 100, "#123456")]
        [TestCase(2000, 2000, "#001122")]
        public void CanCreateBlankImage(int width, int height, string color)
        {
            Console.WriteLine($"Vector.IsHardwareAccelerated: {Vector.IsHardwareAccelerated}");
            var image = ImageUtils.GenerateSolidImage(width, height, Rgba32.ParseHex(color));
            Assert.NotNull(image);
            Assert.NotNull(image.Contents);
        }

        [Test]
        public async Task CanCreateFullFractal()
        {
            var options = new FractalOptions
            {
                Expression = "z^3+1",
                PixelSize = new PixelSize(400, 400),
                DomainSize = new DomainSize(-5, 5, -5, 5),
                MaxIterations = 50,
                Precision = 1e-5,
                FillColor = new Hsv(0, 0, 0),
                Depth = 15,
                Gradient = 0.25f,
            };

            var generator = new LocalFractalGenerator(options);
            var result = await generator.GenerateAsync();

            var image = ImageUtils.GenerateImage(result.Contents);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Fractal.png");
            image.Save(path);
            TestContext.AddTestAttachment(path, "Fractal image generated");
        }
    }
}