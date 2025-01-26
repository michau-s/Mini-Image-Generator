using MinImage;
using System.Runtime.InteropServices;
using ImSh = SixLabors.ImageSharp;

namespace Frontend
{
    internal partial class Program
    {
        static async Task Main(string[] args)
        {
            var input = "";
            var parser = new InputParser();
            var misc = new MiscellaneousCommands();

            while (true)
            {
                Console.WriteLine("Enter a command chain in the form:");
                Console.WriteLine("generating_command arg1 arg2 | processing_command1 | processing_command2 arg1 arg2");
                Console.WriteLine("Or type 'Help' for the list of available commands");
                input = Console.ReadLine() ?? "";
                
                if (input == "Help")
                {
                    misc.Help();
                }
                else if (input == "Exit")
                {
                    return;
                }
                else if (!parser.parseInput(input))
                {
                    Console.WriteLine($"Invalid Syntax! Use `Help`");
                    Console.WriteLine();
                }
                else
                {
                    break;
                }
            }


            var commands = input.Split('|').Select(x => x.Trim()).ToArray();

            int n = 1;
            if (commands[0].Split()[0] == "Generate")
            {
                int.TryParse(commands[0].Split()[1], out n);
            }

            var tasks = new List<Task>();

            for (int i = 0; i < n; i++)
            {
                int index = i;

                tasks.Add(Task.Run(() =>
                {
                    var generator = new ImageGenerator();
                    var processor = new ImageProcesser();
                    var misc = new MiscellaneousCommands();

                    IntPtr Texture = IntPtr.Zero;
                    int width = 0;
                    int height = 0;

                    foreach (var command in commands)
                    {
                        var split = command.Split(' ');

                        switch (split[0])
                        {
                            case "Generate":
                                int.TryParse(split[2], out width);
                                int.TryParse(split[3], out height);
                                Texture = generator.Generate(width, height);
                                break;
                            case "Input":
                                //TODO: Implement checking if the file exists
                                
                                
                                break;
                            case "Output":
                                misc.Output(Texture, width, height, split[1] + $"{index}.jpeg");
                                
                                break;
                            case "Blur":
                                int.TryParse(split[1], out int w);
                                int.TryParse(split[2], out int h);
                                Texture = processor.BlurImage(Texture, width, height, w, h);
                                
                                break;
                            case "RandomCircles":
                                int.TryParse(split[1], out int n);
                                int.TryParse(split[2], out int r);
                                
                                
                                break;
                            case "Room":
                                int.TryParse(split[1], out int x1);
                                int.TryParse(split[2], out int y1);
                                int.TryParse(split[3], out int x2);
                                int.TryParse(split[4], out int y2);
                                
                                
                                break;
                            case "ColorCorrection":
                                int.TryParse(split[1], out int red);
                                int.TryParse(split[2], out int green);
                                int.TryParse(split[3], out int blue);
                                
                                
                                break;
                            case "GammaCorrection":
                                int.TryParse(split[1], out int gamma);
                                
                                
                                break;

                            case "Help":
                                break;
                        }
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}

//var generator = new ImageGenerator();
//var processor = new ImageProcesser();
//var misc = new MiscellaneousCommands();

//var image = generator.Generate(1024, 1024);
//Console.WriteLine($"Generated image {index}");


//image = processor.BlurImage(image, 1024, 1024, 50, 50);
//Console.WriteLine($"Blurred image {index}");

//misc.Output(image, 1024, 1024, $"./image{index}.jpeg");