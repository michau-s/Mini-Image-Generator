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
            var cancellationTokenSource = new CancellationTokenSource();

            // Displaying the interface
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

            // Thread for checking if cancellation was requested
            Task.Run(() =>
            {
                while(true)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.X )
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                }
            });

            var commands = input.Split('|').Select(x => x.Trim()).ToArray();

            // If the first command is Generate, we need to run n tasks
            // Otherwise, run 1 task
            int n = 1;
            if (commands[0].Split()[0] == "Generate")
            {
                int.TryParse(commands[0].Split()[1], out n);
            }

            var tasks = new List<Task>();

            Console.WriteLine($"Press 'x' to cancel processing");

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

                    // Processing each command one by one
                    foreach (var command in commands)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            Console.WriteLine($"Cancelling Image {index}");
                            return;
                        }
                        
                        var split = command.Split(' ');

                        switch (split[0])
                        {
                            case "Generate":
                                int.TryParse(split[2], out width);
                                int.TryParse(split[3], out height);
                                Texture = generator.Generate(width, height, cancellationTokenSource.Token);
                                break;
                            case "Input":
                                Texture = misc.Input(split[1], out width, out height);
                                
                                break;
                            case "Output":
                                misc.Output(Texture, width, height, split[1] + $"{index}.jpeg");
                                
                                break;
                            case "Blur":
                                int.TryParse(split[1], out int w);
                                int.TryParse(split[2], out int h);
                                Texture = processor.BlurImage(Texture, width, height, w, h, cancellationTokenSource.Token);
                                
                                break;
                            case "RandomCircles":
                                int.TryParse(split[1], out int n);
                                float.TryParse(split[2], out float r);
                                Texture = processor.DrawCirclesImage(Texture, width, height, r, n, cancellationTokenSource.Token);
                                
                                break;
                            case "Room":
                                float.TryParse(split[1], out float x1);
                                float.TryParse(split[2], out float y1);
                                float.TryParse(split[3], out float x2);
                                float.TryParse(split[4], out float y2);
                                Texture = processor.Room(Texture, width, height, x1, y1, x2, y2, cancellationTokenSource.Token);
                                
                                break;
                            case "ColorCorrection":
                                float.TryParse(split[1], out float red);
                                float.TryParse(split[2], out float green);
                                float.TryParse(split[3], out float blue);
                                Texture = processor.ColorCorrectionImage(Texture, width, height, red, green, blue, cancellationTokenSource.Token);
                                
                                break;
                            case "GammaCorrection":
                                float.TryParse(split[1], out float gamma);
                                Texture = processor.GammaCorrectionImage(Texture, width, height, gamma, cancellationTokenSource.Token);
                                
                                break;

                            case "Help":
                                break;
                        }
                    }

                    misc.FreeImage(Texture);
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