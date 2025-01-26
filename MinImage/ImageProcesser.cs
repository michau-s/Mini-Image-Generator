using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MinImage
{
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
                Console.WriteLine($"An exception occure while blurring an image");
                Marshal.FreeHGlobal(texture);
                throw;
            }
        }
    }
}
