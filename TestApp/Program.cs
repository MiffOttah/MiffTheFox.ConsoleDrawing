﻿using System;
using MiffTheFox.ConsoleDrawing.Win32;
using MiffTheFox.ConsoleDrawing.BoxDrawing;

namespace TestApp
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.GetEncoding(437);
            
            using var output = Win32ConsoleOutput.Create();
            var page = output.CreatePageFromBuffer();
            page.Clear();

            page.DrawText(1, 1, "Text!");

            page.DrawHLine(32, 4, 10, BorderThickness.Double);
            
            //page.DrawBox(20, 5, 10, 3);
            //page.DrawText(21, 6, "This is a box!");

            page.Write();

            int cX = 0;
            int cY = 0;

            ConsoleKey ck = ConsoleKey.Spacebar;
            do
            {
                page.SetColor(cX, cY, page.DrawBackground, ConsoleColor.White);

                switch (ck)
                {
                    case ConsoleKey.LeftArrow:
                        cX = ((cX <= 0) ? page.Width : cX) - 1;
                        break;

                    case ConsoleKey.RightArrow:
                        cX++;
                        if (cX >= page.Width) cX = 0;
                        break;

                    case ConsoleKey.UpArrow:
                        cY = ((cY <= 0) ? page.Height : cY) - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        cY++;
                        if (cY >= page.Height) cY = 0;
                        break;

                    case ConsoleKey.Enter:
                        page.DrawText(cX, cY, "Text!");
                        break;

                    case ConsoleKey.Home:
                        page.DrawBackground = ConsoleColor.Black;
                        break;

                    case ConsoleKey.End:
                        page.DrawBackground = ConsoleColor.Blue;
                        break;
                }

                page.SetColor(cX, cY, ConsoleColor.Red, ConsoleColor.White);
                page.Write();
                ck = Console.ReadKey(true).Key;
            } while (ck != ConsoleKey.Escape);
        }
    }
}
