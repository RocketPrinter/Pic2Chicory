using System;
using System.Collections.Generic;
using System.Threading;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Pic2Chicory
{


    public static class Color
    {
        // if -1 skip or use white
        // uses the "RedMean" algorithm
        public static int GetNearestColorIndexRedMean(Rgba32 color)
        {
            //if transparent ignore the point
            if (color.A < 127) return -1;

            int index = -1;
            double distance;
            //not all palattes have pure white but it's good enough
            double bestDistance = RedMeanColorDifference(color, new Rgba32(255, 255, 255, 255));

            for (int i = 0; i < Program.selectedPalette.colors.Length; i++)
            {
                distance = RedMeanColorDifference(color, Program.selectedPalette.colors[i]);
                if (bestDistance > distance)
                {
                    bestDistance = distance;
                    index = i;
                }
            }

            return index;
        }

        // if -1 skip or use white
        // uses the hue to compare colors
        public static int GetNearestColorIndexHue(Rgba32 color)
        {
            //if transparent ignore the point
            if (color.A < 127) return -1;
            int chue = GetHue(color);

            int index = -1;
            int distance;
            //not all palattes have pure white but it's good enough
            int bestDistance = Math.Abs(chue - GetHue(new Rgba32(255, 255, 255, 255)));

            for (int i = 0; i < Program.selectedPalette.colors.Length; i++)
            {
                distance = Math.Abs(chue - GetHue(Program.selectedPalette.colors[i]));
                if (bestDistance > distance)
                {
                    bestDistance = distance;
                    index = i;
                }
            }

            return index;
        }

        //https://en.wikipedia.org/wiki/Color_difference
        static double RedMeanColorDifference(Rgba32 a, Rgba32 b)
        {
            int redMean = (a.R + b.R) / 2;
            int redDelta = b.R - a.R;
            int greenDelta = b.G - a.G;
            int blueDelta = b.B - a.B;

            //dark magic
            return Math.Sqrt((2 + redMean / 256f) * redDelta * redDelta + 4 * greenDelta * greenDelta + (2 + (255 - redMean) / 256f) * blueDelta * blueDelta);
        }
        //https://www.rapidtables.com/convert/color/rgb-to-hsv.html
        static int GetHue(Rgba32 rgb)
        {
            int r = rgb.R, g = rgb.G, b = rgb.B;
            int cmax = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);
            int cmin = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
            int delta = cmax - cmin;

            if (delta == 0)
                return 0;

            //dark magic
            if (cmax == r)
                return (int)(60f * Utils.RealModulo((int)((g - b) / (255f * delta)), 6));
            if (cmax == g)
                return (int)(60f * ((b - r) / (255f * delta) + 2));

            return (int)(60f * ((r - g) / (255f * delta) + 4));
        }
    }
}
