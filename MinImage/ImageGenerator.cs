using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImSh = SixLabors.ImageSharp;

namespace MinImage
{
    [StructLayout(LayoutKind.Sequential)]
    struct MyColor
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
    }
    
    /// <summary>
    /// A class for "Generate" methods
    /// </summary>
    partial class ImageGenerator
    {
        private const string LibName = "ImageGenerator";


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool TryReportCallback(float progress);

        [LibraryImport(LibName)]
        static partial void GenerateImage(IntPtr array, int width, int height, TryReportCallback tryReportCallback);

        public IntPtr Generate(int width, int height, CancellationToken cancellationToken)
        {
            int size = width * height * Marshal.SizeOf(typeof(MyColor));
            IntPtr texture = new IntPtr();

            texture = Marshal.AllocHGlobal(size);

            try
            {
                bool Progres(float progress)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"Cancelled generating");
                        return false;
                    }
                    return true;
                }

                GenerateImage(texture, width, height, Progres);
                
                return texture;

            }
            catch (Exception)
            {
                Console.WriteLine($"An exception occure while generating an image");
                Marshal.FreeHGlobal(texture);
                throw;
            }
        }
    }
}
