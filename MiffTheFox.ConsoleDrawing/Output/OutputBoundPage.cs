using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox.ConsoleDrawing.Output
{
    public class OutputBoundPage : Page
    {
        public IConsoleOutput Output { get; }

        public OutputBoundPage(IConsoleOutput output, int width, int height) : base(width, height)
        {
            Output = output;
        }

        public void Write() => Output.Write(this);
    }
}
