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
        const string VT_ResetFormatting = CSI + "0m";
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
            // set up terminal for TUI
            Writer.Write(VT_CursorOff);
            Writer.Write(VT_ResetFormatting);

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
                ConsoleColor.DarkRed => "41m",
                ConsoleColor.DarkGreen => "42m",
                ConsoleColor.DarkYellow => "43m",
                ConsoleColor.DarkBlue => "44m",
                ConsoleColor.DarkMagenta => "45m",
                ConsoleColor.DarkCyan => "46m",
                ConsoleColor.Gray => "47m",
                ConsoleColor.DarkGray => "100m",
                ConsoleColor.Red => "101m",
                ConsoleColor.Green => "102m",
                ConsoleColor.Yellow => "103m",
                ConsoleColor.Blue => "104m",
                ConsoleColor.Magenta => "105m",
                ConsoleColor.Cyan => "106m",
                ConsoleColor.White => "107m",
                _ => "40m" // default to black
            });
        }

        protected void SetForegroundColor(ConsoleColor color)
        {
            // hard-coded escape sequences for best performance
            Writer.Write(CSI);
            Writer.Write(color switch
            {
                ConsoleColor.Black => "30m",
                ConsoleColor.DarkRed => "31m",
                ConsoleColor.DarkGreen => "32m",
                ConsoleColor.DarkYellow => "33m",
                ConsoleColor.DarkBlue => "34m",
                ConsoleColor.DarkMagenta => "35m",
                ConsoleColor.DarkCyan => "36m",
                ConsoleColor.Gray => "37m",
                ConsoleColor.DarkGray => "90m",
                ConsoleColor.Red => "91m",
                ConsoleColor.Green => "92m",
                ConsoleColor.Yellow => "93m",
                ConsoleColor.Blue => "94m",
                ConsoleColor.Magenta => "95m",
                ConsoleColor.Cyan => "96m",
                _ => "97m" // default to white
            });
        }

        public void Dispose()
        {
            Writer.Dispose();
        }
    }
}
