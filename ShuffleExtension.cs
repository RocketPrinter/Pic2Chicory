﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pic2Chicory
{
    public static class ShuffleExtension
    {
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
