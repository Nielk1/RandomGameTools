using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HostileWatersTools
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> files = new List<string>();
            files.Add(@"I:\SteamLibrary\steamapps\common\Hostile Waters Antaeus Rising\Audio.mng");
            files.Add(@"I:\SteamLibrary\steamapps\common\Hostile Waters Antaeus Rising\Game.mng");
            files.Add(@"I:\SteamLibrary\steamapps\common\Hostile Waters Antaeus Rising\Movie.mng");
            files.Add(@"I:\SteamLibrary\steamapps\common\Hostile Waters Antaeus Rising\Patch.mng");

            files.ForEach(filename =>
            {
                using (FileStream dat = File.Open(filename, FileMode.Open))
                {
                    MngFile tmp = new MngFile(dat);

                    int filenameLength = tmp.Entries.Max(dr => dr.Filename.Length);
                    int offsetLength = tmp.Entries.Max(dr => dr.Offset.ToString().Length);
                    int sizeLength = tmp.Entries.Max(dr => dr.Size.ToString().Length);
                    int unknownLength = tmp.Entries.Max(dr => dr.Unknown.ToString().Length);
                    int indexLength = tmp.Entries.Count.ToString().Length;

                    for (int i = 0; i < tmp.Entries.Count; i++)
                    {
                        Console.WriteLine(
                            "{0,-" + indexLength + "} {1,-" + filenameLength + "} {2," + offsetLength + "} {3," + sizeLength + "} {4," + unknownLength + "}",
                            i + 1,
                            tmp.Entries[i].Filename,
                            tmp.Entries[i].Offset,
                            tmp.Entries[i].Size,
                            tmp.Entries[i].Unknown);
                    }

                    Console.ReadKey(true);

                    string ArchiveFilename = Path.GetFileName(filename).Replace('.', '_');
                    for (int i = 0; i < tmp.Entries.Count; i++)
                    {
                        Console.WriteLine(
                            "{0,-" + indexLength + "} {1,-" + filenameLength + "} {2," + offsetLength + "} {3," + sizeLength + "} {4," + unknownLength + "}",
                            i + 1,
                            tmp.Entries[i].Filename,
                            tmp.Entries[i].Offset,
                            tmp.Entries[i].Size,
                            tmp.Entries[i].Unknown);

                        //string[] fullPath = (ArchiveFilename + Path.DirectorySeparatorChar + tmp.Entries[i].Filename).Split(Path.DirectorySeparatorChar);
                    
                        //if (!Directory.Exists(ArchiveFilename)) Directory.CreateDirectory(ArchiveFilename);
                        string SavePathFile = ArchiveFilename + Path.DirectorySeparatorChar + tmp.Entries[i].Filename;
                        string SavePath = Path.GetDirectoryName(SavePathFile);
                        if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);

                        if (!File.Exists(SavePathFile)) File.WriteAllBytes(SavePathFile, tmp.GetFile(i));
                    }
                }
            });

            /*
            //string filename = @"I:\SteamLibrary\steamapps\common\Hostile Waters Antaeus Rising\Audio\Speech\tmungenglish.DAT";

            List<string> files = DirSearch(@"I:\SteamLibrary\steamapps\common\Hostile Waters Antaeus Rising\Audio\", "*.dat");

            files.ForEach(filename => 
            {
                string filename2 = Path.ChangeExtension(filename, "mbx");
                DatFile tmp;

                using (FileStream dat = File.Open(filename, FileMode.Open))
                {
                    tmp = new DatFile(dat);

                    int filenameLength = tmp.Entries.Max(dr => dr.Filename.Length);
                    int unknown1Length = tmp.Entries.Max(dr => dr.Size.ToString().Length);
                    int unknown2Length = tmp.Entries.Max(dr => dr.Offset.ToString().Length);
                    int indexLength = tmp.Entries.Count.ToString().Length;

                    //for (int i = 0; i < tmp.Entries.Count; i++)
                    //{
                    //    Console.WriteLine(
                    //        "{0," + indexLength + ":D" + indexLength + "} {1,-12} {2," + unknown1Length + "} {3," + unknown2Length + "}",
                    //        i + 1,
                    //        new string(tmp.Entries[i].Filename).TrimEnd('\0'),
                    //        tmp.Entries[i].Size,
                    //        tmp.Entries[i].Offset);
                    //}

                    //Console.ReadKey(true);

                    List<char> invalid = new List<char>();
                    invalid.AddRange(Path.GetInvalidFileNameChars().AsEnumerable());
                    invalid.AddRange(Path.GetInvalidPathChars().AsEnumerable());

                    for (int c = 0; c < 32; c++)
                    {
                        invalid.Add((char)c);
                    }
                    for (int c = 127; c < 256; c++)
                    {
                        invalid.Add((char)c);
                    }

                    using (FileStream mbx = File.Open(filename2, FileMode.Open))
                    {
                        string ArchiveFilename = Path.GetFileName(filename2).Replace('.', '_');
                        for (int i = 0; i < tmp.Entries.Count; i++)
                        {
                            Console.WriteLine(
                                "{0," + indexLength + ":D" + indexLength + "} {1,-12} {2," + unknown1Length + "} {3," + unknown2Length + "}",
                                i + 1,
                                new string(tmp.Entries[i].Filename).TrimEnd('\0'),
                                tmp.Entries[i].Size,
                                tmp.Entries[i].Offset);

                            //string SavePathFile = ArchiveFilename + Path.DirectorySeparatorChar + "[" + i + "]" + new string(tmp.Entries[i].Filename).TrimEnd('\0');
                            string SavePathFile = new string(tmp.Entries[i].Filename).TrimEnd('\0');
                            foreach (char c in invalid)
                            {
                                SavePathFile = SavePathFile.Replace(c.ToString(), string.Format("[{0,2:X2}]", (int)c));
                            }
                            SavePathFile = ArchiveFilename + Path.DirectorySeparatorChar + string.Format("[{0," + indexLength + ":D" + indexLength + "}]", i) + SavePathFile + @".raw";

                            string SavePath = Path.GetDirectoryName(SavePathFile);
                            if (!Directory.Exists(SavePath)) Directory.CreateDirectory(SavePath);

                            if (!File.Exists(SavePathFile))
                                using (FileStream fileOut = File.Create(SavePathFile))
                                {
                                    mbx.Seek(tmp.Entries[i].Offset, SeekOrigin.Begin);
                                    for (int x = 0; x < tmp.Entries[i].Size; x++)
                                    {
                                        fileOut.WriteByte((byte)mbx.ReadByte());
                                    }
                                }
                        }
                    }
                }
            });*/
        }

        static List<string> DirSearch(string sDir,string pattern)
        {
            List<string> retVal = new List<string>();
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d, pattern))
                    {
                        //Console.WriteLine(f);
                        retVal.Add(f);
                    }
                    retVal.AddRange(DirSearch(d, pattern).AsEnumerable());
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
            return retVal;
        }
    }
}
