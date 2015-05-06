using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ObulisPK2Extract
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length > 0)
            {
                string filename = @"D:\Program Files (x86)\Steam\steamapps\common\Obulis\obulis.pk2";
                //string filename = args[0];

                if (File.Exists(filename))
                {
                    using (FileStream dat = File.Open(filename, FileMode.Open))
                    {
                        Pk2File tmp = new Pk2File(dat);

                        //tmp.Entries.ForEach(dr => Console.WriteLine("{0}\t{1}", new string(dr.Magic), dr.Filename.TrimEnd('\0')));
                        //Console.ReadKey(true);

                        if (!Directory.Exists(Path.GetFileNameWithoutExtension(filename)))
                        {
                            Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
                            Console.WriteLine("Unknown\t\tOffset\tSize\tMagic\tFilename");
                            for (int i = 0; i < tmp.Entries.Count; i++)
                            {
                                Console.WriteLine("{2,8:X8}\t{3}\t{4}\t{0}\t{1}", new string(tmp.Entries[i].Magic), tmp.Entries[i].Filename.TrimEnd('\0'), tmp.Entries[i].Unknown1, tmp.Entries[i].Offset, tmp.Entries[i].Size);

                                string SavePathFile = Path.GetFullPath(Path.GetFileNameWithoutExtension(filename)) + Path.DirectorySeparatorChar + tmp.Entries[i].Filename.TrimEnd('\0') + @"." + new string(tmp.Entries[i].Magic);
                        
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
