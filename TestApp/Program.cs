using System;
using MiffTheFox.ConsoleDrawing.Win32;
using MiffTheFox.ConsoleDrawing.BoxDrawing;
using MiffTheFox.ConsoleDrawing;
using MiffTheFox.ConsoleDrawing.Output;

namespace TestApp
{
    class Program
    {
        static void Main()
        {
            //Console.OutputEncoding = System.Text.CodePagesEncodingProvider.Instance.GetEncoding(437);

            //using var output = Win32ConsoleOutput.Create(codepage: 437);
            using var output = new AnsiConsoleOutput(Console.OutputEncoding, Console.WindowWidth, Console.WindowHeight);

            var page = output.CreatePageFromBuffer();
            page.DrawBackground = ConsoleColor.Black;
            page.DrawForeground = ConsoleColor.White;
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

            string savePath = System.IO.Path.Combine(Environment.CurrentDirectory, "PageSaveLoad.dat");

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

                    case ConsoleKey.E:
                        page.DrawText(0, 0, "Encoding = " + Console.OutputEncoding.EncodingName);
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

                    case ConsoleKey.X:
                        page.Save(savePath);
                        page.DrawText(0, page.Height - 1, "Save " + savePath);
                        break;

                    case ConsoleKey.Z:
                        page.BlitFrom(Page.Load(savePath), 0, 0);
                        page.DrawText(0, page.Height - 1, "Load " + savePath);
                        break;

                    case ConsoleKey.L:
                        page.DrawText(0, 0, $"{cX},{cY}  ");
                        break;

                    case ConsoleKey.C:
                        DrawColors(page);
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

        private static void DrawColors(OutputBoundPage page)
        {
            var cc = new ConsoleColor[] {
                ConsoleColor.Black,
                ConsoleColor.DarkRed,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkYellow,
                ConsoleColor.DarkBlue,
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkCyan,
                ConsoleColor.Gray,
                ConsoleColor.DarkGray,
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.Blue,
                ConsoleColor.Magenta,
                ConsoleColor.Cyan,
                ConsoleColor.White
            };

            for (int i = 0; i < cc.Length; i++)
            {
                for (int j = 0; j < cc.Length; j++)
                {
                    page.SetCell(i, j, '║', cc[i], cc[j]);
                }
            }
        }
    }
}
