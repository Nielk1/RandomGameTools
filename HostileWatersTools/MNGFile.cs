using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
//using System.IO.Compression;
using System.Linq;
using System.Text;
//using zlib;

namespace HostileWatersTools
{
    class MngFile
    {
        private bool streamed;
        private bool readOnly;
        private Stream data;

        public UInt32 fileCount;

        public List<MngEntry> Entries;

        public struct MngEntry
        {
            public string Filename;
            public UInt32 Size;
            public UInt32 Unknown;
            public UInt32 Offset;
        }

        public MngFile(Stream data)
        {
            long pos = data.Position;

            readOnly = true;
            streamed = true;
            this.data = data;

            byte[] buffer = new byte[4];
            data.Read(buffer,0,4);
            if (buffer[0] != 0x5A || buffer[1] != 0x47 || buffer[2] != 0x57 || buffer[3] != 0x48)
            {
                throw new FormatException("Type indicator ZGWH not found");
            }

            data.Read(buffer, 0, 4);
            fileCount = BitConverter.ToUInt32(buffer, 0);

            Entries = new List<MngEntry>();

            for(UInt32 i = 0; i < fileCount; i++)
            {
                MngEntry entry = new MngEntry();

                StringBuilder filename = new StringBuilder();
                byte readByte = (byte)data.ReadByte();
                while (readByte != 0x00)
                {
                    filename.Append((char)readByte);
                    readByte = (byte)data.ReadByte();
                }

                entry.Filename = filename.ToString();

                data.Read(buffer, 0, 4);
                entry.Size = BitConverter.ToUInt32(buffer, 0);

                data.Read(buffer, 0, 4);
                entry.Unknown = BitConverter.ToUInt32(buffer, 0);

                data.Read(buffer, 0, 4);
                entry.Offset = BitConverter.ToUInt32(buffer, 0);

                Entries.Add(entry);
            }
        }

        public byte[] GetFile(int index)
        {
            if (index >= Entries.Count) throw new IndexOutOfRangeException("Index not in file table");

            MngEntry entry = Entries[index];
            byte[] rawFile = new byte[entry.Size];
            data.Position = entry.Offset;
            data.Read(rawFile, 0, (int)entry.Size);

            //switch (entry.Compression)
            //{
            //    case ZorkCompression.LZO1X:
            //        rawFile = lzo.Decompress_LZO1X(rawFile, (int)entry.UncompressedSize);
            //        break;
            //    case ZorkCompression.LZO1Y:
            //        rawFile = lzo.Decompress_LZO1Y(rawFile, (int)entry.UncompressedSize);
            //        break;
            //}
            //if (Encrypted)
            //{
            //    rawFile = XOR(rawFile, EncryptionKey);
            //}

            rawFile = Decompress(rawFile);

            return rawFile;
        }

        static byte[] Decompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            //using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var zipStream = new ZlibStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
