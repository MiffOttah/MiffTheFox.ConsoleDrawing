using System;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace MiffTheFox.ConsoleDrawing
{
    partial class Page
    {
        const ulong SERIALIZATION_MAGIC_NUMBER = 0x0A5441446443784D;

        public void Save(Stream stream)
        {
            Span<byte> magic = stackalloc byte[8];
            BitConverter.TryWriteBytes(magic, SERIALIZATION_MAGIC_NUMBER);
            stream.Write(magic);

            using var gzip = new GZipStream(stream, CompressionMode.Compress, true);
            using var writer = new BinaryWriter(gzip, Encoding.ASCII, true);

            // int32 - width
            writer.Write(Width);

            // int32 - height
            writer.Write(Height);

            // 128 bits of reserved
            writer.Write((ulong)0);
            writer.Write((ulong)0);

            // write data top to bottom, left to right
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    writer.Write(_Cells[x, y]);
                }
            }
        }

        public void Save(string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            Save(stream);
        }

        public static Page Load(Stream stream)
        {
            {
                Span<byte> magic = stackalloc byte[8];
                if (stream.Read(magic) != magic.Length || BitConverter.ToUInt64(magic) != SERIALIZATION_MAGIC_NUMBER) throw new PageLoadException();
            } // magic goes out of scope

            using var gzip = new GZipStream(stream, CompressionMode.Decompress, true);

            int doIntRead()
            {
                Span<byte> intReadBuffer = stackalloc byte[4];
                if (gzip?.Read(intReadBuffer) != intReadBuffer.Length) throw new PageLoadException();
                return BitConverter.ToInt32(intReadBuffer);
            }

            int width = doIntRead();
            int height = doIntRead();

            // read reserved values
            {
                Span<byte> reserved = stackalloc byte[16];
                if (gzip.Read(reserved) != reserved.Length) throw new PageLoadException();
            } // reserved goes out of scope

            var page = new Page(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    page._Cells[x, y] = doIntRead();
                }
            }
            return page;
        }

        public static Page Load(string path)
        {
            using var stream = new FileStream(path, FileMode.Open);
            return Load(stream);
        }
    }

    public class PageLoadException : Exception
    {
        public PageLoadException() : base("Page file is not in a correct format.")
        {
        }
    }
}
