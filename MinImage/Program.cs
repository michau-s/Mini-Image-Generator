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
            var reporter = new ProgressReporter();

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
                else if (input == "Clear")
                {
                    Console.Clear();
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

            // Starting the work
            var tasks = new List<Task>();
            int count = commands.Length;
            
            //Skipping irrelevant commands in the progress bar
            foreach(var command in commands)
            {
                if (command.Split()[0] == "Help" || command.Split()[0] == "Output" || command.Split()[0] == "Input")
                {
                    count--;
                }
            }

            reporter.Init(n, count);
            Console.Clear();
            Console.CursorVisible = false;

            for (int i = 0; i < n; i++)
            {
                int index = i;

                tasks.Add(Task.Run(() =>
                {
                    var generator = new ImageGenerator();
                    var processor = new ImageProcesser();
                    var misc = new MiscellaneousCommands();

                    // Subscribing to the reporter
                    // The misc class does not need to subscribe
                    // as we don't really want to cancel reading and writing images
                    generator.progressUpdated += (index, progress) =>
                    {
                        reporter.ReportProgress(index, progress);
                    };

                    processor.progressUpdated += (index, progress) =>
                    {
                        reporter.ReportProgress(index, progress);
                    };

                    IntPtr Texture = IntPtr.Zero;
                    int width = 0;
                    int height = 0;

                    // Processing each command one by one
                    foreach (var command in commands)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            return;
                        }
                        
                        var split = command.Split(' ');

                        // The senior programmer fears the intern's switch statement
                        switch (split[0])
                        {
                            case "Generate":
                                int.TryParse(split[2], out width);
                                int.TryParse(split[3], out height);
                                Texture = generator.Generate(width, height, cancellationTokenSource.Token, index);
                                reporter.CommandFinished(index);
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
                                Texture = processor.BlurImage(Texture, width, height, w, h, cancellationTokenSource.Token, index);
                                reporter.CommandFinished(index);
                                break;

                            case "RandomCircles":
                                int.TryParse(split[1], out int n);
                                float.TryParse(split[2], out float r);
                                Texture = processor.DrawCirclesImage(Texture, width, height, r, n, cancellationTokenSource.Token, index);
                                reporter.CommandFinished(index);
                                break;

                            case "Room":
                                float.TryParse(split[1], out float x1);
                                float.TryParse(split[2], out float y1);
                                float.TryParse(split[3], out float x2);
                                float.TryParse(split[4], out float y2);
                                Texture = processor.Room(Texture, width, height, x1, y1, x2, y2, cancellationTokenSource.Token, index);
                                reporter.CommandFinished(index);
                                break;

                            case "ColorCorrection":
                                float.TryParse(split[1], out float red);
                                float.TryParse(split[2], out float green);
                                float.TryParse(split[3], out float blue);
                                Texture = processor.ColorCorrectionImage(Texture, width, height, red, green, blue, cancellationTokenSource.Token, index);
                                reporter.CommandFinished(index);
                                break;

                            case "GammaCorrection":
                                float.TryParse(split[1], out float gamma);
                                Texture = processor.GammaCorrectionImage(Texture, width, height, gamma, cancellationTokenSource.Token, index);
                                reporter.CommandFinished(index);
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