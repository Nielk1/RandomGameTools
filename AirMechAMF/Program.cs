using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AirMechAMF
{
	internal class Program
	{
		private static void Main(string[] args)
		{
            //DirectoryInfo dir = new DirectoryInfo(@"D:\Data\Subject Collections\Minor Games Resources\AirMech\Data Extraction\rook\");
            DirectoryInfo dir = new DirectoryInfo(@"D:\Data\Subject Collections\Minor Games Resources\AirMech\Data Extraction\");
			FileInfo[] files = dir.GetFiles("*.amf", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				Console.WriteLine(file.FullName);
				using (FileStream dat = File.Open(file.FullName, FileMode.Open))
				{
					AirMechAMFFile tmp;
					try
					{
						tmp = new AirMechAMFFile(dat);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						continue;
					}

					using (FileStream collada = new FileStream(file.FullName + ".dae", FileMode.Create, FileAccess.Write))
						Collada.Export(tmp, collada);
				}
			}
		}
	}
}