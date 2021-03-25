using System;
using System.Collections.Generic;
using System.Text;

namespace MiffTheFox.ConsoleDrawing.BoxDrawing
{
    public static class BoxDrawingExtensions
    {
        public static void DrawHLine(this Page page, int x, int y, int width, BorderThickness thickness = BorderThickness.Single)
        {
            var lineChar = thickness == BorderThickness.Double ? '═' : '─';
            for (int i = 0; i < width - 1; i++)
            {
                page.SetCell(
                    x + i,
                    y,
                    lineChar
                );
            }
        }

        public static void DrawVLine(this Page page, int x, int y, int height, BorderThickness thickness = BorderThickness.Single)
        {
            var lineChar = thickness == BorderThickness.Double ? '║' : '│';
            for (int i = 0; i < height - 1; i++)
            {
                page.SetCell(
                    x,
                    y + i,
                    lineChar
                );
            }
        }

        public static void DrawBox(this Page page, int x, int y, int width, int height, BorderThickness thickness = BorderThickness.Single)
        {
            string corners = thickness == BorderThickness.Double ? "╔╗╚╝" : "┌┐└┘";

            DrawHLine(page,
                x + 1,
                y,
                width - 1,
                thickness);
            DrawHLine(page,
                x + 1,
                y + height - 1,
                width - 1,
                thickness);
            DrawVLine(page,
                x,
                y + 1,
                height - 1,
                thickness);
            DrawVLine(page,
                x + width - 1,
                y + 1,
                height - 1,
                thickness);

            page.SetCell(x, y, corners[0]);
            page.SetCell(x + width - 1, y, corners[1]);
            page.SetCell(x, y + height - 1, corners[2]);
            page.SetCell(x + width - 1, y + height - 1, corners[3]);
        }
    }

    public enum BorderThickness
    {
        Single = 0,
        Double = 1
    }
}
