using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox.ConsoleDrawing
{
    public class Page
    {
        private readonly int[,] _Cells;

        public int Width { get; }
        public int Height { get; }

        public ConsoleColor DrawBackground { get; set; }
        public ConsoleColor DrawForeground { get; set; }

        public Page(int width, int height)
        {
            Width = width;
            Height = height;
            _Cells = new int[Width, Height];
        }

        public char GetCell(int x, int y, out ConsoleColor background, out ConsoleColor foregound)
            => CellEncoding.Decode(_Cells[x, y], out background, out foregound);

        public void SetCell(int x, int y, char ch, ConsoleColor background, ConsoleColor foreground)
            => _Cells[x, y] = CellEncoding.Encode(ch, background, foreground);

        public void SetColor(int x, int y, ConsoleColor background, ConsoleColor foregound)
        {
            SetCell(x, y, (char)(_Cells[x, y] & 0xFFFF), background, foregound);
        }

        public void Clear(ConsoleColor background, ConsoleColor foreground)
        {
            int c = CellEncoding.Encode(' ', background, foreground);
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    _Cells[i, j] = c;
                }
            }
        }

        public void Clear() => Clear(DrawBackground, DrawForeground);

        public void DrawText(int x, int y, string text)
        {
            for (int i = 0; i < text.Length && (x + i) < Width; i++)
            {
                SetCell(x + i, y, text[i], DrawBackground, DrawForeground);
            }
        }
    }
}
