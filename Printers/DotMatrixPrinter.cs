using System;
using System.Collections.Generic;
using System.Threading;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Pic2Chicory.Printers
{
    class DotMatrixPrinter : IPrinter
    {
        //miliseconds
        public const int posDownDelay = 25;
        public const int downUpDelay = 25;
        public const int upPosDelay = 25;

        public void Print(Image<Rgba32> image, int rezx, int rezy)
        {
            Vector2Int[] posarr = new Vector2Int[rezx * rezy];
            for (int i = 0; i < rezx; i++)
                for (int j = 0; j < rezy; j++)
                {
                    posarr[i * rezy + j] = new Vector2Int(i, j);
                }
            //posarr.Shuffle();

            float offsetx = 0.5f / rezx;
            float offsety = 0.5f / rezy;

            foreach (Vector2Int v2i in posarr)
            {
                //make sure we have the right color selected or skip this one
                if (Program.SelectColor(image[v2i.x,v2i.y])== false) continue;

                CursorControl.SetCursorPos01(offsetx + (double)v2i.x / rezx, offsety + (double)v2i.y / rezy);
                Thread.Sleep(posDownDelay);
                CursorControl.sendMouseDown();
                Thread.Sleep(downUpDelay);
                CursorControl.sendMouseUp();
                Thread.Sleep(upPosDelay);
            }
        }
    }
}
