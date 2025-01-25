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
    static partial class ImageGenerator
    {
        private const string LibName = "ImageGenerator";

        [StructLayout(LayoutKind.Sequential)]
        private struct MyColor
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate bool TryReportCallback(float progress);

        [LibraryImport(LibName)]
        static partial void GenerateImage(IntPtr array, int width, int height, TryReportCallback tryReportCallback);

        public static void Generate(int width, int height)
        {
            int size = width * height * Marshal.SizeOf(typeof(MyColor));
            IntPtr texture = new IntPtr();

            texture = Marshal.AllocHGlobal(size);

            try
            {
                bool Progres(float progress)
                {
                    return true;
                }

                GenerateImage(texture, width, height, Progres);

                // from the starter code
                ImSh.Image<ImSh::PixelFormats.Rgba32> image = new(width, height);
                image.DangerousTryGetSinglePixelMemory(out Memory<ImSh::PixelFormats.Rgba32> memory);
                var span = memory.Span;

                unsafe
                {
                    MyColor* pixels = (MyColor*)texture;
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            span[i * width + j].R = pixels[i * width + j].r;
                            span[i * width + j].G = pixels[i * width + j].g;
                            span[i * width + j].B = pixels[i * width + j].b;
                            span[i * width + j].A = pixels[i * width + j].a;
                        }
                    }
                }

                // from the starter code
                ImSh.Formats.Jpeg.JpegEncoder encoder = new();
                FileStream fs = new($"./Image_0.jpeg", FileMode.OpenOrCreate, FileAccess.Write);
                encoder.Encode(image, fs);
                image.Dispose();
            }
            finally
            {
                Marshal.FreeHGlobal(texture);
            }
        }
    }
}
