using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox.ConsoleDrawing
{
    internal static class CellEncoding
    {
        internal static int Encode(char ch, ConsoleColor background, ConsoleColor foreground)
            => ch | ((int)background << 24) | ((int)foreground << 16);

        internal static char Decode(int enc, out ConsoleColor background, out ConsoleColor foreground)
        {
            background = (ConsoleColor)((enc >> 24) & 15);
            foreground = (ConsoleColor)((enc >> 16) & 15);
            return (char)(enc & 0xFFFF);
        }
    }
}
