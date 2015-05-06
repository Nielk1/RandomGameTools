using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UltimateTicTacToeDTA
{
    class UltimateTicTacToeDTAFile
    {
        private Stream data;

        public UInt32 Unknown1;
        public UInt32 Unknown2;

        struct DTAData
        {
            public long Offset;
            public long Size;
        }

        List<DTAData> Entries = new List<DTAData>();

        public UltimateTicTacToeDTAFile(Stream data)
        {
            this.data = data;

            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4); Unknown1 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown2 = BitConverter.ToUInt32(buffer, 0);

            while(data.Read(buffer, 0, 4) > 0)
            {
                UInt32 FileSize = BitConverter.ToUInt32(buffer, 0);
                Entries.Add(new DTAData() { Offset = data.Position, Size = FileSize });
                data.Seek(FileSize, SeekOrigin.Current);
            }
        }

        public int CountFiles()
        {
            return Entries.Count;
        }

        public byte[] GetFile(int index)
        {
            if (index >= Entries.Count) throw new IndexOutOfRangeException("Index not in file table");

            DTAData entry = Entries[index];

            data.Position = entry.Offset;
            byte[] rawFile = new byte[entry.Size];

            data.Read(rawFile, 0, (int)entry.Size);

            return rawFile;
        }
    }
}
