using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PhobosDat
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length > 0)
            {
                string filename = @"D:\program files (x86)\steam\steamapps\common\1953 - KGB Unleashed\Data\common.dat";
                //string filename = args[0];

                if (File.Exists(filename))
                {
                    using (FileStream dat = File.Open(filename, FileMode.Open))
                    {
                        PhobosDatFile tmp = new PhobosDatFile(dat);

                        //tmp.Entries.ForEach(dr => Console.WriteLine("{0}", dr.Filename));
                        //Console.ReadKey(true);

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
