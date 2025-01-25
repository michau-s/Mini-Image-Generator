using MinImage;
using System.Runtime.InteropServices;
using ImSh = SixLabors.ImageSharp;

namespace Frontend
{
    internal partial class Program
    {
        static void Main(string[] args)
        {
            ImageGenerator.Generate(1024, 1024);
        }
    }
}
