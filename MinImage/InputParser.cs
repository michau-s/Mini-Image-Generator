using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinImage
{
    //TODO: Improve this class
    internal class InputParser
    {
        private readonly string[] generatingCommands = {"Generate", "Input", "CustomGen1", "CustomGen2", "CustomGen3"};
        private readonly string[] processingCommands = {"Output", "Blur", "RandomCircles", "Room", "ColorCorrection", "GammaCorrection", "CustomProc1", "CustomProc2", "CustomProc3" };

        //TODO: add custom exception to indicate where the syntax error occured;
        public bool parseInput(string input)
        {
            if (input == "Help")
            {
                return true;
            }

            if (!startsWithGenerating(input))
            {
                return false;
            }

            var procCommands = input.Split('|').Select(x => x.Trim()).ToArray();

            foreach (var command in procCommands)
            {
                if (!isValidCommand(command))
                {
                    return false;
                }
            }

            return true;
        }

        private bool startsWithGenerating(string input)
        {
            bool startsWithGen = false;

            foreach (var generatingCommand in generatingCommands)
            {
                if (input.StartsWith(generatingCommand))
                {
                    startsWithGen = true;
                }
            }
            return startsWithGen;
        }

        private bool isValidCommand(string command)
        {
            var split = command.Split(' ');
            //TODO: add custom gen and proc commands
            switch (split[0])
            {
                case "Generate":
                    if (split.Length != 4
                        || !int.TryParse(split[1], out int width)
                        || !int.TryParse(split[2], out int height))
                    {
                        return false;
                    }
                    break;
                case "Input":
                    //TODO: Implement checking if the file exists
                    if (split.Length != 2)
                    {
                        return false;
                    }
                    break;
                case "Output":
                    if (split.Length != 2)
                    {
                        return false;
                    }
                    break;
                case "Blur":
                    if (split.Length != 3
                        || !int.TryParse(split[1], out int w)
                        || !int.TryParse(split[2], out int h))
                    {
                        return false;
                    }
                    break;
                case "RandomCircles":
                    if (split.Length != 3
                        || !int.TryParse(split[1], out int n)
                        || !int.TryParse(split[2], out int r))
                    {
                        return false;
                    }
                    break;
                case "Room":
                    if (split.Length != 5
                        || !int.TryParse(split[1], out int x1)
                        || !int.TryParse(split[2], out int y1)
                        || !int.TryParse(split[3], out int x2)
                        || !int.TryParse(split[4], out int y2))
                    {
                        return false;
                    }
                    break;
                case "ColorCorrection":
                    if (split.Length != 4
                        || !int.TryParse(split[1], out int red)
                        || !int.TryParse(split[2], out int green)
                        || !int.TryParse(split[3], out int blue))
                    {
                        return false;
                    }
                    break;
                case "GammaCorrection":
                    if (split.Length != 2 || !int.TryParse(split[1], out int gamma))
                    {
                        return false;
                    }
                    break;

                case "Help":
                    if (split.Length != 1)
                    {
                        return false;
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }
    }
}
