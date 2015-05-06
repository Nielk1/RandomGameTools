using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlightControlPack
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                //string filename = @"D:\program files (x86)\steam\steamapps\common\flight_control_hd\res\texture.pack";
                string filename = args[0];

                if(File.Exists(filename))
                {
                    using (FileStream dat = File.Open(filename, FileMode.Open))
                    {
                        PackFile tmp = new PackFile(dat);

                        if (!Directory.Exists(Path.GetFileNameWithoutExtension(filename)))
                        {
                            Directory.CreateDirectory(Path.GetFileNameWithoutExtension(filename));
                            for (int i = 0; i < tmp.Entries.Count; i++)
                            {
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
