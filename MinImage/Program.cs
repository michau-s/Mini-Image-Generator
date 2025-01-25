using MinImage;
using System.Runtime.InteropServices;
using ImSh = SixLabors.ImageSharp;

namespace Frontend
{
    internal partial class Program
    {
        static async Task Main(string[] args)
        {


            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                int index = i;

                tasks.Add(Task.Run(async () =>
                {
                    var generator = new ImageGenerator();
                    var misc = new MiscellaneousCommands();

                    var image = await generator.Generate(1024, 1024);
                    misc.Output(image, 1024, 1024, $"./image{index}.jpeg");

                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
