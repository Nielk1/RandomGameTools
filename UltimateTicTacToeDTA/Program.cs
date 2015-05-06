using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UltimateTicTacToeDTA
{
    class Program
    {
        static void Main(string[] args)
        {
            //DirectoryInfo dir = new DirectoryInfo(@"D:\program files (x86)\steam\steamapps\common\baboinvasion\");
            //FileInfo[] files = dir.GetFiles("*.hgp", SearchOption.AllDirectories);
            //foreach (var file in files)
            //if (args.Length > 0)
            {
                string filename = @"I:\SteamLibrary\steamapps\common\Ultimate Tic-Tac-Toe\UltimTicTacToe.dta";
                //string filename = file.FullName;
                //string filename = args[0];

                if (File.Exists(filename))
                {
                    using (FileStream dat = File.Open(filename, FileMode.Open))
                    {
                        UltimateTicTacToeDTAFile tmp = new UltimateTicTacToeDTAFile(dat);

                        //tmp.Entries.ForEach(dr => Console.WriteLine("{0}\t{1}\t{2}", dr.ZSize, dr.Zipped != 0 ? 1 : 0, dr.Filename));
                        //Console.ReadKey(true);

                        for(int i=0;i<tmp.CountFiles();i++)
                        {
                            string outputFilename = string.Format("{0}_{1}",Path.GetFileNameWithoutExtension(filename),i);
                            byte[] data = tmp.GetFile(i);
                            if((data[0] == 0x89)
                            && (data[1] == 0x50)
                            && (data[2] == 0x4e)
                            && (data[3] == 0x47)
                            && (data[4] == 0x0d)
                            && (data[5] == 0x0a)
                            && (data[6] == 0x1a)
                            && (data[7] == 0x0a))
                            {
                                outputFilename += ".png";
                            }
                            else
                            {
                                outputFilename += ".bin";
                            }
                            File.WriteAllBytes(outputFilename, data);
                        }

                        /*if (!Directory.Exists(Path.GetFileNameWithoutExtension(filename)))
                        {
                            Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
                            for (int i = 0; i < tmp.Entries.Count; i++)
                            {
                                Console.WriteLine("{0}", tmp.Entries[i].Filename);

                                string SavePathFile = Path.GetFullPath(Path.GetFileNameWithoutExtension(filename)) + Path.DirectorySeparatorChar + tmp.Entries[i].Filename;

                                if (!Directory.Exists(Path.GetDirectoryName(SavePathFile))) Directory.CreateDirectory(Path.GetDirectoryName(SavePathFile));
                                if (!File.Exists(SavePathFile)) File.WriteAllBytes(SavePathFile, tmp.GetFile(i));
                            }
                        }*/
                    }
                }
            }
        }
    }
}
