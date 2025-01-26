using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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

        [LibraryImport(LibName)]
        static partial void Blur(IntPtr array, int width, int height, int w, int h, TryReportCallback tryReportCallback);

        public IntPtr BlurImage(IntPtr texture, int width, int height, int w, int h)
        {
            try
            {
                bool Progres(float progress)
                {
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

        //TODO: fix this
        public IntPtr DrawCirclesImage(IntPtr texture, int width, int height, float radius, int circleCount)
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

        public IntPtr ColorCorrectionImage(IntPtr texture, int width, int height, float red, float green, float blue)
        {
            try
            {
                bool Progres(float progress)
                {
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

        public IntPtr GammaCorrectionImage(IntPtr texture, int width, int height, float gamma)
        {
            try
            {
                bool Progres(float progress)
                {
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
    }
}
