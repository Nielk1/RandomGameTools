using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ObulisPK2Extract
{
    class Pk2File
    {
        private bool streamed;
        private bool readOnly;
        private Stream data;

        //public UInt32 header1;
        //public UInt32 header2;
        //public byte xorBase;
        //public byte[] header_15;
        //public UInt32 fileCount;

        public List<Pk2Entry> Entries;

        //private long dataOffset;

        public struct Pk2Entry
        {
            public char[] Magic;
            public UInt32 Unknown1;
            public UInt32 Size;
            public string Filename;
            public long Offset;
        }

        public Pk2File(Stream data)
        {
            long pos = data.Position;

            readOnly = true;
            streamed = true;
            this.data = data;

            byte[] buffer = new byte[4];
            //data.Read(buffer,0,4);
            //if (buffer[0] != 0x5A || buffer[1] != 0x47 || buffer[2] != 0x57 || buffer[3] != 0x48)
            //{
            //    throw new FormatException("Type indicator ZGWH not found");
            //}

            Entries = new List<Pk2Entry>();
            while(data.Read(buffer,0,4) > 0)
            {
                Pk2Entry entry = new Pk2Entry();

                entry.Magic = new char[4];
                for (int i = 0; i < 4; i++)
                    entry.Magic[i] = (char)buffer[i];
                data.Read(buffer, 0, 4);
                entry.Unknown1 = BitConverter.ToUInt32(buffer, 0);
                data.Read(buffer, 0, 4);
                entry.Size = BitConverter.ToUInt32(buffer, 0);
                int nameLength = data.ReadByte();
                nameLength = (int)(Math.Ceiling((nameLength + 1) / 4.0f) * 4) - 1; // alignment and a 1 offset due to the length byte being part of it
                byte[] tmpFilename = new byte[nameLength];
                data.Read(tmpFilename, 0, (int)nameLength);
                entry.Filename = new string(Encoding.ASCII.GetChars(tmpFilename));
                entry.Offset = data.Position;
                data.Seek((int)(Math.Ceiling(entry.Size / 4.0f) * 4), SeekOrigin.Current); // skip past the data
                Entries.Add(entry);
            }

            //dataOffset = data.Position;
        }

        public byte[] GetFile(int index)
        {
            if (index >= Entries.Count) throw new IndexOutOfRangeException("Index not in file table");

            Pk2Entry entry = Entries[index];

            data.Position = entry.Offset;
            byte[] rawFile = new byte[entry.Size];
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

                //rawFile = Decompress(rawFile);

                return rawFile;
        }
    }
}
