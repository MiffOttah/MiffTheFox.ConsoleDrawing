using MiffTheFox.ConsoleDrawing.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace MiffTheFox.ConsoleDrawing.Win32
{
    public sealed class Win32ConsoleOutput : IConsoleOutput, IDisposable
    {
        private bool _Open = true;
        private readonly SafeFileHandle _Handle;

        private readonly Native.CharInfo[] _CharInfo;
        private readonly ConsoleColor _BackgroundColor;
        private readonly ConsoleColor _ForegroundColor;

        private readonly Native.Coord _BufferSize;
        private readonly Native.Coord _ZeroCoord = new Native.Coord { X = 0, Y = 0 };
        private readonly Encoding _CodePage;

        private Win32ConsoleOutput(ConsoleColor backgroundColor, ConsoleColor foregroundColor, int width, int height, Encoding codepage)
        {
            _Handle = Native.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (_Handle.IsInvalid)
            {
                Dispose();
                throw new IOException("Cannot open the native win32 console handle.");
            }

            _CharInfo = new Native.CharInfo[width * height];
            _BufferSize = new Native.Coord { X = (short)width, Y = (short)height };
            _BackgroundColor = backgroundColor;
            _ForegroundColor = foregroundColor;
            _CodePage = codepage;
        }

        public static Win32ConsoleOutput Create(int? codepage = null)
        {
            // set up system.console before opening handle
            Console.BufferWidth = Console.WindowWidth;
            Console.BufferHeight = Console.WindowHeight;

            Encoding codepageEncoding = null;
            if (codepage.HasValue)
            {
                codepageEncoding = CodePagesEncodingProvider.Instance.GetEncoding(codepage.Value);
                Console.OutputEncoding = codepageEncoding;
            }

            Console.CursorVisible = false; // for some reason this has to be set after changing the encoding

            return new Win32ConsoleOutput(Console.BackgroundColor, Console.ForegroundColor, Console.WindowWidth, Console.WindowHeight, codepageEncoding);
        }

        public OutputBoundPage CreatePageFromBuffer()
        {
            var page = new OutputBoundPage(this, _BufferSize.X, _BufferSize.Y)
            {
                DrawBackground = _BackgroundColor,
                DrawForeground = _ForegroundColor
            };
            page.Clear();
            return page;
        }

        public void Write(Page page)
        {
            int i = 0;
            for (int y = 0; y < page.Height; y++)
            {
                for (int x = 0; x < page.Width; x++)
                {
                    ConsoleColor background, foreground;

                    if (_CodePage == null)
                    {
                        _CharInfo[i].Char.UnicodeChar = page.GetCell(x, y, out background, out foreground);
                    }
                    else
                    {
                        Span<byte> b = stackalloc byte[1];
                        Span<char> c = stackalloc char[1];
                        c[0] = page.GetCell(x, y, out background, out foreground);
                        if (_CodePage.GetBytes(c, b) == 1)
                        {
                            _CharInfo[i].Char.AsciiChar = b[0];
                        }
                        else
                        {
                            _CharInfo[i].Char.AsciiChar = 0x20; // default to space
                        }
                    }

                    _CharInfo[i].Attributes = (short)(((int)background << 4) | (int)foreground);
                    i++;
                }
            }

            var lpWriteRegion = new Native.SmallRect { Left = 0, Top = 0, Right = _BufferSize.X, Bottom = _BufferSize.Y };
            Native.WriteConsoleOutput(_Handle, _CharInfo, _BufferSize, _ZeroCoord, ref lpWriteRegion);
        }

        void _Dispose()
        {
            if (_Open)
            {
                _Handle.Close();
                _Handle.Dispose();
                _Open = false;
            }
        }

        ~Win32ConsoleOutput()
        {
            _Dispose();
        }

        public void Dispose()
        {
            _Dispose();
            GC.SuppressFinalize(this);
        }
    }

    internal static class Native
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteConsoleOutput(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion
        );

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct CharUnion
        {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo
        {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }
    }
}
