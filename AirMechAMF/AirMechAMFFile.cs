using System;
using System.Collections.Generic;
using System.IO;
//using System.IO.Compression;
using System.Linq;
using System.Text;

namespace AirMechAMF
{
	class AirMechAMFFile
	{
		public class SkeletonList : List<SkeletonNode>
		{
			public SkeletonNode Find(string name)
			{
				foreach (var bone in this)
				{
					if (bone.Name == name)
						return bone;

					var child = bone.Children.Find(name);
					if (child != null)
						return child;
				}

				return null;
			}
		}

		public class SkeletonNode
		{
			public string Name;
			public SkeletonList Children;
			public float[] Matrix;

			public SkeletonNode()
			{
				Children = new SkeletonList();
			}
		}

		public class GeoObjNode
		{
			public string Name;
			public SkeletonList Bones;
			public List<MeshObjNode> Mesh;

			public GeoObjNode()
			{
				Bones = new SkeletonList();
				Mesh = new List<MeshObjNode>();
			}
		}

		public struct VertNode
		{
			public float x;
			public float y;
			public float z;
			public float nx;
			public float ny;
			public float nz;
			public float u;
			public float v;
		}

		public struct Vector
		{
			public float x;
			public float y;
			public float z;
		}

		public class FaceData
		{
			public List<UInt16> FaceIDs;
			public UInt32? M_I;

			public FaceData()
			{
				FaceIDs = new List<ushort>();
			}
		}

		public class MeshObjNode
		{
			public List<Vector> Verts;
			public List<Vector> TVerts;
			public List<Vector> VertNormals;
			public List<Vector> TsTangents;
			public List<Vector> TsBiTangents;
			public List<Vector> TsNormals;
			public List<Vector> ShadowGeom;
			public List<VertNode> PVerts;
			public string MeshFrame;
			public List<FaceData> Faces;
			public float[] Obs;
			public float BSPHERE;
			public float[] Matrix;
			public bool ShadowMeshOnly;
			public byte[] BoneRefs;

			public MeshObjNode()
			{
				Verts = new List<Vector>();
				TVerts = new List<Vector>();
				VertNormals = new List<Vector>();
				TsTangents = new List<Vector>();
				TsBiTangents = new List<Vector>();
				TsNormals = new List<Vector>();
				ShadowGeom = new List<Vector>();
				PVerts = new List<VertNode>();
				Faces = new List<FaceData>();
				Obs = new float[4];
				Matrix = new float[16];
				ShadowMeshOnly = false;
				BoneRefs = null;
			}
		}

		public List<GeoObjNode> GEOMOBJs = new List<GeoObjNode>();

		public AirMechAMFFile(Stream data)
		{
			while (ReadSection(data)) { }

			//byte[] buffer = new byte[4];
			//string SectionType = ReadString(data);

			//data.Read(buffer, 0, 4); Version1 = BitConverter.ToUInt32(buffer, 0);
			//data.Read(buffer, 0, 4); Files = BitConverter.ToUInt32(buffer, 0);
			//data.Read(buffer, 0, 4); FirstOffset = BitConverter.ToUInt32(buffer, 0);
			//data.Read(buffer, 0, 4); Version2 = BitConverter.ToUInt32(buffer, 0);
			//data.Read(buffer, 0, 4); NTABLESIZE = BitConverter.ToUInt32(buffer, 0);
			//data.Read(buffer, 0, 4); NTABLEZSIZE = BitConverter.ToUInt32(buffer, 0);
		}

		private bool ReadSection(Stream data)
		{
			byte[] buffer = new byte[4];

			string SectionType = ReadString(data);
			if (SectionType == null) return false;
			switch (SectionType)
			{
				case "GEOMOBJ_C":
					ReadSection_GEOMOBJ_C(data);
					break;
				case "SKELETON_C":
					ReadSection_SKELETON_C(data);
					break;
				case "MESHOBJ_C":
					ReadSection_MESHOBJ_C(data);
					break;
				default:
					Console.WriteLine("Unknown section type \"{0}\"", SectionType);
					return false;
			}
			return true;
		}

