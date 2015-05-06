using DotHackGUCCS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotHackGUCCSTestRig
{
    class Program
    {
        static void Main(string[] args)
        {
            //string exportPath = @"D:\Data\~working\Extracted Data\ext\";

            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\WALL\");
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\PBBS\");
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\ANIMAL\");
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\AVA\PC\");
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\MP\");
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\FIELD\");
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\EQ\");
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\WEB\");
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\GC\");
            DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\");

            FileInfo[] files = dir.GetFiles("*.tmp", SearchOption.AllDirectories);
            foreach (var file in files)
            //var file = files.First();
            //if (args.Length > 0)
            {
                //string filename = @"D:\Data\~working\Extracted Data\data\WEB\nc_001.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\WEB\nc_cmn.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\WEB\nm_a_001.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\WEB\xtwhpcmn.tmp";

                //string filename = @"D:\Data\~working\Extracted Data\data\TOWN\xw.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\TOWN\xcg.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\TOWN\se1_04.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\TOWN\rt00.tmp";

                //string filename = @"D:\Data\~working\Extracted Data\data\FIELD\f_common.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\FIELD\statue.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\FIELD\statue02.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\FIELD\itembox02.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\FIELD\itembox32.tmp";

                //string filename = @"D:\Data\~working\Extracted Data\data\AVA\PC\ex1mag01.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\AVA\PC\ex1mag10.tmp";
                //string filename = @"D:\Data\~working\Extracted Data\data\AVA\PC\ex1mag10w.tmp";

                //string filename = @"D:\Data\~working\Extracted Data\data\DUNGEON\statue.tmp";

                string filename = file.FullName;
                //string filename = args[0];

                Console.WriteLine(filename);

                if (File.Exists(filename))
                {
                    using (FileStream dat = File.Open(filename, FileMode.Open))
                    {
                        DotHackGU2CCSFFile tmp = new DotHackGU2CCSFFile(dat);
                        //try { DotHackGU2CCSF tmp = new DotHackGU2CCSF(dat); }
                        //catch { }

                        Console.WriteLine("----------------");

                        tmp.BlockChain.ForEach(dr =>
                        {
                            PrintNode(dr, 0);
                        });


                        //var filenames = tmp.BlockChain.Where(dr => dr.BlockType == CCSBlockType.FileNames).FirstOrDefault();
                        //CCSFileNamesBlock filenameBlock = (CCSFileNamesBlock)filenames;
                        /*int i = 0;
                        string fileOut = Path.GetFileNameWithoutExtension(filename);
                        if (tmp.BlockChain != null)
                            tmp.BlockChain
                                .Where(dr => dr != null && dr.BlockType == CCSBlockType.Mesh)
                                .ToList()
                                .ForEach(dr =>
                                {
                                    byte[] rawData = ((CCSMeshBlock)dr).RawData;

                                    File.WriteAllBytes(@".\\dump\\" + fileOut + @"[" + i.ToString() + @"].bin", rawData);

                                    i++;
                                });*/
                    }
                }

                //Console.ReadKey(true);
            }
        }

        private static void PrintNode(CCSBlock dr, int depth)
        {
            if(dr == null)
            {
                Console.WriteLine("{0}{1}UNREAD BLOCK", depth > 1 ? new string(' ', (depth - 1) * 2) : "", depth > 0 ? @"+-" : "");
                return;
            }

            Console.WriteLine("{0}{1}{2}", depth > 1 ? new string(' ', (depth - 1) * 2) : "", depth > 0 ? @"+-" : "", dr.BlockType);

            if(dr.BlockChain != null)
            {
                dr.BlockChain.ForEach(dx =>
                {
                    PrintNode(dx, depth + 1);
                });
            }

            /*switch (dr.BlockType)
            {
                case CCSBlockType.FileHeader:
                    {
                        CCSFileHeaderBlock block = (CCSFileHeaderBlock)dr;
                    }
                    break;
                case CCSBlockType.FileNames:
                    {
                        CCSFileNamesBlock block = (CCSFileNamesBlock)dr;
                    }
                    break;
                case CCSBlockType.Hierarchy:
                    {
                        CCSHierarchyBlock block = (CCSHierarchyBlock)dr;
                    }
                    break;
                case CCSBlockType.CCCC2000:
                    {
                        CCSCCCC2000Block block = (CCSCCCC2000Block)dr;
                    }
                    break;
                case CCSBlockType.CCCC0700:
                    {
                        CCSCCCC0700Block block = (CCSCCCC0700Block)dr;
                    }
                    break;
                case CCSBlockType.Pallet:
                    {
                        CCSPalletBlock block = (CCSPalletBlock)dr;
                    }
                    break;
                case CCSBlockType.Texture:
                    {
                        CCSTextureBlock block = (CCSTextureBlock)dr;
                    }
                    break;
                case CCSBlockType.CCCCFF01:
                    {
                        CCSCCCCFF01Block block = (CCSCCCCFF01Block)dr;
                    }
                    break;
                default:
                    {

                    } break;
            }*/
        }
    }
}