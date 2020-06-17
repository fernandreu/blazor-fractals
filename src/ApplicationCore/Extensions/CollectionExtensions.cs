using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.PixelFormats;

namespace ApplicationCore.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<(T item, int index)> Enumerated<T>(this IEnumerable<T> self)
        {
            return self.Select((x, i) => (x, i));
        }

        public static IEnumerable<HsvColorSpec> AsHsvSpecs(this IEnumerable<HexColorSpec> self)
        {
            var converter = new ColorSpaceConverter();
            return self.Select(x => new HsvColorSpec
            {
                Root = x.Root,
                Color = converter.ToHsv(Rgba32.ParseHex(x.Color)),
            });
        }
        
        public static IEnumerable<HexColorSpec> AsHexSpecs(this IEnumerable<HsvColorSpec> self)
        {
            var converter = new ColorSpaceConverter();
            return self.Select(x =>
            {
                var rgb = converter.ToRgb(x.Color);
                return new HexColorSpec
                {
                    Root = x.Root,
                    Color = $"#{rgb.R:X2}{rgb.G:X2}{rgb.B:X2}",
                };
            });
        }
    }
}