		private void ReadSection_GEOMOBJ_C(Stream data)
		{
			byte[] buffer = new byte[4];
			data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);
			string Name = ReadString(data);

			//Console.WriteLine("Reading GEOMOBJ_C named \"{0}\" with {1} children", Name, Count);

			GeoObjNode GeoNode = new GeoObjNode() { Name = Name };
			GEOMOBJs.Add(GeoNode);

			for (UInt32 counter = 0; counter < Count; counter++)
			{
				ReadSection(data);
			}
		}

		private void ReadSection_SKELETON_C(Stream data)
		{
			byte[] buffer = new byte[4];

			//Console.WriteLine("Reading SKELETON_C");

			for (int nodeCount = 1; ; nodeCount++)
			{
				data.Read(buffer, 0, 4);
				if (buffer[0] == (byte)'E' && buffer[1] == (byte)'N' && buffer[2] == (byte)'D' && buffer[3] == 0x00)
				{
					break;
				}

				SkeletonList list = GEOMOBJs.Last().Bones;
				
				UInt32 Count = BitConverter.ToUInt32(buffer, 0);
				for (UInt32 counter = 0; counter < Count; counter++)
				{
					string[] BoneName = new string[nodeCount];
					for (int nodeCounter = 0; nodeCounter < nodeCount; nodeCounter++)
					{
						BoneName[nodeCounter] = ReadString(data);
					}

					float[] matrix = new float[4 * 4];
					for (int x = 0; x < matrix.Length; x++)
					{
						data.Read(buffer, 0, 4);
						matrix[x] = BitConverter.ToSingle(buffer, 0);
					}
					//Console.WriteLine("Read bone data \"{0}\"", string.Join(".", BoneName));
					//for (int x = 0; x < matrix.Length; x+=4)
					//{
					//    Console.WriteLine("{0},{1},{2},{3}", matrix[x], matrix[x + 1], matrix[x + 2], matrix[x + 3]);
					//}

					var bone = new SkeletonNode();
					bone.Name = BoneName[0];
					bone.Matrix = matrix;
					
					if (nodeCount == 1)
					{
						list.Add(bone);
					}
					else
					{
						var parent = list.Find(BoneName[1]);

						if (parent == null)
							list.Add(bone);
						else
							parent.Children.Add(bone);
					}
				}
			}
		}

