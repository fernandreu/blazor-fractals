using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
                Root = new Complex(x.Real, x.Imaginary),
                Color = converter.ToHsv(Rgba32.ParseHex(x.Color)),
            });
        }
        
        public static IEnumerable<HexColorSpec> AsHexSpecs(this IEnumerable<HsvColorSpec> self)
        {
            var converter = new ColorSpaceConverter();
            return self.Select(x =>
            {
                var rgb = converter.ToRgb(x.Color);
                var r = (byte) (rgb.R * 255);
                var g = (byte) (rgb.G * 255);
                var b = (byte) (rgb.B * 255);
                return new HexColorSpec
                {
                    Real = x.Root.Real,
                    Imaginary = x.Root.Imaginary,
                    Color = $"#{r:x2}{g:x2}{b:x2}",
                };
            });
        }
    }
}