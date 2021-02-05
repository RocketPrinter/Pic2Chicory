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
        private struct Dot
        {
            public int x, y;
            public Dot(int x,int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        //miliseconds
        public const int posDownDelay = 25;
        public const int downUpDelay = 25;
        public const int upPosDelay = 25;

        public const float doubleclickPreventionDistance=2f;
        public const int doubleclickPreventionDelay = 200;
        public void Print(Image<Rgba32> image, int rezx, int rezy)
        {
            //sort the dots by index
            List<Dot>[] dotsByIndex = new List<Dot>[Program.selectedPalette.colors.Length];
            for (int i = 0; i < dotsByIndex.Length; i++) dotsByIndex[i] = new List<Dot>();

            for (int i = 0; i < rezx; i++)
                for (int j = 0; j < rezy; j++)
                {
                    int cindex = Program.GetNearestColorIndex(image[i, j]);
                    if (cindex == -1) continue;
                    dotsByIndex[cindex].Add(new Dot(i,j));
                }
            
            //shuffle because it looks cooler
            for (int i = 0; i < dotsByIndex.Length; i++) dotsByIndex[i].Shuffle();

            float offsetx = 0.5f / rezx;
            float offsety = 0.5f / rezy;

            //paint
            Dot lastDot = new Dot(-999999,-999999);
            for (int i = 0; i < dotsByIndex.Length; i++)
            {
                Program.SelectColor(i);//select the right color

                foreach (Dot dot in dotsByIndex[i])
                {
                    double distance = Math.Sqrt((dot.x-lastDot.x)* (dot.x - lastDot.x)+(dot.y-lastDot.y)* (dot.y - lastDot.y));
                    if (distance <= doubleclickPreventionDistance)
                        Thread.Sleep(doubleclickPreventionDelay);
                    lastDot = dot;
                    CursorControl.SetCursorPos01(offsetx + (double)dot.x / rezx, offsety + (double)dot.y / rezy);
                    Thread.Sleep(posDownDelay);
                    CursorControl.sendMouseDown();
                    Thread.Sleep(downUpDelay);
                    CursorControl.sendMouseUp();
                    Thread.Sleep(upPosDelay);
                }
            }
        }
    }
}