		private void ReadSection_MESHOBJ_C(Stream data)
		{
			byte[] buffer = new byte[4];
			data.Read(buffer, 0, 4); UInt32 Count = BitConverter.ToUInt32(buffer, 0);

			//Console.WriteLine("Reading MESHOBJ_C with {0} children", Count);

			for (UInt32 counter = 0; counter < Count; counter++)
			{
				MeshObjNode mesh = new MeshObjNode();
				UInt32? M_I = null;

				bool reading = true;
				while (reading)
				{
					string Section = ReadString(data);

					switch (Section)
					{
						case "BSPHERE":
							{
								data.Read(buffer, 0, 4); UInt32 Unknown1 = BitConverter.ToUInt32(buffer, 0);
								Console.WriteLine("Reading BSPHERE {0,8:X8}", Unknown1);
								mesh.BSPHERE = Unknown1;
							}
							break;
						case "OBS":
							{
								//float[] quat = new float[4];
								for (int x = 0; x < mesh.Obs/*quat*/.Length; x++)
								{
									data.Read(buffer, 0, 4);
									mesh.Obs[x]/*quat[x]*/ = BitConverter.ToSingle(buffer, 0);
								}
								//Console.WriteLine("Reading OBS {0}", string.Join(", ", quat));
							}
							break;
						case "P_VERTS":
							{
								data.Read(buffer, 0, 4); UInt32 VRows = BitConverter.ToUInt32(buffer, 0);
								data.Read(buffer, 0, 4); UInt32 VCols = BitConverter.ToUInt32(buffer, 0);
								data.Read(buffer, 0, 4); UInt32 VUnk = BitConverter.ToUInt32(buffer, 0);

								//data.Seek(VRows * VCols, SeekOrigin.Current);
								for (int row = 0; row < VRows; row++)
								{
									float[] vals = new float[VCols / 4];
									for (int col = 0; col < (VCols / 4); col++)
									{
										data.Read(buffer, 0, 4);
										vals[col] = BitConverter.ToSingle(buffer, 0);
									}
									//                                    Console.WriteLine(string.Join("\t", vals));

									mesh.PVerts.Add(new VertNode()
									{
										x = vals.Length > 0 ? vals[0] : 0.0f,
										y = vals.Length > 1 ? vals[1] : 0.0f,
										z = vals.Length > 2 ? vals[2] : 0.0f,
										nx = vals.Length > 3 ? vals[3] : 0.0f,
										ny = vals.Length > 4 ? vals[4] : 0.0f,
										nz = vals.Length > 5 ? vals[5] : 0.0f,
										u = vals.Length > 6 ? vals[6] : 0.0f,
										v = vals.Length > 7 ? vals[7] : 0.0f,
									});
								}

								//Console.WriteLine("Reading P_VERTS {0} rows, {1} cols, {2} unknowns of vertex data", VRows, VCols, VUnk);
							}
							break;
						case "VERTS":
							{
								data.Read(buffer, 0, 4); UInt32 VRows = BitConverter.ToUInt32(buffer, 0);

								//data.Seek(VRows * VCols, SeekOrigin.Current);
								for (int row = 0; row < VRows; row++)
								{
									data.Read(buffer, 0, 4);
									float x = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float y = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float z = BitConverter.ToSingle(buffer, 0);

									mesh.Verts.Add(new Vector()
									{
										x = x,
										y = y,
										z = z
									});
								}

								//Console.WriteLine("Reading VERTS {0} rows of vertex data", VRows);
							}
							break;
						case "TVERTS":
							{
								data.Read(buffer, 0, 4); UInt32 VRows = BitConverter.ToUInt32(buffer, 0);

								//data.Seek(VRows * VCols, SeekOrigin.Current);
								for (int row = 0; row < VRows; row++)
								{
									data.Read(buffer, 0, 4);
									float x = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float y = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float z = BitConverter.ToSingle(buffer, 0);

									mesh.TVerts.Add(new Vector()
									{
										x = x,
										y = y,
										z = z
									});
								}

								//Console.WriteLine("Reading TVERTS {0} rows of vertex uv data", VRows);
							}
							break;
						case "VERTNORMALS":
							{
								data.Read(buffer, 0, 4); UInt32 VRows = BitConverter.ToUInt32(buffer, 0);

								//data.Seek(VRows * VCols, SeekOrigin.Current);
								for (int row = 0; row < VRows; row++)
								{
									data.Read(buffer, 0, 4);
									float x = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float y = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float z = BitConverter.ToSingle(buffer, 0);

									mesh.VertNormals.Add(new Vector()
									{
										x = x,
										y = y,
										z = z
									});
								}

								//Console.WriteLine("Reading VERTNORMALS {0} rows of vertex uv data", VRows);
							}
							break;
						case "TS_TANGENTS":
							{
								data.Read(buffer, 0, 4); UInt32 VRows = BitConverter.ToUInt32(buffer, 0);

								//data.Seek(VRows * VCols, SeekOrigin.Current);
								for (int row = 0; row < VRows; row++)
								{
									data.Read(buffer, 0, 4);
									float x = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float y = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float z = BitConverter.ToSingle(buffer, 0);

									mesh.TsTangents.Add(new Vector()
									{
										x = x,
										y = y,
										z = z
									});
								}

								//Console.WriteLine("Reading TS_TANGENTS {0} rows of vertex uv data", VRows);
							}
							break;
						case "TS_BITANGENTS":
							{
								data.Read(buffer, 0, 4); UInt32 VRows = BitConverter.ToUInt32(buffer, 0);

								//data.Seek(VRows * VCols, SeekOrigin.Current);
								for (int row = 0; row < VRows; row++)
								{
									data.Read(buffer, 0, 4);
									float x = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float y = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float z = BitConverter.ToSingle(buffer, 0);

									mesh.TsBiTangents.Add(new Vector()
									{
										x = x,
										y = y,
										z = z
									});
								}

								//Console.WriteLine("Reading TS_BITANGENTS {0} rows of vertex uv data", VRows);
							}
							break;
						case "TS_NORMALS":
							{
								data.Read(buffer, 0, 4); UInt32 VRows = BitConverter.ToUInt32(buffer, 0);

								//data.Seek(VRows * VCols, SeekOrigin.Current);
								for (int row = 0; row < VRows; row++)
								{
									data.Read(buffer, 0, 4);
									float x = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float y = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float z = BitConverter.ToSingle(buffer, 0);

									mesh.TsNormals.Add(new Vector()
									{
										x = x,
										y = y,
										z = z
									});
								}

								//Console.WriteLine("Reading TS_NORMALS {0} rows of vertex uv data", VRows);
							}
							break;
						case "FACEGROUPS":
							{
								data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
								Console.WriteLine("Reading FACEGROUPS {0,8:X8}", Unknown);
							}
							break;
						case "M_I":
							{
								data.Read(buffer, 0, 4); M_I = BitConverter.ToUInt32(buffer, 0);
								Console.WriteLine("Reading M_I {0,8:X8}", M_I);
								//mesh.Faces.Add(new List<UInt16>());
							}
							break;
						case "FACES_T":
							{
								mesh.Faces.Add(new FaceData() { M_I = M_I });

								data.Read(buffer, 0, 4); UInt32 FaceCount = BitConverter.ToUInt32(buffer, 0);
								if (FaceCount != 0)
								{
									data.Read(buffer, 0, 4); UInt32 Unknown2 = BitConverter.ToUInt32(buffer, 0);
									data.Read(buffer, 0, 4); UInt32 VertCount = BitConverter.ToUInt32(buffer, 0);
									//Console.WriteLine("Reading FACES_T {0}", VertCount);
									UInt16[] faceverts = new UInt16[VertCount];
									for (int x = 0; x < VertCount; x++)
									{
										data.Read(buffer, 0, 2); UInt16 v1 = BitConverter.ToUInt16(buffer, 0);
										faceverts[x] = v1;
									}
									Console.WriteLine("Reading FACES_T {0,8:X8} {1,8:X8} {2,8:X8}", FaceCount, Unknown2, VertCount);

									mesh.Faces.Last().FaceIDs.AddRange(faceverts);
								}
								//                                Console.WriteLine("{0}", string.Join(", ", faceverts));
							}
							break;
						case "FACES":
							{
								mesh.Faces.Add(new FaceData() { M_I = null });

								data.Read(buffer, 0, 4); UInt32 VertCount = BitConverter.ToUInt32(buffer, 0);
								//Console.WriteLine("Reading FACES_T {0}", VertCount);
								UInt16[] faceverts = new UInt16[VertCount * 3];
								for (int x = 0; x < VertCount; x++)
								{
									data.Read(buffer, 0, 2); UInt16 v1 = BitConverter.ToUInt16(buffer, 0);
									data.Read(buffer, 0, 2); UInt16 v2 = BitConverter.ToUInt16(buffer, 0);
									data.Read(buffer, 0, 2); UInt16 v3 = BitConverter.ToUInt16(buffer, 0);
									faceverts[(x * 3) + 0] = v1;
									faceverts[(x * 3) + 1] = v2;
									faceverts[(x * 3) + 2] = v3;
								}
								Console.WriteLine("Reading FACES {0,8:X8}", VertCount);

								mesh.Faces.Last().FaceIDs.AddRange(faceverts);
								//                                Console.WriteLine("{0}", string.Join(", ", faceverts));
							}
							break;
						case "SHADOW_MESH_ONLY":
							//Console.WriteLine("Reading SHADOW_MESH_ONLY");
							mesh.ShadowMeshOnly = true;
							break;
						case "SHADOW_GEOMETRY":
							{
								data.Read(buffer, 0, 4); UInt32 VRows = BitConverter.ToUInt32(buffer, 0);

								//data.Seek(VRows * VCols, SeekOrigin.Current);
								for (int row = 0; row < VRows; row++)
								{
									data.Read(buffer, 0, 4);
									float x = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float y = BitConverter.ToSingle(buffer, 0);
									data.Read(buffer, 0, 4);
									float z = BitConverter.ToSingle(buffer, 0);

									mesh.ShadowGeom.Add(new Vector()
									{
										x = x,
										y = y,
										z = z
									});
								}

								//Console.WriteLine("Reading SHADOW_GEOMETRY {0} rows of vertex uv data", VRows);
							}
							break;
						case "BONEREFS":
							{
								//Console.Write("Reading BONEREFS ");
								int count = data.ReadByte();
								mesh.BoneRefs = new byte[count];
								for (int x = 0; x < count; x++)
								{
									byte hex = (byte)data.ReadByte();
									//Console.Write("{0,2:X2}", hex);
									mesh.BoneRefs[x] = hex;
								}
								//Console.WriteLine();
							}
							break;
						case "SKINWEIGHTGROUPS":
							{
								data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
								Console.WriteLine("Reading SKINWEIGHTGROUPS {0,8:X8}", Unknown); // never not 0, for now...
							}
							break;
						//case "GHTGROUPS":
						//    {
						//        data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
						//        Console.WriteLine("Reading GHTGROUPS {0,8:X8}", Unknown);
						//    }
						//    break;
						//case "KINWEIGHTGROUPS":
						//    {
						//        data.Read(buffer, 0, 4); UInt32 Unknown = BitConverter.ToUInt32(buffer, 0);
						//        Console.WriteLine("Reading KINWEIGHTGROUPS {0,8:X8}", Unknown);
						//    }
						//    break;
						case "MESHTRANSFORM":
							{
								//Console.WriteLine("Reading MESHTRANSFORM");
								//
								//for (int i = 0; i < 4; i++)
								//{
								//    for (int j = 0; j < 4; j++)
								//    {
								//        data.Read(buffer, 0, 4);
								//        float x = BitConverter.ToSingle(buffer, 0);
								//        Console.Write("{0}, ", x);
								//    }
								//    Console.WriteLine();
								//}

								for (int i = 0; i < 16; i++)
								{
									data.Read(buffer, 0, 4);
									mesh.Matrix[i] = BitConverter.ToSingle(buffer, 0);
								}
							}
							break;
						case "MESHFRAME":
							{
								string name = ReadString(data);
								//Console.WriteLine("Reading MESHFRAME {0}", name);

								mesh.MeshFrame = name;
							}
							break;
						case "END":
							{
								reading = false;
							}
							break;
						default:
							throw new Exception(string.Format("Unknown Mesh Bit \"{0}\"", Section));
					}
				}

				GEOMOBJs.Last().Mesh.Add(mesh);
			}
		}

		private string ReadString(Stream data)
		{
			if (data.Position < data.Length)
			{
				StringBuilder filename = new StringBuilder();
				byte read = 0x00;
				do
				{
					read = (byte)data.ReadByte();
					if (read != 0x00) filename.Append((char)read);
				} while (read != 0x00);
				return filename.ToString();
			}
			else
			{
				return null;
			}
		}
	}
}
