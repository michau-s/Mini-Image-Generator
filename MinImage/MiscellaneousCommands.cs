using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImSh = SixLabors.ImageSharp;


namespace MinImage
{
    /// <summary>
    /// Various commands that did not fit into any other class
    /// </summary>
    internal class MiscellaneousCommands
    {
        /// <summary>
        /// Save an image to the disk at the specified path
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="path"></param>
        public void Output(IntPtr texture, int width, int height, string path)
        {
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
                        span[i * height + j].R = pixels[i * height + j].r;
                        span[i * height + j].G = pixels[i * height + j].g;
                        span[i * height + j].B = pixels[i * height + j].b;
                        span[i * height + j].A = pixels[i * height + j].a;
                    }
                }
            }

            // from the starter code
            ImSh.Formats.Jpeg.JpegEncoder encoder = new();
            FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write);
            encoder.Encode(image, fs);
            image.Dispose();
        }
        
        /// <summary>
        /// Input an image from the disk
        /// </summary>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public IntPtr Input(string path, out int width, out int height)
        {
            var image = ImSh.Image.Load<ImSh.PixelFormats.Rgba32>(path);
            width = image.Width;
            height = image.Height;

            // from the starter code
            int size = width * height * Marshal.SizeOf(typeof(MyColor));
            IntPtr texture = Marshal.AllocHGlobal(size);
            image.DangerousTryGetSinglePixelMemory(out Memory<ImSh::PixelFormats.Rgba32> memory);
            var span = memory.Span;

            unsafe
            {
                MyColor* pixels = (MyColor*)texture;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        pixels[i * height + j].r = span[i * height + j].R;
                        pixels[i * height + j].g = span[i * height + j].G;
                        pixels[i * height + j].b = span[i * height + j].B;
                        pixels[i * height + j].a = span[i * height + j].A;
                    }
                }
            }

            return texture;
        }

        /// <summary>
        /// Display help
        /// </summary>
        public void Help()
        {
            Console.Write(
@"
Generating commands:
    Input <filename>                        - Load an image from the disk.
    Generate <n> <width> <height>           - Create <n> images whose size is <width>x<height> pixels and fill them with random patterns.
    
Processing commands:
    Output <filename_prefix>                - Save images to the disk.
    Blur <w> <h>                            - Apply a <w>x<h> blur.
    RandomCircles <n> <r>                   - Add <n> circles of radius <r> placed randomly on the images.
    Room <x1> <y1> <x2> <y2>                - Draw a filled rectangle with the given coordinates. The coordinates range from 0 to 1.
    ColorCorrection <red> <green> <blue>    - Apply color correction. (values [0-1])
    GammaCorrection <gamma>                 - Apply Gamma correction. (values [0-1])

Misc:
    Exit                                    - Exit the program.
    Clear                                   - Clear the terminal.

");
        }

        /// <summary>
        /// Helper function for freeing the image
        /// </summary>
        /// <param name="texture"></param>
        public void FreeImage(IntPtr texture)
        {
            Marshal.FreeHGlobal(texture);
        }
    }
}
