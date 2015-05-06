using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlightControlPack
{
    class PackFile
    {
        private bool streamed;
        private bool readOnly;
        private Stream data;

        public UInt32 header1;
        public UInt32 header2;
        public byte xorBase;
        public byte[] header_15;
        public UInt32 fileCount;

        public List<PackEntry> Entries;

        private long dataOffset;

        public struct PackEntry
        {
            public UInt32 FileIndex;
            public UInt32 Offset;
            public UInt32 Size;
            public string Filename;
        }

        public PackFile(Stream data)
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

            data.Read(buffer, 0, 4); header1 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); header2 = BitConverter.ToUInt32(buffer, 0);
            xorBase = (byte)data.ReadByte();
            header_15 = new byte[16];
            data.Read(header_15, 0, 15);

            data.Read(buffer, 0, 4);
            fileCount = BitConverter.ToUInt32(buffer, 0);

            Entries = new List<PackEntry>();

            for(UInt32 i = 0; i < fileCount; i++)
            {
                PackEntry entry = new PackEntry();

                data.Read(buffer, 0, 4); entry.FileIndex = BitConverter.ToUInt32(buffer, 0);
                data.Read(buffer, 0, 4); entry.Offset = BitConverter.ToUInt32(buffer, 0);
                data.Read(buffer, 0, 4); entry.Size = BitConverter.ToUInt32(buffer, 0);
                data.Read(buffer, 0, 4); UInt32 nameLength = BitConverter.ToUInt32(buffer, 0);
                byte[] tmpFilename = new byte[nameLength];
                data.Read(tmpFilename, 0, (int)nameLength);
                entry.Filename = new string(Encoding.ASCII.GetChars(tmpFilename));

                Entries.Add(entry);
            }

            dataOffset = data.Position;
        }

        public byte[] GetFile(int index)
        {
            if (index >= Entries.Count) throw new IndexOutOfRangeException("Index not in file table");

            PackEntry entry = Entries[index];
            //byte[] rawFile = new byte[entry.Size];
            //data.Position = entry.Offset + dataOffset;
            //data.Read(rawFile, 0, (int)entry.Size);

            byte[] buffer = new byte[4];
            data.Position = entry.Offset + dataOffset;
            data.Read(buffer, 0, 4); UInt32 size = BitConverter.ToUInt32(buffer, 0);

            byte[] rawFile = new byte[size];
            data.Read(rawFile, 0, (int)size);
            byte xor = xorBase;

            for (int i = 0; i < size;i++)
            {
                rawFile[i] = (byte)(rawFile[i] ^ xor);
                xor = (byte)((xor + 3) & 0xff);
            }



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
