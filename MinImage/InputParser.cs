﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinImage
{
    //TODO: Improve this class

    /// <summary>
    /// A class which handles command chain parsing
    /// </summary>
    internal class InputParser
    {
        private readonly string[] generatingCommands = {"Generate", "Input", "CustomGen1", "CustomGen2", "CustomGen3"};
        private readonly string[] processingCommands = {"Output", "Blur", "RandomCircles", "Room", "ColorCorrection", "GammaCorrection", "CustomProc1", "CustomProc2", "CustomProc3" };

        //TODO: add custom exception to indicate where the syntax error occured;

        /// <summary>
        /// Main method providing an interface for command chain parsing
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks whether a command chain starts with a generating comamand
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks whether a command chain is a valid command chain
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool isValidCommand(string command)
        {
            var split = command.Split(' ');
            //TODO: add custom gen and proc commands
            switch (split[0])
            {
                case "Generate":
                    if (split.Length != 4
                        || !int.TryParse(split[1], out int width)
                        || !int.TryParse(split[2], out int height)
                        || width < 0
                        || height < 0)
                    {
                        return false;
                    }
                    break;
                case "Input":
                    if (split.Length != 2 || !File.Exists(split[1]))
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
                        || !int.TryParse(split[2], out int h)
                        || w < 0
                        || h < 0)
                    {
                        return false;
                    }
                    break;
                case "RandomCircles":
                    if (split.Length != 3
                        || !int.TryParse(split[1], out int n)
                        || !float.TryParse(split[2], out float r)
                        || n < 0
                        || r < 0)
                    {
                        return false;
                    }
                    break;
                case "Room":
                    if (split.Length != 5
                        || !float.TryParse(split[1], out float x1)
                        || !float.TryParse(split[2], out float y1)
                        || !float.TryParse(split[3], out float x2)
                        || !float.TryParse(split[4], out float y2)
                        || x1 < 0
                        || x2 < 0
                        || y1 < 0
                        || y2 < 0
                        || x1 > 1
                        || x2 > 1
                        || y1 > 1
                        || y2 > 1)
                    {
                        return false;
                    }
                    break;
                case "ColorCorrection":
                    if (split.Length != 4
                        || !float.TryParse(split[1], out float red)
                        || !float.TryParse(split[2], out float green)
                        || !float.TryParse(split[3], out float blue)
                        || red < 0
                        || green < 0
                        || blue < 0
                        || red > 1
                        || green > 1
                        || blue > 1)
                    {
                        return false;
                    }
                    break;
                case "GammaCorrection":
                    if (split.Length != 2 || !float.TryParse(split[1], out float gamma) || gamma < 0 || gamma > 1)
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
