using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MinImage
{

    [StructLayout(LayoutKind.Sequential)]
    struct Circle
    {
        public float x, y;
        public float radius;
    }

    partial class ImageProcesser
    {
        private const string LibName = "ImageGenerator";

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool TryReportCallback(float progress);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate MyColor GetColor(float x, float y, MyColor color);

        [LibraryImport(LibName)]
        static partial void Blur(IntPtr array, int width, int height, int w, int h, TryReportCallback tryReportCallback);

        public IntPtr BlurImage(IntPtr texture, int width, int height, int w, int h, CancellationToken cancellationToken)
        {
            try
            {
                bool Progres(float progress)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"Cancelled blurring");
                        return false;
                    }
                    return true;
                }

                Blur(texture, width, height, w, h, Progres);

                return texture;
            }
            catch (Exception)
            {
                Console.WriteLine($"An exception occured while blurring an image");
                Marshal.FreeHGlobal(texture);
                throw;
            }
        }

        [LibraryImport(LibName)]
        static partial void DrawCircles(IntPtr texture, int width, int height, Circle[] circles, int circleCount, TryReportCallback tryReport);

        public IntPtr DrawCirclesImage(IntPtr texture, int width, int height, float radius, int circleCount, CancellationToken cancellationToken)
        {

            try
            {
                var circles = new Circle[circleCount];
                for (int i = 0; i < circleCount; i++)
                {
                    Random rand = new Random();
                    circles[i] = new Circle();
                    circles[i].x = rand.NextSingle();
                    circles[i].y = rand.NextSingle();
                    circles[i].radius = radius / width;
                }

                bool Progres(float progress)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"Cancelled drawing circles");
                        return false;
                    }
                    return true;
                }

                DrawCircles(texture, width, height, circles, circleCount, Progres);

                return texture;
            }
            catch (Exception)
            {
                Console.WriteLine($"An exception occured while drawing the circles");
                Marshal.FreeHGlobal(texture);
                throw;
            }
        }

        [LibraryImport(LibName)]
        static partial void ColorCorrection(IntPtr texture, int width, int height, float red, float green, float blue, TryReportCallback tryReportCallback);

        public IntPtr ColorCorrectionImage(IntPtr texture, int width, int height, float red, float green, float blue, CancellationToken cancellationToken)
        {
            try
            {
                bool Progres(float progress)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"Cancelled color correction");
                        return false;
                    }
                    return true;
                }

                ColorCorrection(texture, width, height, red, green, blue, Progres);

                return texture;
            }
            catch (Exception)
            {
                Console.WriteLine($"An exception occured while applying color correction");
                Marshal.FreeHGlobal(texture);
                throw;
            }
        }

        [LibraryImport(LibName)]
        static partial void GammaCorrection(IntPtr texture, int width, int height, float gamma, TryReportCallback tryReportCallback);

        public IntPtr GammaCorrectionImage(IntPtr texture, int width, int height, float gamma, CancellationToken cancellationToken)
        {
            try
            {
                bool Progres(float progress)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"Cancelled gamma correction");
                        return false;
                    }
                    return true;
                }

                GammaCorrection(texture, width, height, gamma, Progres);

                return texture;
            }
            catch (Exception)
            {
                Console.WriteLine($"An exception occured while applying gamma correction");
                Marshal.FreeHGlobal(texture);
                throw;
            }
        }

        [LibraryImport(LibName)]
        static partial void ProcessPixels_Custom(IntPtr texture, int width, int height, GetColor getColor, TryReportCallback tryReportCallback);

        public IntPtr Room(IntPtr texture, int width, int height, float x1, float y1, float x2, float y2, CancellationToken cancellationToken)
        {
            if (x1 > x2)
            {
                float temp = x1;
                x1 = x2;
                x2 = temp;
            }

            if (y1 > y2)
            {
                float temp = y1;
                y1 = y2;
                y2 = temp;
            }
                
            Random rand = new Random();
            MyColor rectColor = new MyColor();
            rectColor.r = (byte)rand.Next(0, 255);
            rectColor.g = (byte)rand.Next(0, 255);
            rectColor.b = (byte)rand.Next(0, 255);
            rectColor.a = 255;

            MyColor GetColor(float x, float y, MyColor color)
            {

                if (x >= x1 && y >= y1 && x <= x2 && y <= y2)
                {
                    return rectColor;
                }

                return color;
            }
            try
            {
                bool Progres(float progress)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"Cancelled drawing rectangle");
                        return false;
                    }
                    return true;
                }

                ProcessPixels_Custom(texture, width, height, GetColor, Progres);
            }
            catch (Exception)
            {
                Console.WriteLine($"An exception occured while drawing a rectangle");
                Marshal.FreeHGlobal(texture);
                throw;
            }

            return texture;
        }
    }
}
