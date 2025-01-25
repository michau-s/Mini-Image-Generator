using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImSh = SixLabors.ImageSharp;


namespace MinImage
{
    internal class MiscellaneousCommands
    {
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
                        span[i * width + j].R = pixels[i * width + j].r;
                        span[i * width + j].G = pixels[i * width + j].g;
                        span[i * width + j].B = pixels[i * width + j].b;
                        span[i * width + j].A = pixels[i * width + j].a;
                    }
                }
            }

            // from the starter code
            ImSh.Formats.Jpeg.JpegEncoder encoder = new();
            FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write);
            encoder.Encode(image, fs);
            image.Dispose();
        }

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
    ColorCorrection <red> <green> <blue>    - Apply color correction.
    GammaCorrection <gamma>                 - Apply Gamma correction.
");
        }
    }
}
