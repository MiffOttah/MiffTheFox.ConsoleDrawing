using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox.ConsoleDrawing.Output
{
    public interface IConsoleOutput
    {
        OutputBoundPage CreatePageFromBuffer();
        void Write(Page page);
    }
}
