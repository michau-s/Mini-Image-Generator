using MinImage;
using System.Runtime.InteropServices;
using ImSh = SixLabors.ImageSharp;

namespace Frontend
{
    internal partial class Program
    {
        static async Task Main(string[] args)
        {
            var generator = new ImageGenerator();
            var image = await generator.Generate(1024, 1024);
            var misc = new MiscellaneousCommands();
            misc.Output(image, 1024, 1024, $"./image1.jpeg");
            misc.Help();
        }
    }
}
