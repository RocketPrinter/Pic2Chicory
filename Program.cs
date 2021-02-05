﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Numerics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Pic2Chicory
{
    public class ColorPalette
    {
        public string name;
        public string firstColorHex;
        public Rgba32[] colors { get; private set; }
        public ColorPalette(string _name, params string[] hexcodes)
        {
            name = _name;
            firstColorHex = hexcodes[0];
            colors = new Rgba32[hexcodes.Length];
            for (int i = 0; i < hexcodes.Length; i++)
            {
                var drawingColor = System.Drawing.ColorTranslator.FromHtml(hexcodes[i]);
                colors[i] = new Rgba32(drawingColor.R, drawingColor.G, drawingColor.B,255);
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
    
    public static class Program
    {
        #region constants
        public static ColorPalette[] palettes = new ColorPalette[]
        {
            new ColorPalette("Main Menu", "#FFA192","#B696ED","#00EAD0","#D8F55D")
        };

        public const int startingDelay = 3;//seconds
        public const int sendKeyDelay = 1000;//miliseconds

        public const bool disableColors = false;//for testing
        #endregion

        public static ColorPalette selectedPalette;

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

            foreach (ColorPalette cp in palettes)
            {
                if (args[3]==cp.name)
                {
                    selectedPalette = cp;
                    break;
                }
            }
            if (selectedPalette == null)
            {
                Console.WriteLine("Available palettes:");
                foreach (ColorPalette cp in palettes)
                    Console.Write("\""+cp.name+"\" ");
                Environment.Exit(0);
            }
            #endregion

            //image processing
            using Image<Rgba32> image = (Image<Rgba32>)Image.Load(imagePath);
            
            image.Mutate((x) => x.Resize(rezx,rezy));

            //do the coloring
            Console.WriteLine("Starting in {0} seconds", startingDelay);
            Console.WriteLine("Make sure you have {0} selected!",selectedPalette.firstColorHex);
            Thread.Sleep(startingDelay*1000);
            Console.WriteLine("Coloring in!");

            IPrinter printer = new Printers.DotMatrixPrinter();
            printer.Print(image,rezx,rezy);

            Console.WriteLine("Done!");
        }


        public static int selectedColorIndex=0;
        //if returns false skip color or use right mouse
        public static bool SelectColor(Rgba32 color)
        {
            //for testing
#pragma warning disable CS0162 // Unreachable code detected
            if (disableColors) return true;
#pragma warning restore CS0162 // Unreachable code detected

            int newIndex=-1;
            //choose bestColor
            {
                double distance;
                //not all palattes have pure white but it's good enough
                double bestDistance = Utils.RedMeanColorDifference(color, new Rgba32(255, 255, 255, 255));
                for (int i=0;i<selectedPalette.colors.Length;i++)
                {
                    distance = Utils.RedMeanColorDifference(color, selectedPalette.colors[i]);
                    if (bestDistance > distance)
                    {
                        distance = bestDistance;
                        newIndex = i;
                    }
                }

            }
            if (newIndex == -1)
                return false;
            
            while (newIndex!=selectedColorIndex)
            {
                System.Windows.Forms.SendKeys.SendWait("Z");
                newIndex++;
                newIndex %= selectedPalette.colors.Length;
                Thread.Sleep(sendKeyDelay);
            }

            return true;
        }
    }
    
    public interface IPrinter
    {
        public abstract void Print(Image<Rgba32> image, int rezx, int rezy);
    }
}
