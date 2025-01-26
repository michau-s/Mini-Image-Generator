using System;
using System.Collections.Generic;
using System.Text;

namespace MinImage
{
    // https://github.com/WUT-MiNI/P3A_24Z_Lab11

    /// <summary>
    /// Struct used to store worker data
    /// </summary>
    public record WorkerData
    {
        public int Progress = 0;
        public int CommandsFinished = 0;
    }

    /// <summary>
    /// A Class used to draw the progress bar while processing images
    /// </summary>
    public class ProgressReporter
    {
        // For locking
        private readonly object obj = new Object();
        private readonly int barSize = 70;
        private int commandsCount;
        // Key is image index, value is WorkerData
        private Dictionary<int, WorkerData> workers = new Dictionary<int, WorkerData>();

        /// <summary>
        /// Used to initialize the reporter
        /// </summary>
        /// <param name="imagesNo">total number of images</param>
        /// <param name="commandsNo">total number of commands</param>
        public void Init(int imagesNo, int commandsNo)
        {
            lock (obj)
            {
                commandsCount = commandsNo;
                for (int i = 0; i < imagesNo; i++)
                {
                    workers[i] = new WorkerData();
                }
            }
        }

        /// <summary>
        /// Called when a command has finished
        /// </summary>
        /// <param name="index">index of the image being processed by the worker</param>
        public void CommandFinished(int index)
        {
            lock (obj)
            {
                workers[index].CommandsFinished++;

                if (workers[index].CommandsFinished == commandsCount)
                {
                    workers[index].Progress = 100;
                }
                Redraw();
            }
        }

        /// <summary>
        /// Used while subscribing to the event
        /// </summary>
        /// <param name="index">index of the image being processed by the worker</param>
        /// <param name="progress">current progress</param>
        public void ReportProgress(int index, int progress)
        {
            lock (obj)
            {
                workers[index].Progress = progress / commandsCount + workers[index].CommandsFinished * 100 / commandsCount;
                Redraw();
            }
        }

        private void Redraw()
        {
            lock (obj)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Press 'x' to cancel");

                foreach (var (index, workerData) in workers)
                {
                    var progressBar = DrawProgressBar(workerData.Progress, 100, barSize);
                    sb.AppendLine($" Image: {index,-10} {progressBar}");
                }

                Console.SetCursorPosition(0, 0);
                Console.Write(sb.ToString());
            }
        }

        private string DrawProgressBar(int progress, int total, int barSize)
        {
            double percentage = (double)progress / total;
            int filled = (int)Math.Round(percentage * barSize);

            char[] barChars = new char[barSize];
            for (int i = 0; i < barSize; i++)
            {
                barChars[i] = i < filled ? '#' : '-';
            }

            // Some logic for adding the | characters to seperate the process chain
            for (int i = barSize / commandsCount; i < barSize - barSize % commandsCount; i += barSize / commandsCount)
            {
                barChars[i] = '|';
            }

            return $"[{new string(barChars)}] {progress}%";
        }
    }
}
