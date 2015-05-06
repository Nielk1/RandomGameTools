using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace DotHackGU2CVMH
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
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\~working\Extracted Data\data\");

            //FileInfo[] files = dir.GetFiles("*.tmp", SearchOption.AllDirectories);
            //foreach (var file in files)
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

                string filename = @"D:\Data\~working\Extracted Data\data\DUNGEON\statue.tmp";

                //string filename = file.FullName;
                //string filename = args[0];

                Console.WriteLine(filename);

                if (File.Exists(filename))
                {
                    using (FileStream dat = File.Open(filename, FileMode.Open))
                    {
                        DotHackGU2CCSF tmp = new DotHackGU2CCSF(dat);
                        //try { DotHackGU2CCSF tmp = new DotHackGU2CCSF(dat); }
                        //catch { }

                        //if(false)
                        //{
                        //    DotHackGU2CCSF.CCSPallet ActivePallet = null;
                        //    int filenameIndex = 0;
                        //    string[] filenames = null;
                        //    for (int x = 0; x < tmp.Entries.Count; x++)
                        //    {
                        //        if (tmp.Entries[x].GetType() == typeof(DotHackGU2CCSF.CCSFilenames))
                        //        {
                        //            filenames = ((DotHackGU2CCSF.CCSFilenames)tmp.Entries[x]).filenames.Skip(1).ToArray();
                        //        }
                        //        else if (tmp.Entries[x].GetType() == typeof(DotHackGU2CCSF.CCSPallet))
                        //        {
                        //            ActivePallet = (DotHackGU2CCSF.CCSPallet)tmp.Entries[x];
                        //        }
                        //        else if (tmp.Entries[x].GetType() == typeof(DotHackGU2CCSF.CCSTexture))
                        //        {
                        //            DotHackGU2CCSF.CCSTexture raw = (DotHackGU2CCSF.CCSTexture)tmp.Entries[x];

                        //            //var qry = ActivePallet.colors.Where((dr, i) => i % 4 == 3);
                        //            //int baseVal = qry.First();
                        //            //bool killAlpha = qry.All(dr => dr == baseVal) && baseVal == 0x80;
                        //            bool killAlpha = ActivePallet.colors.All(dr => dr.A == 0x80);

                        //            int width = 512;// (int)Math.Sqrt(raw.pixels.Length);
                        //            int height = raw.pixels.Length / width;
                        //            //if (Math.Sqrt(raw.pixels.Length) > width) height++;
                        //            Bitmap image = new Bitmap(width, height);
                        //            for (int p = 0; p < raw.pixels.Length; p++)
                        //            {
                        //                Color col = ActivePallet.colors[raw.pixels[p]];
                        //                if (killAlpha) col = Color.FromArgb(255, col);
                        //                image.SetPixel(p % width, height - 1 - p / width, col);
                        //            }

                        //            string BitmapPath = filename;
                        //            //if (textureCounter > 0) BitmapPath += @"_" + textureCounter + @"_";
                        //            //BitmapPath += @".png";
                        //            BitmapPath = exportPath + filenames[filenameIndex].TrimEnd('\0').Trim();
                        //            if (!Directory.Exists(Path.GetDirectoryName(BitmapPath))) Directory.CreateDirectory(Path.GetDirectoryName(BitmapPath));

                        //            image.Save(BitmapPath, System.Drawing.Imaging.ImageFormat.Bmp);
                        //            filenameIndex++;
                        //        }
                        //    }
                        //}

                        //tmp.Entries.ForEach(dr => Console.WriteLine("{0}\t{1}\t{2}", dr.ZSize, dr.Zipped != 0 ? 1 : 0, dr.Filename));
                        //Console.ReadKey(true);

                        //if (!Directory.Exists(Path.GetFileNameWithoutExtension(filename)))
                        //{
                        //    Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
                        //    for (int i = 0; i < tmp.Entries.Count; i++)
                        //    {
                        //        Console.WriteLine("{0}", tmp.Entries[i].Filename);
                        //
                        //        string SavePathFile = Path.GetFullPath(Path.GetFileNameWithoutExtension(filename)) + Path.DirectorySeparatorChar + tmp.Entries[i].Filename;
                        //
                        //        if (!Directory.Exists(Path.GetDirectoryName(SavePathFile))) Directory.CreateDirectory(Path.GetDirectoryName(SavePathFile));
                        //        if (!File.Exists(SavePathFile)) File.WriteAllBytes(SavePathFile, tmp.GetFile(i));
                        //    }
                        //}
                    }
                }
            }
        }
    }
}
