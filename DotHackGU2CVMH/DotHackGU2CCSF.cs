using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DotHackGU2CVMH
{
    class DotHackGU2CCSF
    {
        private Stream data;
 
        const byte b00000001 = 1;
        const byte b00000010 = 1 << 1;
        const byte b00000100 = 1 << 2;
        const byte b00001000 = 1 << 3;
        const byte b00010000 = 1 << 4;
        const byte b00100000 = 1 << 5;
        const byte b01000000 = 1 << 6;
        const byte b10000000 = 1 << 7;

        public DotHackGU2CCSF(Stream data)
        {
            long pos = data.Position;

            this.data = data;

            byte[] buffer = new byte[4];

            string[] nodeNames = null;

            Dictionary<int, Color[]> Pallets = new Dictionary<int, Color[]>();

            while (data.Read(buffer, 0, 4) > 0)
            {
                UInt32 BlockID = BitConverter.ToUInt32(buffer, 0);

                Console.WriteLine("[{0,8:X8}]", data.Position - 4);

                switch (BlockID)
                {
                    case 0xCCCC0001: // File Info Block
                        {
                            Console.WriteLine("CCCC0001 Block // File Info");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size of following block data: {0} int32s", Size);

                            long nextBlock = data.Position + (Size * 4);

                            data.Read(buffer, 0, 4); string CCSFmarker = ASCIIEncoding.ASCII.GetString(buffer);
                            Console.WriteLine("CCSF Marker: \"{0}\"", CCSFmarker.TrimEnd('\0'));

                            byte[] bufferTmp = new byte[32];
                            data.Read(bufferTmp, 0, 32); string Filename = ASCIIEncoding.ASCII.GetString(bufferTmp);
                            Console.WriteLine("Filename: \"{0}\"", Filename.TrimEnd('\0'));

                            data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000123: {0,8:X8}", Unknown1);

                            data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown: {0,8:X8}", Unknown2);

                            data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000001: {0,8:X8}", Unknown3);

                            data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown4);

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0002:
                        {
                            Console.WriteLine("CCCC0002 Block // Filenames");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            if (Count == 0x00000D85)
                            {
                                Console.WriteLine("Triggered Hardcode for 0x00000D85");
                                Console.WriteLine("Unknown: {0,8:X8}", Count);

                                data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown1);

                                data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown2);

                                data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown3);
                            }
                            else
                            {
                                Console.WriteLine("Size of following block data: {0} int32s", Count);

                                long nextBlock = data.Position + (Count * 4);// +8;

                                data.Read(buffer, 0, 4); UInt32 Lines1 = BitConverter.ToUInt32(buffer, 0);
                                data.Read(buffer, 0, 4); UInt32 Lines2 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Base Files: {0}", Lines1);
                                for (int x = 0; x < Lines1; x++)
                                {
                                    byte[] bufferTmp = new byte[32];
                                    data.Read(bufferTmp, 0, 32); string Filename = ASCIIEncoding.ASCII.GetString(bufferTmp);
                                    Console.WriteLine("[{0}] = \"{1}\"", x, Filename.TrimEnd('\0'));
                                }
                                Console.WriteLine("Nodes (Baked Files): {0}", Lines2);
                                nodeNames = new string[Lines2];
                                for (int x = 0; x < Lines2; x++)
                                {
                                    byte[] bufferTmp = new byte[30];
                                    data.Read(bufferTmp, 0, 30); string Filename = ASCIIEncoding.ASCII.GetString(bufferTmp);
                                    data.Read(buffer, 0, 2); UInt16 BaseFileIndex = BitConverter.ToUInt16(buffer, 0);
                                    Console.WriteLine("[{0}] = \"{1}\":{2}", x, Filename.TrimEnd('\0'), BaseFileIndex);

                                    nodeNames[x] = Filename.TrimEnd('\0');
                                }

                                data.Seek(nextBlock, SeekOrigin.Begin);

                                Console.WriteLine();

                                data.Read(buffer, 0, 4); UInt32 ANOMALY1 = BitConverter.ToUInt32(buffer, 0);
                                if (ANOMALY1 == 0x00000003)
                                {
                                    Console.WriteLine("[{0,8:X8}]", data.Position - 4);

                                    data.Read(buffer, 0, 4); UInt32 ANOMALY2 = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("ANOMALY: {0,8:X8} {1,8:X8}", ANOMALY1, ANOMALY2);
                                    Console.WriteLine();
                                }
                                else
                                {
                                    data.Seek(nextBlock, SeekOrigin.Begin);
                                }
                            }
                        }
                        break;
                    case 0xCCCC0003: // unknown
                        {
                            Console.WriteLine("CCCC0001 Block");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size of following block data: {0} int32s", Count);

                            long nextBlock = data.Position + (Count * 4);

                            

                            Console.WriteLine();

                            data.Seek(nextBlock, SeekOrigin.Begin);
                        }
                        break;
                    case 0xCCCC0400: // Pallet
                        {
                            Console.WriteLine("CCCC0400 Block // Pallet");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size of following block data: {0} int32s", Count);
                            long nextBlock = data.Position + (Count * 4);

                            data.Read(buffer, 0, 4); UInt32 BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", BakedFileIndex);
                            if (nodeNames != null && nodeNames.Length > BakedFileIndex) Console.Write(" \"{0}\"", nodeNames[BakedFileIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown2);

                            data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 70000001: {0,8:X8}", Unknown3);

                            data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown: {0,8:X8}", Unknown4);

                            data.Read(buffer, 0, 4); UInt32 ColorCount = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Color Count: {0}", ColorCount);

                            Color[] cols = new Color[ColorCount];
                            Bitmap palletImage = new Bitmap((int)ColorCount, 1);
                            for (int x = 0; x < ColorCount; x++)
                            {
                                data.Read(buffer, 0, 4); // rgba

                                cols[x] = Color.FromArgb(buffer[3], buffer[0], buffer[1], buffer[2]);
                                palletImage.SetPixel(x, 0, Color.FromArgb((int)(cols[x].A * 255.0 / 128.0), cols[x]));
                            }
                            Pallets.Add((int)BakedFileIndex, cols);

                            if (nodeNames != null && nodeNames.Length > BakedFileIndex)
                            {
                                var invalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());

                                string cleanedFilename = new string(nodeNames[BakedFileIndex]
                                .Where(x => !invalidChars.Contains(x))
                                .ToArray());

                                if (cleanedFilename.Length == 0) cleanedFilename += "_";

                                if (!File.Exists(cleanedFilename + @".png"))
                                    palletImage.Save(cleanedFilename + @".png", System.Drawing.Imaging.ImageFormat.Png);
                            }

                            Console.WriteLine();

                            data.Seek(nextBlock, SeekOrigin.Begin);
                        }
                        break;
                    case 0xCCCC0300: // Texture
                        {
                            Console.WriteLine("CCCC0300 Block // Texture");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);

                            data.Read(buffer, 0, 4); UInt32 BakedFileIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", BakedFileIndex);
                            if (nodeNames != null && nodeNames.Length > BakedFileIndex) Console.Write(" \"{0}\"", nodeNames[BakedFileIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 PalletFileIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Pallet Node Index: {0}", PalletFileIndex);
                            if (nodeNames != null && nodeNames.Length > PalletFileIndex) Console.Write(" \"{0}\"", nodeNames[PalletFileIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown4);

                            data.Read(buffer, 0, 4); UInt32 Unknown5 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown: {0,8:X8}", Unknown5);

                            int width = data.ReadByte();
                            int height = data.ReadByte();
                            Console.WriteLine("Width: {0}", (int)Math.Pow(2, width));
                            Console.WriteLine("Height: {0}", (int)Math.Pow(2, height));

                            data.Read(buffer, 0, 2); UInt16 CDCD = BitConverter.ToUInt16(buffer, 0);
                            Console.WriteLine("CDCD Marker: {0,4:X4}", CDCD);

                            //int width = (int)Math.Pow(2, buffer[0]);
                            //int height = (int)Math.Pow(2, buffer[1]);

                            data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown: {0,8:X8}", Unknown);

                            data.Read(buffer, 0, 4); UInt32 PixelCount = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Count Pixels: {0}", PixelCount * 4);

                            Color[] pallet = Pallets[(int)PalletFileIndex];
                            uint rawPixelCount = PixelCount * 4;

                            width = (int)Math.Pow(2, width);
                            height = (int)Math.Pow(2, height);

                            Bitmap output = new Bitmap(width, height);
                            for (int x = 0; x < rawPixelCount; x++)
                            {
                                byte col = (byte)data.ReadByte();

                                if (pallet.Length == 16)
                                {
                                    byte P1 = (byte)(col & 0x0f);
                                    byte P2 = (byte)((col & 0xf0) >> 4);

                                    output.SetPixel((x * 2) % width, height - 1 - ((x * 2) / width), Color.FromArgb((int)(pallet[P1].A / 128.0 * 255.0), pallet[P1]));
                                    output.SetPixel(((x * 2) + 1) % width, height - 1 - (((x * 2) + 1) / width), Color.FromArgb((int)(pallet[P2].A / 128.0 * 255.0), pallet[P2]));
                                }
                                else
                                {
                                    output.SetPixel(x % width, height - 1 - (x / width), Color.FromArgb((int)(pallet[col].A / 128.0 * 255.0), pallet[col]));
                                }
                            }
                            if (nodeNames != null && nodeNames.Length > BakedFileIndex)
                            {
                                var invalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());

                                string cleanedFilename = new string(nodeNames[BakedFileIndex]
                                .Where(x => !invalidChars.Contains(x))
                                .ToArray());

                                if (cleanedFilename.Length == 0) cleanedFilename += "_";

                                if (!File.Exists(cleanedFilename + @".png"))
                                    output.Save(cleanedFilename + @".png", System.Drawing.Imaging.ImageFormat.Png);
                            }

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0005: // unknown
                        {
                            Console.WriteLine("CCCC0005 Block");

                            data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                            data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown: {0,8:X8} {1,8:X8}", Unknown1, Unknown2);
                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0500: // unknown
                        {
                            Console.WriteLine("CCCC0500 Block");

                            data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                            data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown: {0,8:X8} {1,8:X8}", Unknown1, Unknown2);
                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0A00: // Skeleton?
                        {
                            Console.WriteLine("CCCC0A00 Block // Skeleton?");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Count: {0} (Should be 3)", Count);

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 ParentIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Parent Node Index: {0}", ParentIndex);
                            if (nodeNames != null && nodeNames.Length > ParentIndex) Console.Write(" \"{0}\"", nodeNames[ParentIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 SelfIndex2 = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("2nd Node Index: {0}", SelfIndex2);
                            if (nodeNames != null && nodeNames.Length > SelfIndex2) Console.Write(" \"{0}\"", nodeNames[SelfIndex2]);
                            Console.WriteLine();

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC2000: // unknown
                        {
                            Console.WriteLine("CCCC2000 Block");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Count: {0} (should be 5)", Count);

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000001: {0,8:X8}", Unknown1);

                            data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown2);

                            data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown3);

                            data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Always 0x 00000000: {0,8:X8}", Unknown4);

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0200: // Material
                        {
                            Console.WriteLine("CCCC0200 Block // Material");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Count: {0} (should be 5)", Count);

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 TextureIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Texture Index: {0}", TextureIndex);
                            if (nodeNames != null && nodeNames.Length > TextureIndex) Console.Write(" \"{0}\"", nodeNames[TextureIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown2: {0,8:X8}", Unknown2);

                            data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown3: {0,8:X8}", Unknown3);

                            data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown4: {0,8:X8}", Unknown4);

                            ObjModel model = new ObjModel(nodeNames[SelfIndex]);
                            model.mats.Add(new ObjModel.Material()
                            {
                                Name = nodeNames[SelfIndex],
                                map_Kd = nodeNames[TextureIndex] + ".png"
                            });


                            var invalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());

                            string cleanedFilename = new string(nodeNames[SelfIndex]
                            .Where(x => !invalidChars.Contains(x))
                            .ToArray());

                            if (cleanedFilename.Length == 0) cleanedFilename += "_";

                            //if (!File.Exists(cleanedFilename + @".mtl"))
                            model.MaterialToFile(cleanedFilename + @".mtl");


                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0202: // unknown
                        {
                            Console.WriteLine("CCCC0202 Block");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Count: {0} (should be 6)", Count);

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            //data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                            //Console.WriteLine("Unknown1: {0,8:X8}", Unknown1);

                            //data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            //Console.WriteLine("Unknown2: {0,8:X8}", Unknown2);

                            //data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                            //Console.WriteLine("Unknown3: {0,8:X8}", Unknown3);

                            //data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                            //Console.WriteLine("Unknown4: {0,8:X8}", Unknown4);

                            //data.Read(buffer, 0, 4); UInt32 Unknown5 = BitConverter.ToUInt32(buffer, 0);
                            //Console.WriteLine("Unknown5: {0,8:X8}", Unknown5);

                            for (int x = 0; x < Count - 1; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown1);
                            }

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0503: // unknown
                        {
                            Console.WriteLine("CCCC0503 Block");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Count: {0}", Count);

                            for (int x = 0; x < Count; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown1);
                            }

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0102: // unknown
                        {
                            Console.WriteLine("CCCC0102 Block // complex data, bitwise toggles");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0} int32s", Size);

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 Marker = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Marker: {0,8:X8}", Marker);

                            byte ToggleInputs1 = buffer[0];
                            byte ToggleInputs2 = buffer[1];

                            if ((ToggleInputs1 & b00000001) == b00000001)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("Unknown0: {0,8:X8}", Unknown);
                                }
                            }
                            if ((ToggleInputs1 & b00000010) == b00000010)
                            {
                                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Count1: {0}", Counter);
                                for (int x = 0; x < Counter; x++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("Unknown1: {0,8:X8} {1,8:X8} {2,8:X8} {3,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4);
                                }
                            }
                            if ((ToggleInputs1 & b00000100) == b00000100)
                            {
                                throw new Exception(string.Format("Unknown2 CCCC0102 Block Type {0,8:X8}", Marker));
                            }
                            if ((ToggleInputs1 & b00001000) == b00001000)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("Unknown3: {0,8:X8}", Unknown);
                                }
                            }
                            if ((ToggleInputs1 & b00010000) == b00010000)
                            {
                                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Count4: {0}", Counter);
                                for (int x = 0; x < Counter; x++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("Unknown4: {0,8:X8} {1,8:X8} {2,8:X8} {3,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4);
                                }
                            }
                            if ((ToggleInputs1 & b00100000) == b00100000)
                            {
                                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Count5: {0}", Counter);
                                for (int x = 0; x < Counter; x++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown5 = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("Unknown5: {0,8:X8} {1,8:X8} {2,8:X8} {3,8:X8} {4,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4, Unknown5);
                                }
                            }
                            if ((ToggleInputs1 & b01000000) == b01000000)
                            {
                                for (int x = 0; x < 3; x++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("Unknown6: {0,8:X8}", Unknown);
                                }
                            }
                            if ((ToggleInputs1 & b10000000) == b10000000)
                            {
                                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Count7: {0}", Counter);
                                for (int x = 0; x < Counter; x++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("Unknown7: {0,8:X8} {1,8:X8} {2,8:X8} {3,8:X8}", Unknown1, Unknown2, Unknown3, Unknown4);
                                }
                            }

                            if ((ToggleInputs2 & b00000001) == b00000001)
                            {
                                throw new Exception(string.Format("Unknown8 CCCC0102 Block Type {0,8:X8}", Marker));
                            }
                            if ((ToggleInputs2 & b00000010) == b00000010)
                            {
                                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown9: {0}", Counter);
                            }
                            if ((ToggleInputs2 & b00000100) == b00000100)
                            {
                                data.Read(buffer, 0, 4); UInt32 Counter = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("CountA: {0}", Counter);
                                for (int x = 0; x < Counter; x++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("UnknownA: {0,8:X8} {1,8:X8}", Unknown1, Unknown2);
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

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0700: // unknown
                        {
                            Console.WriteLine("CCCC0700 Block");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0}", Size);

                            long endPost = data.Position + Size * 4;

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown1: {0}", Unknown1);

                            data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown2: {0,8:X8}", Unknown2);

                            Console.WriteLine();

                            /*for (int x = 0; x < Count; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 CCCCFF01 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("----CCCCFF01 Sub-Block: {0,8:X8}", CCCCFF01);

                                data.Read(buffer, 0, 4); UInt32 InnerSize = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("----Count: {0,8:X8}", InnerSize);

                                for (int y = 0; y < InnerSize; y++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 InnerUnknown = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("--------Unknown: {0,8:X8}", InnerUnknown);
                                }
                            }*/

                            Console.WriteLine("//Inner Data//");

                            data.Seek(endPost, SeekOrigin.Begin);

                            Console.WriteLine();
                        }
                        break;
                    /*case 0xCCCC0E00: // unknown
                        {
                            Console.WriteLine("CCCC0E00 Block");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0} int32s", Size);

                            for (int x = 0; x < Size; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;*/
                    /*case 0xCCCC0900: // unknown
                        {
                     * 
                     * 
                     * 
                     * 
                     * 
                     * 
                     * 
                     * Console.WriteLine("CCCC0900 Block");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0} 32bit vals", Count);
                            for (int x = 0; x < Count; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;*/
                    /*case 0xCCCC0100: // unknown
                        {
                            Console.WriteLine("CCCC0100 Block");

                            data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Count: {0}", Count);
                            for (int x = 0; x < Count; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;*/
                    case 0xCCCC0800: // Mesh
                        {
                            Console.WriteLine("CCCC0800 Block // Mesh");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0}", Size);

                            long startPosition = data.Position;
                            long endPosition = data.Position + (Size * 4);

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            try
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown1: {0,8:X8} (if 0, end early)", Unknown1);

                                data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown2: {0,8:X8}", Unknown2);

                                data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown3: {0,8:X8}", Unknown3);

                                data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown4: {0,8:X8}", Unknown4);

                                int Unknown5A = data.ReadByte();
                                int Unknown5B = data.ReadByte();
                                int Unknown5C = data.ReadByte();
                                int Unknown5D = data.ReadByte();
                                Console.WriteLine("Unknown5: {0,2:X2} {1,2:X2} {2,2:X2} {3,2:X2}", Unknown5A, Unknown5B, Unknown5C, Unknown5D);

                                data.Read(buffer, 0, 4); UInt32 UnknownFloatA = BitConverter.ToUInt32(buffer, 0);
                                float UnknownFloatB = BitConverter.ToSingle(buffer, 0);
                                Console.WriteLine("UnknownFloat: {0,8:X8} {1}", UnknownFloatA, UnknownFloatB.ToString("R", CultureInfo.InvariantCulture));

                                if (Unknown1 != 0)
                                {
                                    if (Unknown5D == 0x80) // shadow
                                    {
                                        data.Read(buffer, 0, 4); UInt32 Count1 = BitConverter.ToUInt32(buffer, 0);
                                        Console.WriteLine("Count1: {0,8:X8}", Count1);

                                        data.Read(buffer, 0, 4); UInt32 Count2 = BitConverter.ToUInt32(buffer, 0);
                                        Console.WriteLine("Count2: {0,8:X8}", Count2);

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
                                            Console.WriteLine("Data1: {0} {1} {2}", h1, h2, h3);
                                        }
                                        data.Seek((4 - 1) / 4 * 4, SeekOrigin.Current); // Align

                                        for (int x = 0; x < Count2; x++)
                                        {
                                            data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                            Console.WriteLine("Data2: {0,8:X8}", Unknown);
                                        }
                                    }
                                    else
                                    {
                                        data.Read(buffer, 0, 4); UInt32 UnknownA1 = BitConverter.ToUInt32(buffer, 0);
                                        Console.WriteLine("UnknownA1: {0,8:X8}", UnknownA1);

                                        data.Read(buffer, 0, 4); UInt32 MaterialIndex = BitConverter.ToUInt32(buffer, 0);
                                        Console.Write("Material Index: {0}", MaterialIndex);
                                        if (nodeNames != null && nodeNames.Length > MaterialIndex) Console.Write(" \"{0}\"", nodeNames[MaterialIndex]);
                                        Console.WriteLine();

                                        ObjModel model = new ObjModel(nodeNames[SelfIndex]);

                                        data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                                        Console.WriteLine("Count: {0} ({0,8:X8})", Count);

                                        for (int x = 0; x < Count; x++)
                                        {
                                            //Console.Write("Data1:");
                                            //Console.Write(" {0,2:X2}", data.ReadByte());
                                            //Console.Write(" {0,2:X2}", data.ReadByte());
                                            //Console.Write(" {0,2:X2}", data.ReadByte());
                                            //Console.Write(" {0,2:X2}", data.ReadByte());
                                            //Console.Write(" {0,2:X2}", data.ReadByte());
                                            //Console.Write(" {0,2:X2}", data.ReadByte());
                                            //Console.WriteLine();

                                            //data.Read(buffer, 0, 2); Half h1 = Half.FromBinary(buffer, 0);
                                            //data.Read(buffer, 0, 2); Half h2 = Half.FromBinary(buffer, 0);
                                            //data.Read(buffer, 0, 2); Half h3 = Half.FromBinary(buffer, 0);
                                            //Console.WriteLine("Data1: {0} {1} {2}", h1, h2, h3);

                                            data.Read(buffer, 0, 2); Int16 h1 = BitConverter.ToInt16(buffer, 0);
                                            data.Read(buffer, 0, 2); Int16 h2 = BitConverter.ToInt16(buffer, 0);
                                            data.Read(buffer, 0, 2); Int16 h3 = BitConverter.ToInt16(buffer, 0);
                                            //Console.WriteLine("XYZ [{0}]: {1,4:X4} {2,4:X4} {3,4:X4} {1} {2} {3}", x, h1, h2, h3);
                                            Console.WriteLine("XYZ [{0}]: {1} {2} {3}", x, h1, h2, h3);

                                            model.vertices.Add(new ObjModel.Vector3() { x = h1 * 0.1f, y = h2 * 0.1f, z = h3 * 0.1f });

                                            if (data.Position >= data.Length) throw new Exception("Read Too Far");
                                        }
                                        data.Seek((4 - 1) / 4 * 4, SeekOrigin.Current); // Align

                                        UInt32[] normData = new UInt32[Count];
                                        for (int x = 0; x < Count; x++)
                                        {
                                            data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                            //Console.WriteLine("Data2: {0,8:X8} {1}", Unknown, Convert.ToString(Unknown, 2).PadLeft(32, '0'));
                                            normData[x] = Unknown;

                                            model.normals.Add(new ObjModel.Vector3() {
                                                x = (sbyte)buffer[0] / 64.0f,
                                                y = (sbyte)buffer[1] / 64.0f,
                                                z = (sbyte)buffer[2] / 64.0f
                                            });

                                            Console.WriteLine("Normal [{0}]: {1} {2} {3} {4}",
                                                x,
                                                (sbyte)buffer[0],
                                                (sbyte)buffer[1],
                                                (sbyte)buffer[2],
                                                buffer[3] == 0x01 ? "No Draw, Force Odd" : buffer[3] == 0x02 ? "No Draw, Force Even" : string.Empty);
                                        }

                                        for (int x = 0; x < Count; x++)
                                        {
                                            data.Read(buffer, 0, 4); //UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                            Console.WriteLine("RGBA [{0}]: {1,2:X2} {2,2:X2} {3,2:X2} {4,2:X2}", x, buffer[0], buffer[1], buffer[2], buffer[3]);
                                        }

                                        for (int x = 0; x < Count; x++)
                                        {
                                            data.Read(buffer, 0, 2); UInt16 U = BitConverter.ToUInt16(buffer, 0);
                                            data.Read(buffer, 0, 2); UInt16 V = BitConverter.ToUInt16(buffer, 0);
                                            Console.WriteLine("UV [{0}]: {1} {2}", x, U, V);

                                            model.uv.Add(new ObjModel.Vector3() { x = U / 255.0f, y = V / 255.0f });
                                        }

                                        {
                                            string material = nodeNames[MaterialIndex];
                                            List<int> verts = new List<int>();
                                            bool winding = false;
                                            for (int v = 0; v < Count; v++)
                                            {
                                                if ((normData[v] & 0x03000000) == 0)
                                                {
                                                    if (v > 1)
                                                    {
                                                        if (winding)
                                                        {
                                                            verts.Add(v - 2);
                                                            verts.Add(v - 1);
                                                            verts.Add(v);
                                                        }
                                                        else
                                                        {
                                                            verts.Add(v - 2);
                                                            verts.Add(v);
                                                            verts.Add(v - 1);
                                                        }
                                                    }
                                                    winding = !winding;
                                                }
                                                else if ((normData[v] & 0x01000000) > 0)
                                                {
                                                    winding = true;
                                                }
                                                else if ((normData[v] & 0x02000000) > 0)
                                                {
                                                    winding = false;
                                                }
                                            }
                                            model.faces.Add(material, verts);


//model.mats.Add(new ObjModel.Material()
//{
//    Name = material,
//    map_Kd = material + ".png"
//});




                                            var invalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());

                                            string cleanedFilename = new string(nodeNames[SelfIndex]
                                            .Where(x => !invalidChars.Contains(x))
                                            .ToArray());

                                            if (cleanedFilename.Length == 0) cleanedFilename += "_";

                                            //string append = "";
                                            //int i = 0;
                                            //while (File.Exists(cleanedFilename + append + @".obj"))
                                            //{
                                            //    append = string.Format("[{0}]", i);
                                            //    i++;
                                            //}

                                            //model.MeshToFile(cleanedFilename + append + @".obj");

                                            //if (!File.Exists(cleanedFilename + @".obj"))
                                                model.MeshToFile(cleanedFilename + @".obj");

////if (!File.Exists(cleanedFilename + @".mtl"))
//    model.MaterialToFile(cleanedFilename + @".mtl");
                                        }
                                    }
                                }

                                //if (data.Position > endPosition) throw new Exception("Read Too Far");
                                if (data.Position > endPosition) throw new Exception("Overrun");
                                if (data.Position < endPosition) throw new Exception("Underun");

                                /*{
                                    data.Seek(startPosition, SeekOrigin.Begin);

                                    data.Read(buffer, 0, 4); SelfIndex = BitConverter.ToUInt32(buffer, 0);
                                    var invalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());

                                    string cleanedFilename = new string(nodeNames[SelfIndex]
                                    .Where(x => !invalidChars.Contains(x))
                                    .ToArray());

                                    if (cleanedFilename.Length == 0) cleanedFilename += "_";

                                    string append = "";
                                    int i = 0;
                                    while (File.Exists(cleanedFilename + append + @"[GOODPARSE].bin"))
                                    {
                                        append = string.Format("[{0}]", i);
                                        i++;
                                    }

                                    using (FileStream outStream = File.Create(cleanedFilename + append + @"[GOODPARSE].bin"))
                                    {
                                        for (int x = 0; x < (Size - 1); x++)
                                        {
                                            data.Read(buffer, 0, 4);
                                            outStream.Write(buffer, 0, 4);
                                        }
                                    }
                                }*/
                            }
                            catch(Exception ex)
                            {
                                data.Seek(endPosition, SeekOrigin.Begin);

                                //data.Seek(startPosition, SeekOrigin.Begin);

                                //data.Read(buffer, 0, 4); SelfIndex = BitConverter.ToUInt32(buffer, 0);
                                ////var invalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars());

                                //string cleanedFilename = new string(nodeNames[SelfIndex]
                                //.Where(x => !invalidChars.Contains(x))
                                //.ToArray());

                                //if (cleanedFilename.Length == 0) cleanedFilename += "_";

                                //string append = "";
                                //int i = 0;
                                ////if (ex.Message == "Read Too Far")
                                //{
                                //    /*while (File.Exists(cleanedFilename + append + @"[OVERRUN].bin"))
                                //    {
                                //        append = string.Format("[{0}]", i);
                                //        i++;
                                //    }*/

                                //    //using (FileStream outStream = File.Create(cleanedFilename + append + @"[OVERRUN].bin"))
                                //    {
                                //        for (int x = 0; x < (Size - 1); x++)
                                //        {
                                //            data.Read(buffer, 0, 4);
                                //            //outStream.Write(buffer, 0, 4);
                                //        }
                                //    }
                                //}
                                ///*else
                                //{
                                //    while (File.Exists(cleanedFilename + append + @"[BADPARSE].bin"))
                                //    {
                                //        append = string.Format("[{0}]", i);
                                //        i++;
                                //    }

                                //    using (FileStream outStream = File.Create(cleanedFilename + append + @"[BADPARSE].bin"))
                                //    {
                                //        for (int x = 0; x < (Size - 1); x++)
                                //        {
                                //            data.Read(buffer, 0, 4);
                                //            outStream.Write(buffer, 0, 4);
                                //        }
                                //    }
                                //}*/
                            }

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC2400: // articles?
                        {
                            Console.WriteLine("CCCC2400 Block // articles?");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            Console.WriteLine("//Article Data//");

                            data.Seek((Size * 4) - 4, SeekOrigin.Current);

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0D90: // unknown
                        {
                            Console.WriteLine("CCCC0D90 Block");

                            data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown1: {0,8:X8}", Unknown1);
                            data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown2: {0,8:X8}", Unknown2);
                            data.Read(buffer, 0, 4); UInt32 Unknown3 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown3: {0,8:X8}", Unknown3);
                            data.Read(buffer, 0, 4); UInt32 Unknown4 = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown4: {0,8:X8}", Unknown4);

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCCFFFF: // unknown
                        {
                            Console.WriteLine("CCCCFFFF Block // delimiter hack in effect");

                            /*data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Unknown: {0,8:X8}", Count);

                            data.Read(buffer, 0, 2); UInt16 Count1 = BitConverter.ToUInt16(buffer, 0);
                            Console.WriteLine("Count1: {0}", Count1);

                            data.Read(buffer, 0, 2); UInt16 Count2 = BitConverter.ToUInt16(buffer, 0);
                            Console.WriteLine("Count2: {0}", Count2);

                            for (int x = 0; x < Count1; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown1: {0,8:X8}", Unknown);
                            }

                            for (int x = 0; x < Count2; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown2: {0,8:X8}", Unknown);
                            }*/

                            for (; ; )
                            {
                                if (data.Read(buffer, 0, 4) == 0) break;
                                if (buffer[2] == 0xCC && buffer[3] == 0xCC)
                                {
                                    data.Seek(-4, SeekOrigin.Current);
                                    break;
                                }
                                UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();

                            /*long nextBlock = data.Position;
                            data.Read(buffer, 0, 4); UInt32 ANOMALY1 = BitConverter.ToUInt32(buffer, 0);
                            if (ANOMALY1 == 0x00000000)
                            {
                                data.Read(buffer, 0, 4); UInt32 ANOMALY2 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("ANOMALY: {0,8:X8} {1,8:X8}", ANOMALY1, ANOMALY2);
                                Console.WriteLine();
                            }
                            else
                            {
                                data.Seek(nextBlock, SeekOrigin.Begin);
                            }*/
                        }
                        break;
                    /*case 0xCCCC0C00: // unknown
                        {
                            Console.WriteLine("CCCC0C00 Block");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);

                            for (int x = 0; x < Size; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;*/
                    /*case 0xCCCC1900: // unknown
                        {
                            Console.WriteLine("CCCC1900 Block");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);

                            for (int x = 0; x < Size; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;*/
                    /*case 0xCCCC1902: // unknown
                        {
                            Console.WriteLine("CCCC1902 Block");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);

                            for (int x = 0; x < Size; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;*/
                    /*case 0xCCCC1300: // unknown
                        {
                            Console.WriteLine("CCCC1300 Block");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);

                            for (int x = 0; x < Size; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;*/
                    case 0xCCCC0000: // unknown
                        {
                            Console.WriteLine("CCCC0000 Block");

                            for (int x = 0; x < 7; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC00FD: // unknown
                        {
                            Console.WriteLine("CCCC00FD Block");

                            for (int x = 0; x < 7; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            for (; ; )
                            {
                                if (data.Read(buffer, 0, 4) == 0) break;
                                if (buffer[2] == 0xCC && buffer[3] == 0xCC)
                                {
                                    data.Seek(-4, SeekOrigin.Current);
                                    break;
                                }
                                UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown Delimiter Overflow: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;
                    /*case 0xCCCC0D80: // unknown
                        {
                            Console.WriteLine("CCCC0D80 Block");

                            for (int x = 0; x < 3; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;*/
                    case 0xCCCC1400: // unknown
                        {
                            Console.WriteLine("CCCC1400 Block");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);

                            for (int x = 0; x < Size; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC0051: // unknown
                        {
                            Console.WriteLine("CCCC0051 Block // delimiter based stop");

                            for (; ; )
                            {
                                if (data.Read(buffer, 0, 4) == 0) break;
                                if (buffer[2] == 0xCC && buffer[3] == 0xCC)
                                {
                                    data.Seek(-4, SeekOrigin.Current);
                                    break;
                                }
                                UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown: {0,8:X8}", Unknown);
                            }

                            Console.WriteLine();
                        }
                        break;
                    case 0xCCCC1F00: // Sprite Data?
                        {
                            Console.WriteLine("CCCC1F00 Block // Sprite Data?");

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);
                            long nextBlock = data.Position + (Size * 4);

                            data.Read(buffer, 0, 4); UInt32 SelfIndex = BitConverter.ToUInt32(buffer, 0);
                            Console.Write("Node Index: {0}", SelfIndex);
                            if (nodeNames != null && nodeNames.Length > SelfIndex) Console.Write(" \"{0}\"", nodeNames[SelfIndex]);
                            Console.WriteLine();

                            data.Read(buffer, 0, 2); UInt16 Count1 = BitConverter.ToUInt16(buffer, 0);
                            Console.WriteLine("Count 1: {0}", Count1);

                            data.Read(buffer, 0, 2); UInt16 Count2 = BitConverter.ToUInt16(buffer, 0);
                            Console.WriteLine("Count 2: {0,4:X4}", Count2);

                            for (int x = 0; x < Count2; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("Unknown (Count2): {0,8:X8}", Unknown2);
                            }

                            for (int x = 0; x < Count1; x++)
                            {
                                data.Read(buffer, 0, 4); UInt32 Always0x00000000 = BitConverter.ToUInt32(buffer, 0);
                                Console.WriteLine("----Always 0x00000000: {0,8:X8}", Always0x00000000);

                                data.Read(buffer, 0, 2); UInt16 CountInnerA = BitConverter.ToUInt16(buffer, 0);
                                Console.WriteLine("----CountA: {0}", CountInnerA);

                                data.Read(buffer, 0, 2); UInt16 CountInnerB = BitConverter.ToUInt16(buffer, 0);
                                Console.WriteLine("----CountB: {0}", CountInnerB);

                                for (int y = 0; y < CountInnerA; y++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 UnknownI = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("--------Unknown Inner A: {0,8:X8}", UnknownI);
                                }

                                for (int y = 0; y < CountInnerB; y++)
                                {
                                    data.Read(buffer, 0, 4); UInt32 CountInnerInnerBA = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("--------CountBA: {0}", CountInnerInnerBA);

                                    data.Read(buffer, 0, 4); UInt32 UnknownI1 = BitConverter.ToUInt32(buffer, 0);
                                    data.Read(buffer, 0, 4); UInt32 UnknownI2 = BitConverter.ToUInt32(buffer, 0);
                                    Console.WriteLine("--------Unknown Inner B: {0,8:X8} {1,8:X8}", UnknownI1, UnknownI2);

                                    for (int z = 0; z < CountInnerInnerBA; z++)
                                    {
                                        data.Read(buffer, 0, 4); UInt32 UnknownI4 = BitConverter.ToUInt32(buffer, 0);
                                        Console.WriteLine("------------Unknown Inner B>A: {0}", UnknownI4);
                                    }
                                }
                            }

                            //for (int x = 0; x < Count1; x++)
                            //{
                            //    Console.WriteLine("\tUnknown:");
                            //    for (int y = 0; y < 40; y++)//????????
                            //    {
                            //        Console.Write(" {0,2:X2}", data.ReadByte());
                            //    }
                            //    Console.WriteLine();
                            //}
                            Console.WriteLine();

                            data.Seek(nextBlock, SeekOrigin.Begin);
                        }
                        break;
                    default:
                        if ((BlockID & 0xCCCC0000) == 0xCCCC0000)
                        {
                            Console.WriteLine("{0,8:X8} Block // No Parser", BlockID);

                            data.Read(buffer, 0, 4); UInt32 Size = BitConverter.ToUInt32(buffer, 0);
                            Console.WriteLine("Size: {0,8:X8}", Size);
                            long nextBlock = data.Position + (Size * 4);

                            data.Seek(Size * 4, SeekOrigin.Current);

                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Unknown Block: {0,8:X8}", BlockID);
                            throw new Exception("Unknown Block");
                        }
                        break;
                }
                Console.ReadKey(true);
            }
        }
    }
}
