using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HostileWatersTools
{
    class DatFile
    {
        public UInt32 fileCount;

        public List<DatEntry> Entries;

        public struct DatEntry
        {
            public char[] Filename;
            public UInt32 Size;
            public UInt32 Offset;
        }

        public DatFile(Stream data)
        {
            long pos = data.Position;

            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4);
            fileCount = BitConverter.ToUInt32(buffer, 0);

            Entries = new List<DatEntry>();

            for(UInt32 i = 0; i < fileCount; i++)
            {
                DatEntry entry = new DatEntry();
                entry.Filename = new char[12];
                for (int y = 0; y < 12; y++)
                {
                    entry.Filename[y] = (char)data.ReadByte();
                }

                data.Read(buffer, 0, 4);
                entry.Size = BitConverter.ToUInt32(buffer, 0);

                data.Read(buffer, 0, 4);
                entry.Offset = BitConverter.ToUInt32(buffer, 0);

                Entries.Add(entry);
            }
        }
    }
}
