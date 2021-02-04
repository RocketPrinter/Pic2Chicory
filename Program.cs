using System;
using System.Collections.Generic;
using System.Threading;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Pic2Chicory
{
    class Program
    {
        public class ColorPalette
        {
            public string name;
            public Rgb24[] colors { get; private set; }
            public ColorPalette(string _name, params string[] hexcodes)
            {
                name = _name;
                colors = new Rgb24[hexcodes.Length];
                for (int i=0;i<hexcodes.Length;i++)
                {
                    var drawingColor = System.Drawing.ColorTranslator.FromHtml(hexcodes[i]);
                    colors[i] = new Rgb24(drawingColor.R,drawingColor.G,drawingColor.B);
                }
            }
        }
        public struct Vector2Int
        {
            public int x, y;
            public Vector2Int(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
        }

        public static ColorPalette[] palettes = new ColorPalette[]
        {
            new ColorPalette("test", "#464646")
        };

        //seconds
        public const int startingDelay = 3;
        
        //miliseconds
        public const int posDownDelay = 25;
        public const int downUpDelay  = 25;
        public const int upPosDelay   = 25;

        //path to picture
        //rez x
        //rez y
        //color palette
        static void Main(string[] args)
        {
            #region argument proccesing
            if (args.Length==0||args.Length>4)
            {
                Console.WriteLine("Usage: <path> <rez x> <rez y> <color palette>");
                Environment.Exit(0);
            }

            string imagePath = args[0]; 

            int rezx = Convert.ToInt32(args[1]);
            int rezy = Convert.ToInt32(args[2]);

            ColorPalette palette = null;
            foreach (ColorPalette cp in palettes)
            {
                if (args[3]==cp.name)
                {
                    palette = cp;
                    break;
                }
            }
            if (palette == null)
            {
                Console.WriteLine("Available palettes:");
                foreach (ColorPalette cp in palettes)
                    Console.Write("\""+cp.name+"\" ");
                Environment.Exit(0);
            }
            #endregion

            //
            Vector2Int[] posarr = new Vector2Int[rezx*rezy];
            for (int i=0;i<rezx;i++)
            for (int j=0;j<rezy;j++)
                {
                    posarr[i * rezx + j] = new Vector2Int(i,j);
                }
            posarr.Shuffle();

            //image processing
            //using Image<Rgb24> image = (Image<Rgb24>)Image.Load(imagePath);
            //
            //image.Mutate((x) => x.Resize(rezx,rezy));

            //do the coloring
            Console.WriteLine("Starting in {0} seconds", startingDelay);
            Thread.Sleep(startingDelay*1000);
            Console.WriteLine("Coloring in!");

            float offsetx = 0.5f/rezx;
            float offsety = 0.5f/rezy;

            foreach (Vector2Int v2i in posarr)
            {
                CursorControl.SetCursorPos01(offsetx+(float)v2i.x / rezx,offsety+(float)v2i.y / rezy);
                Thread.Sleep(posDownDelay);
                CursorControl.sendMouseDown();
                Thread.Sleep(downUpDelay);
                CursorControl.sendMouseUp();
                Thread.Sleep(upPosDelay);
            }
            Console.WriteLine("Done!");
        }
    }
}
