using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiffTheFox.ConsoleDrawing.Output
{
    public class AnsiConsoleOutput : IConsoleOutput, IDisposable
    {
        protected TextWriter Writer { get; }

        const string CSI = "\x1B[";
        const string VT_CursorOff = CSI + "?25l";
        const string VT_ClearScreen = CSI + "3J";

        public int Width { get; }
        public int Height { get; }

        public AnsiConsoleOutput(Stream stdout, Encoding? encoding = null, bool leaveOpen = false, int width = 80, int height = 25)
            : this(new StreamWriter(stdout, encoding ?? new UTF8Encoding(false), leaveOpen: leaveOpen), width, height)
        {
        }

        public AnsiConsoleOutput(TextWriter writer, int width = 80, int height = 25)
        {
            Writer = writer;
            Width = width;
            Height = height;
        }

        public AnsiConsoleOutput(Encoding? encoding = null, int width = 80, int height = 25)
            : this(Console.OpenStandardOutput(), encoding, width: width, height: height)
        {
        }

        public OutputBoundPage CreatePageFromBuffer()
        {
            // hide cursor
            Writer.Write(VT_CursorOff);

            return new OutputBoundPage(this, Width, Height);
        }

        public void Write(Page page)
        {
            // get default colors
            page.GetCell(0, 0, out var bg, out var fg);
            var lastBg = bg;
            var lastFg = fg;
            SetBackgroundColor(bg);
            SetForegroundColor(fg);
            
            Writer.Write(VT_ClearScreen);

            for (int y = 0; y < page.Height; y++)
            {
                // set cursor position to current row
                Writer.Write($"{CSI}{y + 1};1H");

                for (int x = 0; x < page.Width; x++)
                {
                    char c = page.GetCell(x, y, out bg, out fg);

                    if (bg != lastBg)
                    {
                        SetBackgroundColor(bg);
                        lastBg = bg;
                    }

                    if (fg != lastFg)
                    {
                        SetForegroundColor(fg);
                        lastFg = fg;
                    }

                    Writer.Write(c);
                }
            }
        }

        protected void SetBackgroundColor(ConsoleColor color)
        {
            Writer.Write(CSI);
            // hard-coded escape sequences for best performance
            Writer.Write(color switch
            {
                ConsoleColor.Red => "1;41m",
                ConsoleColor.Green => "1;42m",
                ConsoleColor.Yellow => "1;43m",
                ConsoleColor.Blue => "1;44m",
                ConsoleColor.Magenta => "1;45m",
                ConsoleColor.Cyan => "1;46m",
                ConsoleColor.Gray => "1;47m",
                ConsoleColor.DarkGray => "1;100m",
                ConsoleColor.DarkRed => "1;101m",
                ConsoleColor.DarkGreen => "1;102m",
                ConsoleColor.DarkYellow => "1;103m",
                ConsoleColor.DarkBlue => "1;104m",
                ConsoleColor.DarkMagenta => "1;105m",
                ConsoleColor.DarkCyan => "1;106m",
                ConsoleColor.White => "1;107m",
                _ => "1;40m" // default to black
            });
        }

        protected void SetForegroundColor(ConsoleColor color)
        {
            // hard-coded escape sequences for best performance
            Writer.Write(CSI);
            Writer.Write(color switch
            {
                ConsoleColor.Black => "1;30m",
                ConsoleColor.Red => "1;31m",
                ConsoleColor.Green => "1;32m",
                ConsoleColor.Yellow => "1;33m",
                ConsoleColor.Blue => "1;34m",
                ConsoleColor.Magenta => "1;35m",
                ConsoleColor.Cyan => "1;36m",
                ConsoleColor.Gray => "1;37m",
                ConsoleColor.DarkGray => "1;90m",
                ConsoleColor.DarkRed => "1;91m",
                ConsoleColor.DarkGreen => "1;92m",
                ConsoleColor.DarkYellow => "1;93m",
                ConsoleColor.DarkBlue => "1;94m",
                ConsoleColor.DarkMagenta => "1;95m",
                ConsoleColor.DarkCyan => "1;96m",
                ConsoleColor.White => "1;97m",
                _ => "1;97m" // default to white
            });
        }

        public void Dispose()
        {
            Writer.Dispose();
        }
    }
}
