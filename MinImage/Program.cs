using MinImage;
using System.Runtime.InteropServices;
using ImSh = SixLabors.ImageSharp;

namespace Frontend
{
    internal partial class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter a command chain in the form:");
            Console.WriteLine("generating_command arg1 arg2 | processing_command1 | processing_command2 arg1 arg2");
            Console.WriteLine("Or type 'Help' for the list of available commands");

            var input = Console.ReadLine() ?? "";
            var parser = new InputParser();
            if (!parser.parseInput(input))
            {
                Console.WriteLine($"Invalid Syntax! Use `Help`");
                return;
            }

            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                int index = i;

                tasks.Add(Task.Run(async () =>
                {
                    var generator = new ImageGenerator();
                    var processor = new ImageProcesser();
                    var misc = new MiscellaneousCommands();

                    var image = await generator.Generate(1024, 1024);
                    image = await processor.BlurImage(image, 1024, 1024, 50, 50);
                    misc.Output(image, 1024, 1024, $"./image{index}.jpeg");

                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
