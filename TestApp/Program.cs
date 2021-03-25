using System;
using MiffTheFox.ConsoleDrawing.Win32;
using MiffTheFox.ConsoleDrawing.BoxDrawing;
using MiffTheFox.ConsoleDrawing;

namespace TestApp
{
    class Program
    {
        static void Main()
        {
            //Console.OutputEncoding = System.Text.CodePagesEncodingProvider.Instance.GetEncoding(437);

            using var output = Win32ConsoleOutput.Create(codepage: 437);
            var page = output.CreatePageFromBuffer();
            page.Clear();

            page.DrawText(1, 1, "Text!");

            page.DrawHLine(32, 4, 10, BorderThickness.Double);
            
            page.DrawBox(20, 5, 22, 3);
            page.DrawText(21, 6, "This is a box!");

            var subpage = new Page(25, 8);
            subpage.DrawForeground = ConsoleColor.Cyan;
            subpage.DrawBox(0, 0, 25, 8);

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

                    case ConsoleKey.S:
                        page.BlitFrom(subpage, cX, cY);
                        break;

                    case ConsoleKey.B:
                        page.DrawBox(cX, cY, 6, 2, BorderThickness.Double);
                        break;

                    case ConsoleKey.H:
                        page.DrawHLine(cX, cY, 10, BorderThickness.Single);
                        break;

                    case ConsoleKey.V:
                        page.DrawVLine(cX, cY, 5, BorderThickness.Single);
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
