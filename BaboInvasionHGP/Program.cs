using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaboInvasionHGP
{
    class Program
    {
        static void Main(string[] args)
        {
            //DirectoryInfo dir = new DirectoryInfo(@"D:\program files (x86)\steam\steamapps\common\baboinvasion\");
            //FileInfo[] files = dir.GetFiles("*.hgp", SearchOption.AllDirectories);
            //foreach (var file in files)
            if (args.Length > 0)
            {
                //string filename = @"D:\program files (x86)\steam\steamapps\common\baboinvasion\jungle_tiles.hgp";
                //string filename = file.FullName;
                string filename = args[0];

                if (File.Exists(filename))
                {
                    using (FileStream dat = File.Open(filename, FileMode.Open))
                    {
                        BaboInvasionHGPFile tmp = new BaboInvasionHGPFile(dat);

                        tmp.Entries.ForEach(dr => Console.WriteLine("{0}\t{1}\t{2}", dr.ZSize, dr.Zipped != 0? 1 : 0, dr.Filename));
                        //Console.ReadKey(true);

                        if (!Directory.Exists(Path.GetFileNameWithoutExtension(filename)))
                        {
                            Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
                            for (int i = 0; i < tmp.Entries.Count; i++)
                            {
                                Console.WriteLine("{0}", tmp.Entries[i].Filename);

                                string SavePathFile = Path.GetFullPath(Path.GetFileNameWithoutExtension(filename)) + Path.DirectorySeparatorChar + tmp.Entries[i].Filename;

                                if (!Directory.Exists(Path.GetDirectoryName(SavePathFile))) Directory.CreateDirectory(Path.GetDirectoryName(SavePathFile));
                                if (!File.Exists(SavePathFile)) File.WriteAllBytes(SavePathFile, tmp.GetFile(i));
                            }
                        }
                    }
                }
            }
        }
    }
}
