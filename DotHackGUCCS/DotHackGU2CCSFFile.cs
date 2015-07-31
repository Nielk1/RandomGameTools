using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DotHackGUCCS
{
    public class DotHackGU2CCSFFile
    {
        public List<CCSBlock> BlockChain { get; private set; }
        public CCSFileNamesBlock FileNamesBlock { get { return BlockChain.Where(dr => dr.BlockType == CCSBlockType.FileNames).Cast<CCSFileNamesBlock>().FirstOrDefault(); } }

        public DotHackGU2CCSFFile(Stream data)
        {
            BlockChain = new List<CCSBlock>();

            byte[] buffer = new byte[4];
            try
            {
                while (data.Read(buffer, 0, 4) > 0)
                {
                    //Console.WriteLine("[{0,8:X8}]", data.Position - 4);

                    UInt32 BlockID = BitConverter.ToUInt32(buffer, 0);

                    CCSBlock newBlock = CCSBlock.MakeBlock(BlockID, data);
                    BlockChain.Add(newBlock);

                    //Console.ReadKey(true);
                }
            }
            catch { }
        }
    }

    public enum CCSBlockType : uint
    {
        FileHeader = 0xCCCC0001,
        FileNames  = 0xCCCC0002,
        CCCC0003   = 0xCCCC0003,
        CCCC0005   = 0xCCCC0005,
        Object   = 0xCCCC0100,
        CCCC0101   = 0xCCCC0101,
        CCCC0102   = 0xCCCC0102,
        Material   = 0xCCCC0200,
        CCCC0201   = 0xCCCC0201,
        CCCC0202   = 0xCCCC0202,
        Texture    = 0xCCCC0300,
        Pallet     = 0xCCCC0400,
        CCCC0500   = 0xCCCC0500,
        CCCC0502   = 0xCCCC0502,
        CCCC0503   = 0xCCCC0503,
        CCCC0600   = 0xCCCC0600,
        CCCC0602   = 0xCCCC0602,
        CCCC0603   = 0xCCCC0603,
        CCCC0604   = 0xCCCC0604,
        CCCC0606   = 0xCCCC0606,
        CCCC0608   = 0xCCCC0608,
        CCCC0609   = 0xCCCC0609,
        Animation   = 0xCCCC0700,
        Mesh       = 0xCCCC0800,
        Composit   = 0xCCCC0900,
        Hierarchy  = 0xCCCC0A00,
        CCCC0B00   = 0xCCCC0B00,
        CCCC0C00   = 0xCCCC0C00,
        CCCC0D80   = 0xCCCC0D80,
        CCCC0D90   = 0xCCCC0D90,
        CCCC0E00   = 0xCCCC0E00,
        CCCC1300   = 0xCCCC1300,
        CCCC1400   = 0xCCCC1400,
        CCCC1700   = 0xCCCC1700,
        CCCC1900   = 0xCCCC1900,
        CCCC1901   = 0xCCCC1901,
        CCCC1902   = 0xCCCC1902,
        CCCC1A00   = 0xCCCC1A00,
        CCCC1B00   = 0xCCCC1B00,
        CCCC1D00   = 0xCCCC1D00,
        CCCC1F00   = 0xCCCC1F00,
        CCCC2000   = 0xCCCC2000,
        CCCC2300   = 0xCCCC2300,
        CCCC2400   = 0xCCCC2400,
        CCCCFF01   = 0xCCCCFF01,
    }

    public abstract class CCSBlock
    {
        public abstract CCSBlockType BlockType { get; }
        public abstract long BlockSize { get; }

        public List<CCSBlock> BlockChain { get; protected set; }

        public override string ToString()
        {
            return BlockType.ToString();
        }

        public virtual string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return string.Empty;
        }

        public static CCSBlock MakeBlock(UInt32 BlockID, Stream data)
        {
            if (BlockID == (UInt32)CCSBlockType.FileHeader)
            {
                return new CCSFileHeaderBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.FileNames)
            {
                return new CCSFileNamesBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0003)
            {
                return new CCSCCCC0003Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0005)
            {
                return new CCSCCCC0005Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.Object)
            {
                return new CCSObjectBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0101)
            {
                return new CCSCCCC0101Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0102)
            {
                return new CCSCCCC0102Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.Composit)
            {
                return new CCSCompositBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.Hierarchy)
            {
                return new CCSHierarchyBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0500)
            {
                return new CCSCCCC0500Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0600)
            {
                return new CCSCCCC0600Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0603)
            {
                return new CCSCCCC0603Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0604)
            {
                return new CCSCCCC0604Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0602)
            {
                return new CCSCCCC0602Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0606)
            {
                return new CCSCCCC0606Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0608)
            {
                return new CCSCCCC0608Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0609)
            {
                return new CCSCCCC0609Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0502)
            {
                return new CCSCCCC0502Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0503)
            {
                return new CCSCCCC0503Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0B00)
            {
                return new CCSCCCC0B00Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0C00)
            {
                return new CCSCCCC0C00Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0D80)
            {
                return new CCSCCCC0D80Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0D90)
            {
                return new CCSCCCC0D90Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0E00)
            {
                return new CCSCCCC0E00Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1900)
            {
                return new CCSCCCC1900Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1300)
            {
                return new CCSCCCC1300Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1400)
            {
                return new CCSCCCC1400Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1700)
            {
                return new CCSCCCC1700Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1A00)
            {
                return new CCSCCCC1A00Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1B00)
            {
                return new CCSCCCC1B00Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1D00)
            {
                return new CCSCCCC1D00Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1F00)
            {
                return new CCSCCCC1F00Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1901)
            {
                return new CCSCCCC1901Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC1902)
            {
                return new CCSCCCC1902Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC2000)
            {
                return new CCSCCCC2000Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.Animation)
            {
                return new CCSAnimationBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.Mesh)
            {
                return new CCSMeshBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0201)
            {
                return new CCSCCCC0201Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC0202)
            {
                return new CCSCCCC0202Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.Pallet)
            {
                return new CCSPalletBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.Material)
            {
                return new CCSMaterialBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.Texture)
            {
                return new CCSTextureBlock(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC2300)
            {
                return new CCSCCCC2300Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCC2400)
            {
                return new CCSCCCC2400Block(data);
            }
            else if (BlockID == (UInt32)CCSBlockType.CCCCFF01)
            {
                return new CCSCCCCFF01Block(data);
            }
            else
            {
                if ((BlockID & 0xCCCC0000) == 0xCCCC0000)
                {
                    Console.WriteLine("[{0,8:X8}]", data.Position - 4);
                    Console.WriteLine("{0,8:X8} Block // No Parser", BlockID);

                    byte[] buffer = new byte[4];

                    data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                    Console.WriteLine("Size: {0,8:X8}", Size);
                    long nextBlock = data.Position + (Size * 4);

                    data.Seek(Size * 4, SeekOrigin.Current);

                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Unknown Block Type: {0,8:X8}", BlockID);
                    throw new Exception(string.Format("Unknown Block Type {0,8:X8}", BlockID));
                }
            }
            return null;
        }
    }

    public class CCSFileHeaderBlock : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.FileHeader; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        private string CCSFmarker;

        public string Filename { get; private set; }

        public UInt32 Unknown1 { get; private set; }
        public UInt32 Unknown2 { get; private set; }
        public UInt32 Unknown3 { get; private set; }
        public UInt32 Unknown4 { get; private set; }

        public CCSFileHeaderBlock(Stream data)
        {
            byte[] buffer = new byte[32];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            long nextBlock = data.Position + (_BlockSize * 4);
            data.Read(buffer, 0, 4); CCSFmarker = ASCIIEncoding.ASCII.GetString(buffer, 0, 4);
            data.Read(buffer, 0, 32); Filename = ASCIIEncoding.ASCII.GetString(buffer).TrimEnd('\0');
            data.Read(buffer, 0, 4); Unknown1 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown2 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown3 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown4 = BitConverter.ToUInt32(buffer, 0);
        }
    }

    public class CCSFileNamesBlock : CCSBlock
    {
        public struct BaseFile
        {
            public string Name;
            public char Prefix;

            public override string ToString()
            {
                return string.Format("{0,2:X2}\t{1}", (int)Prefix, Name);
            }
        }

        public struct BakedFile
        {
            public string Name;
            public UInt16 Value;

            public override string ToString()
            {
                return string.Format("{0,2:X2}\t{1}", Value, Name);
            }
        }

        public override CCSBlockType BlockType { get { return CCSBlockType.FileNames; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public BaseFile[] BaseFiles { get; private set; }
        public BakedFile[] BakedFiles { get; private set; }

        //public List<byte> ExtraData;

        public CCSFileNamesBlock(Stream data)
        {
            //ExtraData = new List<byte>();

            byte[] buffer = new byte[32];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //long nextBlock = data.Position + (_BlockSize * 4);// +8;

            data.Read(buffer, 0, 4); UInt32 Lines1 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); UInt32 Lines2 = BitConverter.ToUInt32(buffer, 0);

            BaseFiles = new BaseFile[Lines1];
            for (int x = 0; x < Lines1; x++)
            {
                data.Read(buffer, 0, 32); string Filename = ASCIIEncoding.ASCII.GetString(buffer, 1, 31).TrimEnd('\0');

                BaseFiles[x] = new BaseFile() { Name = Filename, Prefix = (char)buffer[0] };
            }

            BakedFiles = new BakedFile[Lines2];
            for (int x = 0; x < Lines2; x++)
            {
                data.Read(buffer, 0, 30); string Filename = ASCIIEncoding.ASCII.GetString(buffer, 0, 30).TrimEnd('\0');
                data.Read(buffer, 0, 2); UInt16 BaseFileIndex = BitConverter.ToUInt16(buffer, 0);

                BakedFiles[x] = new BakedFile() { Name = Filename, Value = BaseFileIndex };
            }

            //while ((data.Position < _BlockSize) && (data.Read(buffer, 0, 1) > 0))
            //{
            //    ExtraData.Add(buffer[0]);
            //}

            //data.Seek(nextBlock, SeekOrigin.Begin);

            //Console.WriteLine();

            long locFallback = data.Position;

            data.Read(buffer, 0, 4); UInt32 ANOMALY1 = BitConverter.ToUInt32(buffer, 0);
            if (ANOMALY1 == 0x00000003)
            {
                Console.WriteLine("[{0,8:X8}]", data.Position - 4);

                data.Read(buffer, 0, 4); UInt32 ANOMALY2 = BitConverter.ToUInt32(buffer, 0);
                Console.WriteLine("ANOMALY: {0,8:X8} {1,8:X8}", ANOMALY1, ANOMALY2);
                Console.WriteLine();
                //Console.WriteLine("{0} to {1}", locFallback, data.Position);
                //Console.WriteLine();
            }
            else
            {
                //data.Seek(nextBlock, SeekOrigin.Begin);
                data.Seek(locFallback, SeekOrigin.Begin);
            }
        }
    }
    
    public class CCSCCCC0502Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0502; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0502Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0503Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0503; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0503Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0500Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0500; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0500Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0600Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0600; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0600Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0603Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0603; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0603Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0604Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0604; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0604Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0602Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0602; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0602Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0606Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0606; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0606Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0608Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0608; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0608Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0609Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0609; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0609Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0201Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0201; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0201Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0202Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0202; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public List<byte> ExtraData;

        public CCSCCCC0202Block(Stream data)
        {
            ExtraData = new List<byte>();

            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            //while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            //{
            //    UInt32 raw = BitConverter.ToUInt32(buffer, 0);
            //    //Console.WriteLine("\t{0,8:X8}", BlockID);
            //}

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 1) > 0))
            {
                ExtraData.Add(buffer[0]);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0003Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0003; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public List<byte> ExtraData;

        public CCSCCCC0003Block(Stream data)
        {
            ExtraData = new List<byte>();

            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCC0003 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            //while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            //{
            //    UInt32 raw = BitConverter.ToUInt32(buffer, 0);
            //    Console.WriteLine("\t{0,8:X8}", raw);
            //}

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 1) > 0))
            {
                ExtraData.Add(buffer[0]);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0005Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0005; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public List<byte> ExtraData;

        public CCSCCCC0005Block(Stream data)
        {
            ExtraData = new List<byte>();

            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCC0005 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            //while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            //{
            //    UInt32 raw = BitConverter.ToUInt32(buffer, 0);
            //    //Console.WriteLine("\t{0,8:X8}", raw);
            //}

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 1) > 0))
            {
                ExtraData.Add(buffer[0]);
            }

            //Console.WriteLine();
        }
    }
    
    public class CCSCCCC2400Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC2400; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC2400Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC2300Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC2300; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC2300Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSObjectBlock : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.Object; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 OBJBakedFileIndex;
        public UInt32 OBJ2BakedFileIndex;
        public UInt32 MDLBakedFileIndex;
        public List<UInt32> RawData = new List<UInt32>();

        public CCSObjectBlock(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            data.Read(buffer, 0, 4); OBJBakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); OBJ2BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); MDLBakedFileIndex = BitConverter.ToUInt32(buffer, 0);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
                RawData.Add(raw);
            }

            //Console.WriteLine();
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[OBJBakedFileIndex].Name;
        }
    }
    
    public class CCSCompositBlock : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.Composit; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 BakedFileIndex;
        public UInt32 CountSubnodes;
        public List<UInt32> SubnodeBakedFileIndexs;
        public List<Matrix> SubnodeDataItems;

        public struct Matrix
        {
            public float tx;
            public float ty;
            public float tz;
            public float rx;
            public float ry;
            public float rz;
            public float sx;
            public float sy;
            public float sz;
        }

        public List<byte> ExtraData;

        public CCSCompositBlock(Stream data)
        {
            ExtraData = new List<byte>();

            SubnodeBakedFileIndexs = new List<UInt32>();
            SubnodeDataItems = new List<Matrix>();

            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            data.Read(buffer, 0, 4); BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); CountSubnodes = BitConverter.ToUInt32(buffer, 0);

            for (int x = 0; x < CountSubnodes; x++)
            {
                data.Read(buffer, 0, 4); UInt32 SubnodeBakedFileIndex = BitConverter.ToUInt32(buffer, 0);
                SubnodeBakedFileIndexs.Add(SubnodeBakedFileIndex);
            }

            for (int x = 0; x < CountSubnodes; x++)
            {
                data.Read(buffer, 0, 4); float tx = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float ty = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float tz = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float rx = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float ry = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float rz = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float sx = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float sy = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float sz = BitConverter.ToSingle(buffer, 0);

                SubnodeDataItems.Add(new Matrix() {
                    tx = tx,
                    ty = ty,
                    tz = tz,
                    rx = rx,
                    ry = ry,
                    rz = rz,
                    sx = sx,
                    sy = sy,
                    sz = sz,
                });
            }

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 1) > 0))
            {
                ExtraData.Add(buffer[0]);
            }
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[BakedFileIndex].Name;
        }
    }

    public class CCSCCCC0101Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0101; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0101Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0102Block : CCSBlock
    {
        const byte b00000001 = 1;
        const byte b00000010 = 1 << 1;
        const byte b00000100 = 1 << 2;
        const byte b00001000 = 1 << 3;
        const byte b00010000 = 1 << 4;
        const byte b00100000 = 1 << 5;
        const byte b01000000 = 1 << 6;
        const byte b10000000 = 1 << 7;

        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0102; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 SelfIndex;
        public UInt32 Marker;

        public struct DataSet0Item
        {
            public float Unknown1;
            public float Unknown2;
            public float Unknown3;

            public override string ToString()
            {
                return string.Format("{0}\t{1}\t{2}", Unknown1.ToString("0.0"), Unknown2.ToString("0.0"), Unknown3.ToString("0.0"));
            }
        }

        public struct DataSet1Item
        {
            public UInt32 Unknown1;
            public float Unknown2;
            public float Unknown3;
            public float Unknown4;

            public override string ToString()
            {
                return string.Format("{0,8:X8}\t{1}\t{2}\t{3}", Unknown1, Unknown2.ToString("0.0"), Unknown3.ToString("0.0"), Unknown4.ToString("0.0"));
            }
        }

        public struct DataSet3Item
        {
            public float Unknown1;
            public float Unknown2;
            public float Unknown3;

            public override string ToString()
            {
                return string.Format("{0}\t{1}\t{2}", Unknown1.ToString("0.0"), Unknown2.ToString("0.0"), Unknown3.ToString("0.0"));
            }
        }

        public struct DataSet4Item
        {
            public UInt32 Unknown1;
            public float Unknown2;
            public float Unknown3;
            public float Unknown4;

            public override string ToString()
            {
                return string.Format("{0,8:X8}\t{1}\t{2}\t{3}", Unknown1, Unknown2.ToString("0.0"), Unknown3.ToString("0.0"), Unknown4.ToString("0.0"));
            }
        }

        public struct DataSet5Item
        {
            public UInt32 Unknown1;
            public UInt32 Unknown2;
            public UInt32 Unknown3;
            public UInt32 Unknown4;
            public UInt32 Unknown5;

            public override string ToString()
            {
                return string.Format("{0,8:X8}\t{1,8:X8}\t{2,8:X8}\t{3,8:X8}\t{4,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4, Unknown5);
            }
        }

        public struct DataSet6Item
        {
            public float Unknown1;
            public float Unknown2;
            public float Unknown3;

            public override string ToString()
            {
                return string.Format("{0}\t{1}\t{2}", Unknown1.ToString("0.0"), Unknown2.ToString("0.0"), Unknown3.ToString("0.0"));
            }
        }

        public struct DataSet7Item
        {
            public UInt32 Unknown1;
            public float Unknown2;
            public float Unknown3;
            public float Unknown4;

            public override string ToString()
            {
                return string.Format("{0,8:X8}\t{1}\t{2}\t{3}", Unknown1, Unknown2.ToString("0.0"), Unknown3.ToString("0.0"), Unknown4.ToString("0.0"));
            }
        }

        public struct DataSetAItem
        {
            public UInt32 Unknown1;
            public float Unknown2;

            public override string ToString()
            {
                return string.Format("{0,8:X8}\t{1}", Unknown1, Unknown2.ToString("0.0"));
            }
        }

        public DataSet0Item? DataSet0;
        public List<DataSet1Item> DataSet1;
        //DataSet2
        public DataSet3Item? DataSet3;
        public List<DataSet4Item> DataSet4;
        public List<DataSet5Item> DataSet5;
        public DataSet6Item? DataSet6;
        public List<DataSet7Item> DataSet7;
        //DataSet8
        public UInt32? DataSet9;
        public List<DataSetAItem> DataSetA;
        //DataSetB
        //DataSetC
        //DataSetD
        //DataSetE
        //DataSetF

        public CCSCCCC0102Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCC0102 Block // complex data, bitwise toggles");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size: {0} int32s", _BlockSize);

            data.Read(buffer, 0, 4); SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            data.Read(buffer, 0, 4); Marker = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Marker: {0,8:X8}", Marker);

            byte ToggleInputs1 = buffer[0];
            byte ToggleInputs2 = buffer[1];

            if ((ToggleInputs1 & b00000001) == b00000001)
            {
                //for (int x = 0; x < 3; x++)
                //{
                //    data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                //    Console.WriteLine("Unknown0: {0,8:X8}", Unknown);
                //}
                data.Read(buffer, 0, 4); float Unknown1 = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float Unknown2 = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float Unknown3 = BitConverter.ToSingle(buffer, 0);
                DataSet0 = new DataSet0Item()
                {
                    Unknown1 = Unknown1,
                    Unknown2 = Unknown2,
                    Unknown3 = Unknown3
                };
            }
            if ((ToggleInputs1 & b00000010) == b00000010)
            {
                DataSet1 = new List<DataSet1Item>();

                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Count1: {0}", Counter);
                for (int x = 0; x < Counter; x++)
                {
                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown2 = BitConverter.ToSingle(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown3 = BitConverter.ToSingle(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown4 = BitConverter.ToSingle(buffer, 0);
                    //Console.WriteLine("Unknown1: {0,8:X8} {1,8:X8} {2,8:X8} {3,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4);
                    DataSet1.Add(new DataSet1Item() {
                        Unknown1 = Unknown1,
                        Unknown2 = Unknown2,
                        Unknown3 = Unknown3,
                        Unknown4 = Unknown4
                    });
                }
            }
            if ((ToggleInputs1 & b00000100) == b00000100)
            {
                throw new Exception(string.Format("Unknown2 CCCC0102 Block Type {0,8:X8}", Marker));
            }
            if ((ToggleInputs1 & b00001000) == b00001000)
            {
                //for (int x = 0; x < 3; x++)
                //{
                //    data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                //    Console.WriteLine("Unknown3: {0,8:X8}", Unknown);
                //}
                data.Read(buffer, 0, 4); float Unknown1 = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float Unknown2 = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float Unknown3 = BitConverter.ToSingle(buffer, 0);
                DataSet3 = new DataSet3Item()
                {
                    Unknown1 = Unknown1,
                    Unknown2 = Unknown2,
                    Unknown3 = Unknown3
                };
            }
            if ((ToggleInputs1 & b00010000) == b00010000)
            {
                DataSet4 = new List<DataSet4Item>();

                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Count4: {0}", Counter);
                for (int x = 0; x < Counter; x++)
                {
                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown2 = BitConverter.ToSingle(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown3 = BitConverter.ToSingle(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown4 = BitConverter.ToSingle(buffer, 0);
                    //Console.WriteLine("Unknown4: {0,8:X8} {1,8:X8} {2,8:X8} {3,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4);
                    DataSet4.Add(new DataSet4Item()
                    {
                        Unknown1 = Unknown1,
                        Unknown2 = Unknown2,
                        Unknown3 = Unknown3,
                        Unknown4 = Unknown4
                    });
                }
            }
            if ((ToggleInputs1 & b00100000) == b00100000)
            {
                DataSet5 = new List<DataSet5Item>();

                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Count5: {0}", Counter);
                for (int x = 0; x < Counter; x++)
                {
                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                    data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                    data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                    data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                    data.Read(buffer, 0, 4); UInt32 Unknown5 = BitConverter.ToUInt32(buffer, 0);
                    //Console.WriteLine("Unknown5: {0,8:X8} {1,8:X8} {2,8:X8} {3,8:X8} {4,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4, Unknown5);
                    DataSet5.Add(new DataSet5Item()
                    {
                        Unknown1 = Unknown1,
                        Unknown2 = Unknown2,
                        Unknown3 = Unknown3,
                        Unknown4 = Unknown4,
                        Unknown5 = Unknown5
                    });
                }
            }
            if ((ToggleInputs1 & b01000000) == b01000000)
            {
                //for (int x = 0; x < 3; x++)
                //{
                //    data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                //    Console.WriteLine("Unknown6: {0,8:X8}", Unknown);
                //}
                data.Read(buffer, 0, 4); float Unknown1 = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float Unknown2 = BitConverter.ToSingle(buffer, 0);
                data.Read(buffer, 0, 4); float Unknown3 = BitConverter.ToSingle(buffer, 0);
                DataSet6 = new DataSet6Item()
                {
                    Unknown1 = Unknown1,
                    Unknown2 = Unknown2,
                    Unknown3 = Unknown3
                };
            }
            if ((ToggleInputs1 & b10000000) == b10000000)
            {
                DataSet7 = new List<DataSet7Item>();

                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Count7: {0}", Counter);
                for (int x = 0; x < Counter; x++)
                {
                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown2 = BitConverter.ToSingle(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown3 = BitConverter.ToSingle(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown4 = BitConverter.ToSingle(buffer, 0);
                    //Console.WriteLine("Unknown7: {0,8:X8} {1,8:X8} {2,8:X8} {3,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4);
                    DataSet7.Add(new DataSet7Item()
                    {
                        Unknown1 = Unknown1,
                        Unknown2 = Unknown2,
                        Unknown3 = Unknown3,
                        Unknown4 = Unknown4
                    });
                }
            }

            if ((ToggleInputs2 & b00000001) == b00000001)
            {
                throw new Exception(string.Format("Unknown8 CCCC0102 Block Type {0,8:X8}", Marker));
            }
            if ((ToggleInputs2 & b00000010) == b00000010)
            {
                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Unknown9: {0}", Counter);
                DataSet9 = Counter;
            }
            if ((ToggleInputs2 & b00000100) == b00000100)
            {
                DataSetA = new List<DataSetAItem>();

                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("CountA: {0}", Counter);
                for (int x = 0; x < Counter; x++)
                {
                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                    data.Read(buffer, 0, 4); float Unknown2 = BitConverter.ToSingle(buffer, 0);
                    //Console.WriteLine("UnknownA: {0,8:X8} {1,8:X8}", Unknown1, Unknown2);
                    DataSetA.Add(new DataSetAItem()
                    {
                        Unknown1 = Unknown1,
                        Unknown2 = Unknown2
                    });
                }
            }
            if ((ToggleInputs2 & b00001000) == b00001000)
            {
                throw new Exception(string.Format("UnknownB CCCC0102 Block Type {0,8:X8}", Marker));
            }
            if ((ToggleInputs2 & b00010000) == b00010000)
            {
                throw new Exception(string.Format("UnknownC CCCC0102 Block Type {0,8:X8}", Marker));
            }
            if ((ToggleInputs2 & b00100000) == b00100000)
            {
                throw new Exception(string.Format("UnknownD CCCC0102 Block Type {0,8:X8}", Marker));
            }
            if ((ToggleInputs2 & b01000000) == b01000000)
            {
                throw new Exception(string.Format("UnknownE CCCC0102 Block Type {0,8:X8}", Marker));
            }
            if ((ToggleInputs2 & b10000000) == b10000000)
            {
                throw new Exception(string.Format("UnknownF CCCC0102 Block Type {0,8:X8}", Marker));
            }

            //Console.WriteLine();
        }
    }

    public class CCSMaterialBlock : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.Material; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 BakedFileIndex { get; private set; }
        public UInt32 TextureBakedFileIndex { get; private set; }

        public float Unknown1 { get; private set; }
        public UInt32 Unknown2 { get; private set; }
        public UInt32 Unknown3 { get; private set; }

        public CCSMaterialBlock(Stream data)
        {
            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); TextureBakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown1 = BitConverter.ToSingle(buffer, 0);
            data.Read(buffer, 0, 4); Unknown2 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown3 = BitConverter.ToUInt32(buffer, 0);

            //ObjModel model = new ObjModel(nodeNames[SelfIndex]);
            //model.mats.Add(new ObjModel.Material()
            //{
            //    Name = nodeNames[SelfIndex],
            //    map_Kd = nodeNames[TextureIndex] + ".png"
            //});
            //
            //var invalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());
            //
            //string cleanedFilename = new string(nodeNames[SelfIndex]
            //.Where(x => !invalidChars.Contains(x))
            //.ToArray());
            //
            //if (cleanedFilename.Length == 0) cleanedFilename += "_";
            //
            ////if (!File.Exists(cleanedFilename + @".mtl"))
            //model.MaterialToFile(cleanedFilename + @".mtl");
            //
            //Console.WriteLine();
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[BakedFileIndex].Name;
        }
    }

    public class CCSTextureBlock : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.Texture; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 BakedFileIndex { get; private set; }
        public UInt32 PalletFileIndex { get; private set; }

        public UInt32 Unknown1 { get; private set; } // Always 0x 00000000?
        public UInt32 Unknown2 { get; private set; }

        public int Height { get; private set; }
        public int Width { get; private set; }

        private UInt16 CDCD_Marker; // Always CDCD?

        public UInt32 Unknown3 { get; private set; }

        private byte[] RawImageData;

        public CCSTextureBlock(Stream data)
        {
            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); PalletFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown1 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown2 = BitConverter.ToUInt32(buffer, 0);
            int width = data.ReadByte();
            int height = data.ReadByte();
            Width = (int)Math.Pow(2, width);
            Height = (int)Math.Pow(2, height);

            data.Read(buffer, 0, 2); CDCD_Marker = BitConverter.ToUInt16(buffer, 0);
            data.Read(buffer, 0, 4); Unknown3 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); UInt32 PixelCount = BitConverter.ToUInt32(buffer, 0);

            uint rawPixelCount = PixelCount * 4;

            RawImageData = new byte[rawPixelCount];

            for (int x = 0; x < rawPixelCount; x++)
            {
                RawImageData[x] = (byte)data.ReadByte();
            }
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[BakedFileIndex].Name;
        }

        public Image GetImage(Color[] pallet = null)
        {
            if (RawImageData.Length > 0)
            {
                Color[] palletUse;

                int ppb = Height * Width / RawImageData.Length;

                if (pallet != null)
                {
                    if (ppb == 1 && pallet.Length != 256) return null;
                    if (ppb == 2 && pallet.Length != 16) return null;
                    palletUse = pallet;
                }
                else
                {
                    if (ppb == 1)
                    {
                        palletUse = new Color[256];
                        for (int x = 0; x < 256; x++) palletUse[x] = Color.FromArgb(x, x, x);
                    }
                    else if (ppb == 2)
                    {
                        palletUse = new Color[16];
                        for (int x = 0; x < 16; x++) palletUse[x] = Color.FromArgb((x << 4) + x, (x << 4) + x, (x << 4) + x);
                    }
                    else
                    {
                        return null;
                    }
                }

                Bitmap output = new Bitmap(Width, Height);
                for (int x = 0; x < RawImageData.Length; x++)
                {
                    byte col = RawImageData[x];

                    if (ppb == 2)
                    {
                        byte P1 = (byte)(col & 0x0f);
                        byte P2 = (byte)((col & 0xf0) >> 4);

                        output.SetPixel((x * 2) % Width, Height - 1 - ((x * 2) / Width), palletUse[P1]);
                        output.SetPixel(((x * 2) + 1) % Width, Height - 1 - (((x * 2) + 1) / Width), palletUse[P2]);
                    }
                    else
                    {
                        output.SetPixel(x % Width, Height - 1 - (x / Width), palletUse[col]);
                    }
                }

                return output;
            }
            return null;
        }
    }

    public class CCSPalletBlock : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.Pallet; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 BakedFileIndex { get; private set; }

        public UInt32 Unknown1 { get; private set; } // Always 0x 00000000?
        public UInt32 Unknown2 { get; private set; } // Always 0x 70000001?
        public UInt32 Unknown3 { get; private set; }

        public Color[] Colors { get; private set; }

        public CCSPalletBlock(Stream data)
        {
            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            long nextBlock = data.Position + (_BlockSize * 4);
            data.Read(buffer, 0, 4); BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown1 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown2 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown3 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); UInt32 ColorCount = BitConverter.ToUInt32(buffer, 0);

            Colors = new Color[ColorCount];
            for (int x = 0; x < ColorCount; x++)
            {
                data.Read(buffer, 0, 4);
                Colors[x] = Color.FromArgb((buffer[3] * 255 / 128), buffer[0], buffer[1], buffer[2]);
            }

            data.Seek(nextBlock, SeekOrigin.Begin);
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[BakedFileIndex].Name;
        }

        public Image GetPalletImage()
        {
            Bitmap image = new Bitmap(Colors.Length, 1);
            for (int x = 0; x < Colors.Length; x++)
            {
                image.SetPixel(x, 0, Colors[x]);
            }
            return image;
        }
    }

    public class CCSAnimationBlock : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.Animation; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 BakedFileIndex { get; private set; }

        public UInt32 Unknown1 { get; private set; } // Always 0x 00000001?
        public UInt32 Unknown2 { get; private set; } // Always 0x 00000000?
        //public UInt32 Unknown3 { get; private set; } // Always 0x 00000000?
        //public UInt32 Unknown4 { get; private set; } // Always 0x 00000000?

        public CCSAnimationBlock(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("[{0,8:X8}]", data.Position);

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            long nextBlock = data.Position + (_BlockSize * 4);// +8;
            data.Read(buffer, 0, 4); BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown1 = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); Unknown2 = BitConverter.ToUInt32(buffer, 0);
            //data.Read(buffer, 0, 4); Unknown3 = BitConverter.ToUInt32(buffer, 0);
            //data.Read(buffer, 0, 4); Unknown4 = BitConverter.ToUInt32(buffer, 0);

            BlockChain = new List<CCSBlock>();

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 BlockID = BitConverter.ToUInt32(buffer, 0);

                CCSBlock newBlock = CCSBlock.MakeBlock(BlockID, data);
                BlockChain.Add(newBlock);

                //Console.ReadKey(true);
            }

            //data.Seek(nextBlock, SeekOrigin.Begin);

            //Console.WriteLine();
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[BakedFileIndex].Name;
        }
    }

    public class CCSMeshBlock : CCSBlock
    {
        public struct Vertex
        {
            public Int16 x;
            public Int16 y;
            public Int16 z;

            public override string ToString()
            {
                return string.Format("{0}\t{1}\t{2}", x, y, z);
            }
        }

        public struct Normal
        {
            public sbyte x;
            public sbyte y;
            public sbyte z;
            public byte w;

            public override string ToString()
            {
                return string.Format("{0}\t{1}\t{2}\t{3}", x, y, z, w);
            }
        }

        public struct RGBA
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;

            public override string ToString()
            {
                return string.Format("{0,2:X2}\t{1,2:X2}\t{2,2:X2}\t{3,2:X2}", r, g, b, a);
            }
        }

        public struct UV
        {
            public Int16 u;
            public Int16 v;

            public override string ToString()
            {
                return string.Format("{0}\t{1}", u, v);
            }
        }

        public class Mesh
        {
            public UInt32 UnknownA1;
            public UInt32 MaterialIndex;
            public UInt32 VertCount;

            public List<Vertex> Verts { get; private set; }
            public List<Normal> Norms { get; private set; }
            public List<RGBA> RGBAs { get; private set; }
            public List<UV> UVs { get; private set; }

            public Mesh()
            {
                Verts = new List<Vertex>();
                Norms = new List<Normal>();
                RGBAs = new List<RGBA>();
                UVs = new List<UV>();
            }
        }

        public class ShadowMesh
        {
            public List<Vertex> Verts { get; private set; }
            public List<UInt32> Faces { get; private set; }
            public uint VertexCount { get; set; }
            public uint FaceCount { get; set; }

            public ShadowMesh()
            {
                Verts = new List<Vertex>();
                Faces = new List<UInt32>();
            }
        }

        public override CCSBlockType BlockType { get { return CCSBlockType.Mesh; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 SelfIndex;

        public UInt32 Unknown1;
        public UInt32 Unknown2A;
        public UInt32 SubmeshCount;
        public UInt32 Unknown3;
        public UInt32 Unknown4;
        public UInt32 Unknown5;
        public float UnknownFloat;

        public List<Mesh> Meshes;
        public List<ShadowMesh> shadowMesh;

        public byte[] RawData;// temporary

        public List<byte> ExtraData;

        public CCSMeshBlock(Stream data)
        {
            Meshes = new List<Mesh>();
            shadowMesh = new List<ShadowMesh>();

            ExtraData = new List<byte>();

            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);

            long startPosition = data.Position;
            long endPosition = data.Position + (_BlockSize * 4);

            data.Read(buffer, 0, 4); SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.Write("Node Index: {0}", SelfIndex);
            //if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
            //Console.WriteLine();

            //try
            {
                data.Read(buffer, 0, 4); Unknown1 = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Unknown1: {0,8:X8} (if 0, end early)", Unknown1);

                //data.Read(buffer, 0, 4); Unknown2 = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Unknown2: {0,8:X8}", Unknown2);
                //int Unknown2C = buffer[2];

                data.Read(buffer, 0, 2); Unknown2A = BitConverter.ToUInt16(buffer, 0);
                data.Read(buffer, 0, 2); SubmeshCount = BitConverter.ToUInt16(buffer, 0);

                data.Read(buffer, 0, 4); Unknown3 = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Unknown3: {0,8:X8}", Unknown3);

                data.Read(buffer, 0, 4); Unknown4 = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("Unknown4: {0,8:X8}", Unknown4);

                data.Read(buffer, 0, 4); Unknown5 = BitConverter.ToUInt32(buffer, 0);
                int Unknown5D = buffer[3];
                //int Unknown5A = data.ReadByte();
                //int Unknown5B = data.ReadByte();
                //int Unknown5C = data.ReadByte();
                //int Unknown5D = data.ReadByte();
                //Console.WriteLine("Unknown5: {0,2:X2} {1,2:X2} {2,2:X2} {3,2:X2}", Unknown5A, Unknown5B, Unknown5C, Unknown5D);

                data.Read(buffer, 0, 4); //UnknownFloatA = BitConverter.ToUInt32(buffer, 0);
                float UnknownFloat = BitConverter.ToSingle(buffer, 0);
                //Console.WriteLine("UnknownFloat: {0,8:X8} {1}", UnknownFloatA, UnknownFloatB.ToString("R", CultureInfo.InvariantCulture));

                long TmpPos = data.Position;

                //if (Unknown1 != 0)
                //try
                if(Unknown2A == 0x0000 || Unknown2A == 0x0001) // normal mesh
                {
                    //if (Unknown5D == 0)
                    //if(Unknown2B == 0x0001 || Unknown2B == 0x0002)
                    for (int SubmeshCounter = 0; SubmeshCounter < SubmeshCount; SubmeshCounter++)
                    {
                        Mesh meshData = new Mesh();

                        data.Read(buffer, 0, 4); meshData.UnknownA1 = BitConverter.ToUInt32(buffer, 0);
                        //Console.WriteLine("UnknownA1: {0,8:X8}", UnknownA1);

                        data.Read(buffer, 0, 4); meshData.MaterialIndex = BitConverter.ToUInt32(buffer, 0);
                        //Console.Write("Material Index: {0}", MaterialIndex);
                        //if (nodeNames != null && nodeNames.Length > MaterialIndex) Console.Write(" \"{0}\"", nodeNames[MaterialIndex]);
                        //Console.WriteLine();

                        //ObjModel model = new ObjModel(nodeNames[SelfIndex]);

                        data.Read(buffer, 0, 4); meshData.VertCount = BitConverter.ToUInt32(buffer, 0);
                        //Console.WriteLine("Count: {0} ({0,8:X8})", Count);

                        for (int x = 0; x < meshData.VertCount; x++)
                        {
                            data.Read(buffer, 0, 2); Int16 h1 = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 h2 = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 h3 = BitConverter.ToInt16(buffer, 0);
                            //Console.WriteLine("XYZ [{0}]: {1} {2} {3}", x, h1, h2, h3);

                            //model.vertices.Add(new ObjModel.Vector3() { x = h1 * 0.1f, y = h2 * 0.1f, z = h3 * 0.1f });
                            meshData.Verts.Add(new Vertex() { x = h1, y = h2, z = h3 });

                            if (data.Position >= data.Length) throw new Exception("Read Too Far");
                        }
                        //data.Seek((4 - 1) / 4 * 4, SeekOrigin.Current); // Align

                        data.Seek(((4 - (data.Position % 4)) % 4), SeekOrigin.Current); // Align



                        UInt32[] normData = new UInt32[meshData.VertCount];
                        for (int x = 0; x < meshData.VertCount; x++)
                        {
                            data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);

                            normData[x] = Unknown;

                            //model.normals.Add(new ObjModel.Vector3()
                            //{
                            //    x = (sbyte)buffer[0] / 64.0f,
                            //    y = (sbyte)buffer[1] / 64.0f,
                            //    z = (sbyte)buffer[2] / 64.0f
                            //});

                            meshData.Norms.Add(new Normal() { x = (sbyte)buffer[0], y = (sbyte)buffer[1], z = (sbyte)buffer[2], w = buffer[3] });

                            //Console.WriteLine("Normal [{0}]: {1} {2} {3} {4}",
                            //    x,
                            //    (sbyte)buffer[0],
                            //    (sbyte)buffer[1],
                            //    (sbyte)buffer[2],
                            //    buffer[3] == 0x01 ? "No Draw, Force Odd" : buffer[3] == 0x02 ? "No Draw, Force Even" : string.Empty);
                        }

                        for (int x = 0; x < meshData.VertCount; x++)
                        {
                            data.Read(buffer, 0, 4);

                            meshData.RGBAs.Add(new RGBA() { r = buffer[0], g = buffer[1], b = buffer[2], a = buffer[3] });

                            //Console.WriteLine("RGBA [{0}]: {1,2:X2} {2,2:X2} {3,2:X2} {4,2:X2}", x, buffer[0], buffer[1], buffer[2], buffer[3]);
                        }

                        for (int x = 0; x < meshData.VertCount; x++)
                        {
                            data.Read(buffer, 0, 2); Int16 U = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 V = BitConverter.ToInt16(buffer, 0);
                            //Console.WriteLine("UV [{0}]: {1} {2}", x, U, V);

                            meshData.UVs.Add(new UV() { u = U, v = V });

                            //model.uv.Add(new ObjModel.Vector3() { x = U / 255.0f, y = V / 255.0f });
                        }

                        Meshes.Add(meshData);

                        {
                            //string material = nodeNames[MaterialIndex];
                            //List<int> verts = new List<int>();
                            //bool winding = false;
                            //for (int v = 0; v < Count; v++)
                            //{
                            //    if ((normData[v] & 0x03000000) == 0)
                            //    {
                            //        if (v > 1)
                            //        {
                            //            if (winding)
                            //            {
                            //                verts.Add(v - 2);
                            //                verts.Add(v - 1);
                            //                verts.Add(v);
                            //            }
                            //            else
                            //            {
                            //                verts.Add(v - 2);
                            //                verts.Add(v);
                            //                verts.Add(v - 1);
                            //            }
                            //        }
                            //        winding = !winding;
                            //    }
                            //    else if ((normData[v] & 0x01000000) > 0)
                            //    {
                            //        winding = true;
                            //    }
                            //    else if ((normData[v] & 0x02000000) > 0)
                            //    {
                            //        winding = false;
                            //    }
                            //}
                            //model.faces.Add(material, verts);
                        }
                    }
                    //else
                    //{
                    //    RawData = new byte[(_BlockSize - 7) * 4];
                    //
                    //    data.Seek(startPosition + (7 * 4), SeekOrigin.Begin);
                    //    data.Read(RawData, 0, RawData.Length);
                    //}


                    //else
                    //{
                    //    RawData = new byte[(_BlockSize - 7) * 4];
                    //
                    //    data.Seek(startPosition + (7 * 4), SeekOrigin.Begin);
                    //    data.Read(RawData, 0, RawData.Length);
                    //}

                    /*else if (Unknown5D == 0x80)
                    {
                        data.Read(buffer, 0, 4); UInt32 Count1 = BitConverter.ToUInt32(buffer, 0);
                        //Console.WriteLine("Count1: {0,8:X8}", Count1);

                        data.Read(buffer, 0, 4); UInt32 Count2 = BitConverter.ToUInt32(buffer, 0);
                        //Console.WriteLine("Count2: {0,8:X8}", Count2);

                        for (int x = 0; x < Count1; x++)
                        {
                            //Console.Write("Data1:");
                            //Console.Write(" {0,2:X2}", data.ReadByte());
                            //Console.Write(" {0,2:X2}", data.ReadByte());
                            //Console.Write(" {0,2:X2}", data.ReadByte());
                            //Console.Write(" {0,2:X2}", data.ReadByte());
                            //Console.Write(" {0,2:X2}", data.ReadByte());
                            //Console.Write(" {0,2:X2}", data.ReadByte());
                            //Console.WriteLine();

                            //byte[] buffer2 = new byte[2];
                            //data.Read(buffer2, 0, 2); Half h1 = Half.FromBinary(buffer2.Reverse().ToArray(), 0);
                            //data.Read(buffer2, 0, 2); Half h2 = Half.FromBinary(buffer2.Reverse().ToArray(), 0);
                            //data.Read(buffer2, 0, 2); Half h3 = Half.FromBinary(buffer2.Reverse().ToArray(), 0);
                            //Console.WriteLine("Data1: {0} {1} {2}", h1, h2, h3);

                            data.Read(buffer, 0, 2); Int16 h1 = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 h2 = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 h3 = BitConverter.ToInt16(buffer, 0);
                            //Console.WriteLine("Data1: {0} {1} {2}", h1, h2, h3);
                        }
                        data.Seek((4 - 1) / 4 * 4, SeekOrigin.Current); // Align

                        for (int x = 0; x < Count2; x++)
                        {
                            data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                            //Console.WriteLine("Data2: {0,8:X8}", Unknown);
                        }
                    }*/
                }
                if (Unknown2A == 0x0008)
                {
                    for (int SubmeshCounter = 0; SubmeshCounter < SubmeshCount; SubmeshCounter++)
                    {
                        ShadowMesh meshData = new ShadowMesh();

                        data.Read(buffer, 0, 4); meshData.VertexCount = BitConverter.ToUInt32(buffer, 0);
                        data.Read(buffer, 0, 4); meshData.FaceCount = BitConverter.ToUInt32(buffer, 0);

                        for (int x = 0; x < meshData.VertexCount; x++)
                        {
                            data.Read(buffer, 0, 2); Int16 h1 = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 h2 = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 h3 = BitConverter.ToInt16(buffer, 0);
                            //Console.WriteLine("XYZ [{0}]: {1} {2} {3}", x, h1, h2, h3);

                            //model.vertices.Add(new ObjModel.Vector3() { x = h1 * 0.1f, y = h2 * 0.1f, z = h3 * 0.1f });
                            meshData.Verts.Add(new Vertex() { x = h1, y = h2, z = h3 });

                            if (data.Position >= data.Length) throw new Exception("Read Too Far");
                        }
                        data.Seek(((4 - (data.Position % 4)) % 4), SeekOrigin.Current); // Align

                        for (int x = 0; x < meshData.FaceCount; x++)
                        {
                            data.Read(buffer, 0, 4); UInt32 h1 = BitConverter.ToUInt32(buffer, 0);
                            //Console.WriteLine("XYZ [{0}]: {1} {2} {3}", x, h1, h2, h3);

                            //model.vertices.Add(new ObjModel.Vector3() { x = h1 * 0.1f, y = h2 * 0.1f, z = h3 * 0.1f });
                            meshData.Faces.Add(h1);

                            if (data.Position >= data.Length) throw new Exception("Read Too Far");
                        }

                        shadowMesh.Add(meshData);
                    }
                }
                if (Unknown2A == 0x0201)
                {
                    for (int SubmeshCounter = 0; SubmeshCounter < SubmeshCount; SubmeshCounter++)
                    {
                        Mesh meshData = new Mesh();

                        data.Read(buffer, 0, 4); meshData.UnknownA1 = BitConverter.ToUInt32(buffer, 0);
                        data.Read(buffer, 0, 4); meshData.MaterialIndex = BitConverter.ToUInt32(buffer, 0);
                        data.Read(buffer, 0, 4); meshData.VertCount = BitConverter.ToUInt32(buffer, 0);

                        for (int x = 0; x < meshData.VertCount; x++)
                        {
                            data.Read(buffer, 0, 2); Int16 h1 = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 h2 = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 h3 = BitConverter.ToInt16(buffer, 0);

                            meshData.Verts.Add(new Vertex() { x = h1, y = h2, z = h3 });

                            if (data.Position >= data.Length) throw new Exception("Read Too Far");
                        }

                        data.Seek(((4 - (data.Position % 4)) % 4), SeekOrigin.Current); // Align

                        UInt32[] normData = new UInt32[meshData.VertCount];
                        for (int x = 0; x < meshData.VertCount; x++)
                        {
                            data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);

                            normData[x] = Unknown;

                            meshData.Norms.Add(new Normal() { x = (sbyte)buffer[0], y = (sbyte)buffer[1], z = (sbyte)buffer[2], w = buffer[3] });
                        }

                        for (int x = 0; x < meshData.VertCount; x++)
                        {
                            //data.Read(buffer, 0, 4);

                            //meshData.RGBAs.Add(new RGBA() { r = buffer[0], g = buffer[1], b = buffer[2], a = buffer[3] });

                            meshData.RGBAs.Add(new RGBA() { r = 255, g = 255, b = 255, a = 128 });
                        }

                        for (int x = 0; x < meshData.VertCount; x++)
                        {
                            data.Read(buffer, 0, 2); Int16 U = BitConverter.ToInt16(buffer, 0);
                            data.Read(buffer, 0, 2); Int16 V = BitConverter.ToInt16(buffer, 0);

                            meshData.UVs.Add(new UV() { u = U, v = V });
                        }

                        Meshes.Add(meshData);
                    }
                }

                //catch(Exception ex) {
                //    data.Seek(TmpPos, SeekOrigin.Begin);
                //}
                //else
                //{
                //    RawData = new byte[(_BlockSize - 7) * 4];
                //
                //    data.Seek(startPosition + (7 * 4), SeekOrigin.Begin);
                //    data.Read(RawData, 0, RawData.Length);
                //}

                //if (data.Position > endPosition) throw new Exception("Overrun");
                //if (data.Position < endPosition) throw new Exception("Underun");
            }

            while ((data.Position < endPosition) && (data.Read(buffer, 0, 1) > 0))
            {
                ExtraData.Add(buffer[0]);
            }

            //catch (Exception ex)
            //{
            //    data.Seek(endPosition, SeekOrigin.Begin);
            //}

            //RawData = new byte[(_BlockSize + 2) * 4];

            //data.Seek(startPosition - 8, SeekOrigin.Begin);
            //data.Read(RawData, 0, (int)((_BlockSize + 2) * 4));

            //Console.WriteLine();
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[SelfIndex].Name;
        }
    }

    public class CCSHierarchyBlock : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.Hierarchy; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 BakedFileIndex { get; private set; }
        public UInt32 ParentBakedFileIndex { get; private set; }
        public UInt32 BakedFileIndex2 { get; private set; }

        public CCSHierarchyBlock(Stream data)
        {
            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //long nextBlock = data.Position + (_BlockSize * 4);// +8;

            data.Read(buffer, 0, 4); BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); ParentBakedFileIndex = BitConverter.ToUInt32(buffer, 0);
            data.Read(buffer, 0, 4); BakedFileIndex2 = BitConverter.ToUInt32(buffer, 0);
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[BakedFileIndex].Name;
        }
    }

    public class CCSCCCC2000Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC2000; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 BakedFileIndex { get; private set; }

        public List<byte> ExtraData;

        public CCSCCCC2000Block(Stream data)
        {
            ExtraData = new List<byte>();

            byte[] buffer = new byte[4];

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            long nextBlock = data.Position + (_BlockSize * 4);// +8;
            data.Read(buffer, 0, 4); BakedFileIndex = BitConverter.ToUInt32(buffer, 0);

            //Console.WriteLine("CCCC2000 Block of unknown purpose");
            //while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            //{
            //    UInt32 BlockID = BitConverter.ToUInt32(buffer, 0);
            //    Console.WriteLine("\t{0,8:X8}", BlockID);
            //    //CCSBlock newBlock = CCSBlock.MakeBlock(BlockID, data);
            //    //BlockChain.Add(newBlock);
            //}

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 1) > 0))
            {
                ExtraData.Add(buffer[0]);
            }

            //data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Always 0x 00000001: {0,8:X8}", Unknown1);

            //data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown2);

            //data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown3);

            //data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown4);

            Console.WriteLine();
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[BakedFileIndex].Name;
        }
    }
    
    public class CCSCCCC0B00Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0B00; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0B00Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0C00Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0C00; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0C00Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0D80Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0D80; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0D80Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0D90Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0D90; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0D90Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC0E00Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC0E00; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC0E00Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1900Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1900; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1900Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1300Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1300; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1300Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1400Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1400; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1400Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1700Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1700; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1700Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1A00Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1A00; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1A00Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1B00Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1B00; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1B00Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1D00Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1D00; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1D00Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1F00Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1F00; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public UInt32 SelfIndex;

        public List<byte> ExtraData;

        public CCSCCCC1F00Block(Stream data)
        {
            ExtraData = new List<byte>();

            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCC1F00 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            data.Read(buffer, 0, 4); SelfIndex = BitConverter.ToUInt32(buffer, 0);

            //while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            //{
            //    UInt32 raw = BitConverter.ToUInt32(buffer, 0);
            //    //Console.WriteLine("\t{0,8:X8}", BlockID);
            //}

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 1) > 0))
            {
                ExtraData.Add(buffer[0]);
            }

            //Console.WriteLine();

            //Data Sample
            /*
            00 1F CC CC block
            F1 00 00 00 size
            01 00 00 00 node index

            17 00 
            01 00 
            02 00 00 00 

            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 2E 00 
                                    6F 00 00 00 
                                    6F 00 2E 00 
                                    01 00 00 00 00 01 02 03 80 36 00 16 80 00 80 20 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 20 00 
                                    1F 00 00 00 
                                    1F 00 20 00 
                                    01 00 00 00 00 01 02 03 80 0E 00 0F 80 59 80 10 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 20 00 
                                    1F 00 00 00 
                                    1F 00 20 00 
                                    01 00 00 00 00 01 02 03 80 0E 00 0F 00 69 80 10 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 20 00 
                                    1F 00 00 00 
                                    1F 00 20 00 
                                    01 00 00 00 00 01 02 03 80 0E 00 0F 80 00 80 00 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 20 00 
                                    1F 00 00 00 
                                    1F 00 20 00 
                                    01 00 00 00 00 01 02 03 80 0E 00 0F 00 10 80 00 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 20 00 
                                    1F 00 00 00 
                                    1F 00 20 00 
                                    01 00 00 00 00 01 02 03 80 0E 00 0F 80 1F 80 00 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 20 00 
                                    B2 00 00 00 
                                    B2 00 20 00 
                                    01 00 00 00 00 01 02 03 00 58 00 0F 80 00 80 10 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 09 00 
                                    4B 00 00 00 
                                    4B 00 09 00 
                                    01 00 00 00 00 01 02 03 80 24 80 03 00 38 80 34 
            00 00 00 00 04 00 01 00 0F 00 00 00 
                                    0F 00 0F 00 
                                    35 00 00 00 
                                    35 00 0F 00 
                                    01 00 00 00 00 01 02 03 00 12 80 06 80 52 80 00 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 0F 00 
                                    25 00 00 00 
                                    25 00 0F 00 
                                    01 00 00 00 00 01 02 03 80 11 80 06 00 2F 80 00 
            00 00 00 00 04 00 01 00 00 00 00 00 
                                    00 00 0F 00 
                                    22 00 00 00 
                                    22 00 0F 00 
                                    01 00 00 00 00 01 02 03 00 10 80 06 80 41 80 00 
            00 00 00 00 04 00 01 00 04 00 00 00 
                                    04 00 0F 00 
                                    23 00 00 00 
                                    23 00 0F 00 
                                    01 00 00 00 00 01 02 03 80 0E 80 06 00 71 80 00 
            00 00 00 00 04 00 01 00 02 00 00 00 
                                    02 00 0F 00 
                                    23 00 00 00 
                                    23 00 0F 00 
                                    01 00 00 00 00 01 02 03 80 0F 80 06 80 4D 00 08 
            00 00 00 00 04 00 01 00 03 00 00 00 
                                    03 00 0F 00 
                                    41 00 00 00 
                                    41 00 0F 00 
                                    01 00 00 00 00 01 02 03 00 1E 80 06 00 2F 00 08 
            00 00 00 00 08 00 02 00 FF FF FF FF 
                                    FF FF 1F 00 
                                    B5 00 FF FF 
                                    B5 00 1F 00 
                                    B4 00 FF FF 
                                    B4 00 1F 00 
                                    6A 01 FF FF 
                                    6A 01 1F 00 
                                    01 00 00 00 00 01 02 03 00 5A 00 0F 80 00 80 70 
                                    01 00 08 00 04 05 06 07 00 5A 00 0F 80 00 80 70 
            00 00 00 00 04 00 01 00 FF FF FF FF 
                                    FF FF 13 00 
                                    41 00 FF FF 
                                    41 00 13 00 
                                    01 00 00 00 00 01 02 03 00 20 00 09 00 38 80 20 
            00 00 00 00 04 00 01 00 FF FF FF FF 
                                    FF FF 13 00 
                                    41 00 FF FF 
                                    41 00 13 00 
                                    01 00 00 00 00 01 02 03 00 20 00 09 00 59 80 20 
            00 00 00 00 04 00 01 00 FF FF FF FF 
                                    FF FF 13 00 
                                    41 00 FF FF 
                                    41 00 13 00 
                                    01 00 00 00 00 01 02 03 00 20 00 09 00 38 80 2A 
            00 00 00 00 04 00 01 00 FF FF FF FF 
                                    FF FF 13 00 
                                    41 00 FF FF 
                                    41 00 13 00 
                                    01 00 00 00 00 01 02 03 00 20 00 09 00 59 80 2A 
            00 00 00 00 04 00 01 00 FF FF FF FF 
                                    FF FF 11 00 
                                    24 00 FF FF 
                                    24 00 11 00 
                                    01 00 00 00 00 01 02 03 80 11 00 08 80 00 80 37 
            00 00 00 00 04 00 01 00 EC FF F7 FF 
                                    EC FF 08 00 
                                    14 00 F7 FF 
                                    14 00 08 00 
                                    01 00 00 00 00 01 02 03 00 13 80 07 00 13 80 37 
            00 00 00 00 04 00 01 00 FC FF FC FF 
                                    FC FF 05 00 
                                    05 00 FC FF 
                                    05 00 05 00 
                                    01 00 00 00 00 01 02 03 80 03 80 03 80 5D 80 34 
            00 00 00 00 04 00 01 00 BE FF CA FF 
                                    BE FF 28 00 
                                    42 00 CA FF 
                                    42 00 28 00 
                                    01 00 00 00 00 01 02 03 00 41 00 2E 80 00 80 41
            */
        }

        public override string GetNodeName(CCSFileNamesBlock FileNamesBlock)
        {
            return FileNamesBlock.BakedFiles[SelfIndex].Name;
        }
    }

    public class CCSCCCC1901Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1901; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1901Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCC1902Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCC1902; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        public CCSCCCC1902Block(Stream data)
        {
            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            //data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Node Index: {0}", SelfIndex);

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 4) > 0))
            {
                UInt32 raw = BitConverter.ToUInt32(buffer, 0);
                //Console.WriteLine("\t{0,8:X8}", BlockID);
            }

            //Console.WriteLine();
        }
    }

    public class CCSCCCCFF01Block : CCSBlock
    {
        public override CCSBlockType BlockType { get { return CCSBlockType.CCCCFF01; } }
        public override long BlockSize { get { return _BlockSize; } }
        private long _BlockSize;

        //public UInt32 EndMarker;

        public List<byte> ExtraData;

        public CCSCCCCFF01Block(Stream data)
        {
            ExtraData = new List<byte>();

            byte[] buffer = new byte[4];

            //Console.WriteLine("CCCCFF01 Block // unknown");

            data.Read(buffer, 0, 4); _BlockSize = BitConverter.ToUInt32(buffer, 0);
            //Console.WriteLine("Size of following block data: {0} int32s", _BlockSize);

            long nextBlock = data.Position + (_BlockSize * 4);// +8;

            while ((data.Position < nextBlock) && (data.Read(buffer, 0, 1) > 0))
            {
                ExtraData.Add(buffer[0]);
            }

            //data.Read(buffer, 0, 4); EndMarker = BitConverter.ToUInt32(buffer, 0);
        }
    }
}
