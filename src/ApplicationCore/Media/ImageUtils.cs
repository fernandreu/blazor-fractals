using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace ApplicationCore.Media
{
    public static class ImageUtils
    {
        public static WebImage GenerateSolidImage(int width, int height, Rgba32 color)
        {
            using var image = new Image<Rgba32>(width, height, color);
            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            return new WebImage("png", ms.ToArray());
        }

        public static Image<Rgba32> GenerateImage(Hsv[,] contents)
        {
            var converter = new ColorSpaceConverter();
            var image = new Image<Rgba32>(contents.GetLength(0), contents.GetLength(1));
            for (var i = 0; i < image.Width; ++i)
            {
                for (var j = 0; j < image.Height; ++j)
                {
                    image[i, j] = converter.ToRgb(contents[i, j]);
                }
            }

            return image;
        }

        public static WebImage ToWebImage(this Image<Rgba32> self)
        {
            using var ms = new MemoryStream();
            self.SaveAsPng(ms);
            return new WebImage("png", ms.ToArray());
        }
    }
}
