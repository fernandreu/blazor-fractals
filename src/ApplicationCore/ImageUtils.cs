using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ApplicationCore
{
    public static class ImageUtils
    {
        public static async Task<Image> GenerateSolidImageAsync(int width, int height, Rgba32 color)
        {
            using var image = new Image<Rgba32>(width, height, color);
            await using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            return new Image("png", ms.ToArray());
        }
    }
}
