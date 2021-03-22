using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox.ConsoleDrawing.Output
{
    public sealed class SystemConsoleOutput : IConsoleOutput
    {
        private SystemConsoleOutput() { }

        public static SystemConsoleOutput Instance { get; } = new SystemConsoleOutput();

        public OutputBoundPage CreatePageFromBuffer()
        {
            var page = new OutputBoundPage(this, Console.WindowWidth, Console.WindowHeight);
            Console.CursorVisible = false;
            Console.BufferWidth = page.Width;
            Console.BufferHeight = page.Height;
            page.DrawBackground = Console.BackgroundColor;
            page.DrawForeground = Console.ForegroundColor;
            page.Clear();
            return page;
        }

        public void Write(Page page)
        {
            // on windows, the last cell of the console can't be written to without
            // scrolling the console, so we have to just leave it blank.
            // we can set the background color when initially clearing the console

            int lastX = page.Width - 1;
            int lastY = page.Height - 1;

            page.GetCell(lastX, lastY, out var background, out var foreground);
            Console.BackgroundColor = background;
            Console.ForegroundColor = background;
            Console.Clear();

            for (int y = 0; y < page.Height; y++)
            {
                for (int x = 0; x < page.Width; x++)
                {
                    // do not draw final cell (see comment above)
                    if (x == lastX && y == lastY) break;

                    char c = page.GetCell(x, y, out background, out foreground);
                    Console.BackgroundColor = background;
                    Console.ForegroundColor = foreground;
                    Console.Write(c);
                }
            }
        }
    }
}
