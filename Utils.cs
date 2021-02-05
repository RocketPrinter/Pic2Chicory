using System;
using System.Collections.Generic;
using System.Threading;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Pic2Chicory
{
    public static class Utils
    {
        //https://en.wikipedia.org/wiki/Color_difference
        public static double RedMeanColorDifference(Rgba32 a, Rgba32 b)
        {
            int redMean = (a.R + b.R)/2;
            int redDelta = b.R - a.R;
            int greenDelta = b.G - a.G;
            int blueDelta = b.B - a.B;

            return Math.Sqrt((2+ redMean/256f)*redDelta*redDelta+4*greenDelta*greenDelta+(2+(255-redMean)/256f )*blueDelta*blueDelta);
        }

        //great guide:
        //https://bost.ocks.org/mike/shuffle/
        public static void Shuffle<T>(this T[] list, int? seed=null)
        {
            Random rand;
            if (seed == null)
                rand = new Random();
            else
                rand = new Random(seed.Value);

            for (int i=list.Length-1;i>=0;i--)
            {
                int j = rand.Next(0,i+1);
                //swap i and j
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        //great guide:
        //https://bost.ocks.org/mike/shuffle/
        public static void Shuffle<T>(this List<T> list, int? seed=null)
        {
            Random rand;
            if (seed == null)
                rand = new Random();
            else
                rand = new Random(seed.Value);

            for (int i=list.Count-1;i>=0;i--)
            {
                int j = rand.Next(0,i+1);
                //swap i and j
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
