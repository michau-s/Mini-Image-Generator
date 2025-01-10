using System.Runtime.InteropServices;
using ImSh = SixLabors.ImageSharp;

namespace Frontend
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            const int width = 1024;
            const int height = 1024;

            ImSh.Image<ImSh::PixelFormats.Rgba32> image = new(width, height);


            image.DangerousTryGetSinglePixelMemory(out Memory<ImSh::PixelFormats.Rgba32> memory);
            var span = memory.Span;

            for (int i = 0; i < width; i++)
            {
                int red = (255 * i) / width;
                for (int j = 0; j < height; j++)
                {
                    int blue = (255 * j) / height;
                    span[i * width + j].R = (byte)red;
                    span[i * width + j].G = (byte)((red * blue) / 255);
                    span[i * width + j].B = (byte)blue;
                    span[i * width + j].A = 255;
                }
            }

            ImSh.Formats.Jpeg.JpegEncoder encoder = new();
            FileStream fs = new($"./Image_0.jpeg", FileMode.OpenOrCreate, FileAccess.Write);
            encoder.Encode(image, fs);
            image.Dispose();
        }
    }
}
