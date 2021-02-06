using System;
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
                colors[i] = Rgba32.ParseHex(hexcodes[i]);
            }
        }
    }
    
    public static class Program
    {
        #region constants
        public static ColorPalette[] palettes = new ColorPalette[]
        {
            new ColorPalette("Main Menu", "#FFA192","#B696ED","#00EAD0","#D8F55D"),
            new ColorPalette("Cave","#2DF3C0","#887DED","#A4F55C","#DB5DED")
        };

        const int startingDelay = 5;//seconds
        const int sendKeyDelay = 25;//miliseconds

        const bool disableColors = false;//for testing
        enum ColorDistanceMode {redmean, hue }
        const ColorDistanceMode colorDistanceMode = ColorDistanceMode.hue;
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
            //TODO: If image isn't RGBA it will throw an exception. Find a way to fix this ¯\_(ツ)_/¯
            using Image<Rgba32> image = (Image<Rgba32>)Image.Load(imagePath);
            image.Mutate((x) => x.Resize(rezx,rezy));
            image.Save("resized.png");

            //do the coloring
            Console.WriteLine("Starting in {0} seconds", startingDelay);
            Console.WriteLine("Make sure you have {0} selected!",selectedPalette.firstColorHex);
            Thread.Sleep(startingDelay*1000);
            Console.WriteLine("Coloring in!");

            IPrinter printer = new Printers.DotMatrixPrinter();
            printer.Print(image,rezx,rezy);

            Console.WriteLine("Done!");
        }

        public static int GetNearestColorIndex(Rgba32 color)
        {
            switch (colorDistanceMode)
            {
                case ColorDistanceMode.redmean:
                    return Color.GetNearestColorIndexRedMean(color);
                case ColorDistanceMode.hue:
                    //TODO: take saturation and value into account and make sure the functions are right!!!
                    return Color.GetNearestColorIndexHue(color);
            }
        }


        public static int selectedColorIndex=0;
        public static void SelectColor(int colorIndex)
        {
            if (disableColors) return;//for testing

            if (colorIndex == -1)return;
            while (colorIndex != selectedColorIndex)
            {
                System.Windows.Forms.SendKeys.SendWait("Z");
                selectedColorIndex++;
                selectedColorIndex %= selectedPalette.colors.Length;
                Thread.Sleep(sendKeyDelay);
            }
        }
    }
    
    public interface IPrinter
    {
        public abstract void Print(Image<Rgba32> image, int rezx, int rezy);
    }
}
