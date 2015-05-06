using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaboInvasionHGP
{
    class BaboInvasionHGPFile
    {
        //private bool streamed;
        //private bool readOnly;
        private Stream data;

        //public UInt32 header1;
        //public UInt32 header2;
        //public byte xorBase;
        //public byte[] header_15;
        //public UInt32 fileCount;
        public UInt32 Unknown1;
        public UInt32 Unknown2;
        public UInt32 Offset;
        public UInt16 CountFiles;
        //public UInt32 Files;
        //public UInt32 FirstOffset;
        //public UInt32 Version2;
        //public UInt32 NTABLESIZE;
        //public UInt32 NTABLEZSIZE;
        //public long NTABLEOFFSET;
        //
        public List<HGPEntry> Entries;
        //
        ////private long dataOffset;
        //
        //public struct HpkEntry
        //{
        //    //public char[] Magic;
        //    //public UInt32 Unknown1;
        //    //public UInt32 Size;
        //    public string Filename;
        //    //public long Offset;
        //    
        //    public UInt32 ZSize;
        //    public UInt32 Offset;
        //    public UInt32 Size;
        //    public UInt32 CRC;
        //}

        public struct HGPEntry
        {
            public string Filename;
            public UInt32 Offset;
            public UInt32 ZSize;
            public byte Zipped;
        }

        public BaboInvasionHGPFile(Stream data)
        {
            Entries = new List<HGPEntry>();

            long pos = data.Position;

            //readOnly = true;
            //streamed = true;
            this.data = data;

            byte[] buffer = new byte[4];

            string Magic = ReadUnicodeString(data);
            if(Magic != "HGP0")
            {
                throw new FormatException("Type indicator HGP0 not found");
            }

            data.Read(buffer, 0, 4); Unknown1 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown2 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Offset = BitConverter.ToUInt32(buffer, 0);

            data.Seek(Offset, SeekOrigin.Begin);

            data.Read(buffer, 0, 4); CountFiles = BitConverter.ToUInt16(buffer, 0);
            for(int x=0;x<CountFiles;x++)
            {
                HGPEntry entry = new HGPEntry();
                entry.Filename = ReadUnicodeString(data);
                data.Read(buffer, 0, 4); entry.Offset = BitConverter.ToUInt32(buffer, 0);
                data.Read(buffer, 0, 4); entry.ZSize = BitConverter.ToUInt32(buffer, 0);
                entry.Zipped = (byte)data.ReadByte();

                Entries.Add(entry);
            }


            //data.Read(buffer, 0, 4); Files = BitConverter.ToUInt32(buffer, 0);
            //data.Read(buffer, 0, 4); FirstOffset = BitConverter.ToUInt32(buffer, 0);
            //data.Read(buffer, 0, 4); Version2 = BitConverter.ToUInt32(buffer, 0);
            //data.Read(buffer, 0, 4); NTABLESIZE = BitConverter.ToUInt32(buffer, 0);
            //data.Read(buffer, 0, 4); NTABLEZSIZE = BitConverter.ToUInt32(buffer, 0);
            //NTABLEOFFSET = data.Position;

            //byte[] headerData;// = new byte[NTABLESIZE];
            //byte[] headerDataComp = new byte[NTABLEZSIZE];
            //data.Read(headerDataComp, 0, (int)NTABLEZSIZE);
            //headerData = Decompress(headerDataComp);
            //headerDataComp = null;

            //Entries = new List<HpkEntry>();
            //using (MemoryStream headData = new MemoryStream(headerData))
            //{
            //    for (int i = 0; i < Files; i++)
            //    {
            //        HpkEntry entry = new HpkEntry();

            //        StringBuilder filename = new StringBuilder();
            //        byte read = 0x00;
            //        do
            //        {
            //            read = (byte)headData.ReadByte();
            //            if (read != 0x00) filename.Append((char)read);
            //        } while (read != 0x00);
            //        entry.Filename = filename.ToString();

            //        data.Read(buffer, 0, 4); entry.ZSize = BitConverter.ToUInt32(buffer, 0);
            //        data.Read(buffer, 0, 4); entry.Offset = BitConverter.ToUInt32(buffer, 0);
            //        data.Read(buffer, 0, 4); entry.Size = BitConverter.ToUInt32(buffer, 0);
            //        data.Read(buffer, 0, 4); entry.CRC = BitConverter.ToUInt32(buffer, 0);
            //        data.Seek(0x04, SeekOrigin.Current);//NULLs

            //        Entries.Add(entry);
            //    }
            //}

            //for (int i = 0; i < Files; i++)
            //{
                

            //    StringBuilder filename = new StringBuilder();
            //    byte read = 0x00;
            //    do
            //    {
            //        read = (byte)data.ReadByte();
            //        if (read != 0x00) filename.Append((char)read);
            //    } while (read != 0x00);

            //    /*data.Read(buffer, 0, 4);
            //    entry.Magic = new char[4];
            //    for (int i = 0; i < 4; i++)
            //        entry.Magic[i] = (char)buffer[i];
            //    data.Read(buffer, 0, 4);
            //    entry.Unknown1 = BitConverter.ToUInt32(buffer, 0);
            //    data.Read(buffer, 0, 4);
            //    entry.Size = BitConverter.ToUInt32(buffer, 0);
            //    int nameLength = data.ReadByte();
            //    nameLength = (int)(Math.Ceiling((nameLength + 1) / 4.0f) * 4) - 1; // alignment and a 1 offset due to the length byte being part of it
            //    byte[] tmpFilename = new byte[nameLength];
            //    data.Read(tmpFilename, 0, (int)nameLength);
            //    entry.Filename = new string(Encoding.ASCII.GetChars(tmpFilename));
            //    entry.Offset = data.Position;
            //    data.Seek((int)(Math.Ceiling(entry.Size / 4.0f) * 4), SeekOrigin.Current); // skip past the data
            //    Entries.Add(entry);*/
            //}

            //dataOffset = data.Position;
        }

        public byte[] GetFile(int index)
        {
            if (index >= Entries.Count) throw new IndexOutOfRangeException("Index not in file table");

            HGPEntry entry = Entries[index];

            data.Position = entry.Offset;
            byte[] rawFile = new byte[entry.ZSize];

            if (entry.Zipped == 0)
            {
                data.Read(rawFile, 0, (int)entry.ZSize);
            }
            else
            {
                byte[] buffer = new byte[4];
                data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);

                data.Read(rawFile, 0, (int)entry.ZSize);
                rawFile = Decompress(rawFile);
            }

            return rawFile;
        }

        static byte[] Decompress(byte[] data)
        {
            //try
            //{
            using (var compressedStream = new MemoryStream(data))
            //using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var zipStream = new ZlibStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
            /*}catch(ZlibException ex)
            {
                if (ex.Message.StartsWith(@"Bad state (unknown compression method (0x")
                 && ex.Message.EndsWith(@"))"))
                {
                    using (var compressedStream = new MemoryStream(data))
                    using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                    using (var resultStream = new MemoryStream())
                    {
                        zipStream.CopyTo(resultStream);
                        return resultStream.ToArray();
                    }
                }
                else
                {
                    throw ex;
                }
            }*/
        }

        private string ReadString(Stream data)
        {
            if (data.Position < data.Length)
            {
                StringBuilder filename = new StringBuilder();
                byte read = 0x00;
                do
                {
                    read = (byte)data.ReadByte();
                    if (read != 0x00) filename.Append((char)read);
                } while (read != 0x00);
                return filename.ToString();
            }
            else
            {
                return null;
            }
        }

        private string ReadUnicodeString(Stream data)
        {
            if (data.Position < data.Length)
            {
                StringBuilder filename = new StringBuilder();
                byte[] read = new byte[2] { 0x00, 0x00 };
                do
                {
                    data.Read(read,0,2);
                    if (read[0] != 0x00 || read[1] != 0x00) filename.Append(Encoding.Unicode.GetString(read));
                } while (read[0] != 0x00 || read[1] != 0x00);
                return filename.ToString();
            }
            else
            {
                return null;
            }
        }
    }
}
