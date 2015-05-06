using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace AirMechAMF
{
	internal static class Collada
	{
		public static void Export(AirMechAMFFile file, Stream dest)
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture; // need this if you have a different number format (such as germany >_>)

			XmlNode collada = new XmlNode("COLLADA");
			collada.Attributes["version"] = "1.5.0";

			// up axis is Z in this game, I think.
			XmlNode asset = collada.Add("asset");
			asset.Add("up_axis", "Z_UP");

			// add some dummy effects and materials.
			XmlNode libraryEffects = collada.Add("library_effects");
			XmlNode effect = libraryEffects.Add("effect");
			effect.Attributes["id"] = "dummy_effect";
			XmlNode profileCommon = effect.Add("profile_COMMON");
			XmlNode technique = profileCommon.Add("technique");
			technique.Attributes["sid"] = "COMMON";
			XmlNode blinn = technique.Add("blinn");
			blinn.Add("diffuse").Add("color", "0.8 0.8 0.8 1.0");
			blinn.Add("specular").Add("color", "0.2 0.2 0.2 1.0");
			blinn.Add("shininess").Add("float", "0.5");

			XmlNode libraryMaterials = collada.Add("library_materials");
			XmlNode material = libraryMaterials.Add("material");
			material.Attributes["id"] = "dummy_material";
			material.Attributes["name"] = "whatever";
			XmlNode instanceEffect = material.Add("instance_effect");
			instanceEffect.Attributes["url"] = "#dummy_effect";

			// add geometry and scene.
			XmlNode libraryGeometries = collada.Add("library_geometries");

			XmlNode libraryVisualScenes = collada.Add("library_visual_scenes");
			XmlNode visualScene = libraryVisualScenes.Add("visual_scene");
			visualScene.Attributes["id"] = "airmech";

			XmlNode scene = collada.Add("scene");
			XmlNode instanceVisualScene = scene.Add("instance_visual_scene");
			instanceVisualScene.Attributes["url"] = "#airmech";

			for (int i = 0; i < file.GEOMOBJs.Count; i++)
			{
				var geom = file.GEOMOBJs[i];
				XmlNode parentNode = visualScene.Add("node");
				parentNode.Attributes["id"] = string.Format("geom_{0}", i);
				parentNode.Attributes["name"] = geom.Name;
				parentNode.Attributes["type"] = "NODE";

				addBones(parentNode, null, geom.Bones);

				for (int j = 0; j < geom.Mesh.Count; j++)
				{
					var geomMesh = geom.Mesh[j];

					XmlNode geometry = libraryGeometries.Add("geometry");
					geometry.Attributes["id"] = string.Format("geom_{0}_mesh_{1}", i, j);
					XmlNode mesh = geometry.Add("mesh");

					XmlNode positionSource = mesh.Add("source");
					positionSource.Attributes["id"] = string.Format("geom_{0}_positions_{1}", i, j);
					XmlNode positionFloatArray = positionSource.Add("float_array");
					positionFloatArray.Attributes["id"] = string.Format("geom_{0}_positions_{1}_data", i, j);
					positionFloatArray.Attributes["count"] = (geomMesh.PVerts.Count * 3).ToString();

					XmlNode techniqueCommon = positionSource.Add("technique_common");
					XmlNode accessor = techniqueCommon.Add("accessor");
					accessor.Attributes["count"] = geomMesh.PVerts.Count.ToString();
					accessor.Attributes["stride"] = "3";
					accessor.Attributes["source"] = "#" + positionFloatArray.Attributes["id"];
					XmlNode param = accessor.Add("param");
					param.Attributes["name"] = "X";
					param.Attributes["type"] = "float";
					param = accessor.Add("param");
					param.Attributes["name"] = "Y";
					param.Attributes["type"] = "float";
					param = accessor.Add("param");
					param.Attributes["name"] = "Z";
					param.Attributes["type"] = "float";


					XmlNode normalSource = mesh.Add("source");
					normalSource.Attributes["id"] = string.Format("geom_{0}_normals_{1}", i, j);
					XmlNode normalFloatArray = normalSource.Add("float_array");
					normalFloatArray.Attributes["id"] = string.Format("geom_{0}_normals_{1}_data", i, j);
					normalFloatArray.Attributes["count"] = (geomMesh.PVerts.Count * 3).ToString();

					techniqueCommon = normalSource.Add("technique_common");
					accessor = techniqueCommon.Add("accessor");
					accessor.Attributes["count"] = geomMesh.PVerts.Count.ToString();
					accessor.Attributes["stride"] = "3";
					accessor.Attributes["source"] = "#" + normalFloatArray.Attributes["id"];
					param = accessor.Add("param");
					param.Attributes["name"] = "X";
					param.Attributes["type"] = "float";
					param = accessor.Add("param");
					param.Attributes["name"] = "Y";
					param.Attributes["type"] = "float";
					param = accessor.Add("param");
					param.Attributes["name"] = "Z";
					param.Attributes["type"] = "float";


					XmlNode texCoordSource = mesh.Add("source");
					texCoordSource.Attributes["id"] = string.Format("geom_{0}_texcoords_{1}", i, j);
					XmlNode texCoordFloatArray = texCoordSource.Add("float_array");
					texCoordFloatArray.Attributes["id"] = string.Format("geom_{0}_texcoords_{1}_data", i, j);
					texCoordFloatArray.Attributes["count"] = (geomMesh.PVerts.Count * 2).ToString();

					techniqueCommon = texCoordSource.Add("technique_common");
					accessor = techniqueCommon.Add("accessor");
					accessor.Attributes["count"] = geomMesh.PVerts.Count.ToString();
					accessor.Attributes["stride"] = "2";
					accessor.Attributes["source"] = "#" + texCoordFloatArray.Attributes["id"];
					param = accessor.Add("param");
					param.Attributes["name"] = "S";
					param.Attributes["type"] = "float";
					param = accessor.Add("param");
					param.Attributes["name"] = "T";
					param.Attributes["type"] = "float";


					StringBuilder positionData = new StringBuilder();
					StringBuilder normalData = new StringBuilder();
					StringBuilder texCoordData = new StringBuilder();
					for (int k = 0; k < geomMesh.PVerts.Count; k++)
					{
						positionData.Append(geomMesh.PVerts[k].x); positionData.Append(' ');
						positionData.Append(geomMesh.PVerts[k].y); positionData.Append(' ');
						positionData.Append(geomMesh.PVerts[k].z); positionData.Append(' ');

						normalData.Append(geomMesh.PVerts[k].nx); normalData.Append(' ');
						normalData.Append(geomMesh.PVerts[k].ny); normalData.Append(' ');
						normalData.Append(geomMesh.PVerts[k].nz); normalData.Append(' ');

						texCoordData.Append(geomMesh.PVerts[k].u); texCoordData.Append(' ');
						texCoordData.Append(geomMesh.PVerts[k].v); texCoordData.Append(' ');
					}
					positionFloatArray.Value = positionData.ToString();
					normalFloatArray.Value = normalData.ToString();
					texCoordFloatArray.Value = texCoordData.ToString();

					XmlNode vertices = mesh.Add("vertices");
					vertices.Attributes["id"] = string.Format("geom_{0}_vertices_{1}", i, j);
					XmlNode input = vertices.Add("input");
					input.Attributes["semantic"] = "POSITION";
					input.Attributes["source"] = "#" + positionSource.Attributes["id"];

					for (int k = 0; k < geomMesh.Faces.Count; k++)
					{
						var faceIDs = geomMesh.Faces[k].FaceIDs;
						if (faceIDs.Count == 0) // find out why this happens maybe
							continue;

						XmlNode triangles = mesh.Add("triangles");
						triangles.Attributes["count"] = (faceIDs.Count / 3).ToString();
						triangles.Attributes["material"] = "m1";
						input = triangles.Add("input");
						input.Attributes["offset"] = "0";
						input.Attributes["semantic"] = "VERTEX";
						input.Attributes["source"] = "#" + vertices.Attributes["id"];
						input = triangles.Add("input");
						input.Attributes["offset"] = "0";
						input.Attributes["semantic"] = "NORMAL";
						input.Attributes["source"] = "#" + normalSource.Attributes["id"];
						input = triangles.Add("input");
						input.Attributes["offset"] = "0";
						input.Attributes["semantic"] = "TEXCOORD";
						input.Attributes["source"] = "#" + texCoordSource.Attributes["id"];

						XmlNode p = triangles.Add("p");

						StringBuilder pData = new StringBuilder();
						for (int v = 0; v < faceIDs.Count; v++)
						{
							pData.Append(faceIDs[v]);
							pData.Append(' ');
						}
						p.Value = pData.ToString();
					}

					XmlNode node = parentNode.Add("node");
					node.Attributes["id"] = string.Format("geom_{0}_node_{1}", i, j);
					node.Attributes["name"] = geomMesh.MeshFrame;
					node.Attributes["type"] = "NODE";

					// collada stores matrices column-major...
					XmlNode matrix = node.Add("matrix");
					matrix.Attributes["sid"] = "transform";
					float[] m = getTransform(geom.Bones, geomMesh.MeshFrame);
					for (int k = 0; k < 16; k++)
						matrix.Value += m[k] + " ";


					XmlNode instanceGeometry = node.Add("instance_geometry");
					instanceGeometry.Attributes["url"] = "#" + geometry.Attributes["id"];
					XmlNode bindMaterial = instanceGeometry.Add("bind_material");
					techniqueCommon = bindMaterial.Add("technique_common");
					XmlNode instanceMaterial = techniqueCommon.Add("instance_material");
					instanceMaterial.Attributes["symbol"] = "m1";
					instanceMaterial.Attributes["target"] = "#dummy_material";
					XmlNode bindVertexInput = instanceMaterial.Add("bind_vertex_input");
					bindVertexInput.Attributes["semantic"] = "UVSET0";
					bindVertexInput.Attributes["input_semantic"] = "TEXCOORD";
					bindVertexInput.Attributes["input_set"] = "0";
				}
			}

			collada.WriteXml(dest);
		}

		private static void addBones(XmlNode parentNode, float[] transform, IEnumerable<AirMechAMFFile.SkeletonNode> bones)
		{
			/*
			float[] inverse;
			if (transform == null)
				inverse = new[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f};
			else
				inverse = matrixInverse(transform);	
			*/

			foreach (var bone in bones)
			{
				XmlNode node = parentNode.Add("node");
				node.Attributes["name"] = bone.Name;
				node.Attributes["type"] = "JOINT";
				node.Attributes["id"] = bone.Name; // not sure if all names will contain valid characters.

				// collada stores matrices column-major...
				float[] m;
				if (bone.Matrix == null)
					m = new[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f};
				else
					m = matrixTranspose(bone.Matrix);

				float[] relative = m; // matrixMult(inverse, m);
				XmlNode matrix = node.Add("matrix");
				matrix.Attributes["sid"] = "transform";
				for (int i = 0; i < 16; i++)
					matrix.Value += relative[i] + " ";

				if (bone.Children.Count > 0)
					addBones(node, m, bone.Children);
			}
		}

		private static float[] getTransform(AirMechAMFFile.SkeletonList bones, string name)
		{
			var bone = bones.Find(name);
			if (bone == null)
				return new[] {1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f};

			return matrixTranspose(bone.Matrix);
		}

		private static float[] matrixMult(float[] a, float[] b)
		{
			float[] result = new float[16];
			for (int i = 0; i < 16; i++)
			{
				int x = i & 3;
				int y = (i >> 2) << 2;
				result[i] = (a[y] * b[x]) + (a[y + 1] * b[x + 4]) + (a[y + 2] * b[x + 8]) + (a[y + 3] * b[x + 12]);
			}
			return result;
		}

		private static float[] matrixTranspose(float[] m)
		{
			float[] transposed = new float[16];
			for (int i = 0; i < 4; i++)
				for (int j = 0; j < 4; j++)
					transposed[i * 4 + j] = m[i + j * 4];
			return transposed;
		}

		private static float[] matrixInverse(float[] matrix)
		{
			float[] result = new float[16];
			
			// 2x2 determinants.
			float s0 = matrix[0] * matrix[5] - matrix[1] * matrix[4];
			float s1 = matrix[0] * matrix[9] - matrix[1] * matrix[8];
			float s2 = matrix[0] * matrix[13] - matrix[1] * matrix[12];
			float s3 = matrix[4] * matrix[9] - matrix[5] * matrix[8];
			float s4 = matrix[4] * matrix[13] - matrix[5] * matrix[12];
			float s5 = matrix[8] * matrix[13] - matrix[9] * matrix[12];
			float c5 = matrix[10] * matrix[15] - matrix[11] * matrix[14];
			float c4 = matrix[6] * matrix[15] - matrix[7] * matrix[14];
			float c3 = matrix[6] * matrix[11] - matrix[7] * matrix[10];
			float c2 = matrix[2] * matrix[15] - matrix[3] * matrix[14];
			float c1 = matrix[2] * matrix[11] - matrix[3] * matrix[10];
			float c0 = matrix[2] * matrix[7] - matrix[3] * matrix[6];

			// 4x4 determinant.
			float d = (s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0);
			if (d >= -float.Epsilon && d <= float.Epsilon)
				return result; // invalid matrix.
			d = 1.0f / d;

			// invert matrix.
			result[0] = (matrix[5] * c5 - matrix[9] * c4 + matrix[13] * c3) * d;
			result[4] = (-matrix[4] * c5 + matrix[8] * c4 - matrix[12] * c3) * d;
			result[8] = (matrix[7] * s5 - matrix[11] * s4 + matrix[15] * s3) * d;
			result[12] = (-matrix[6] * s5 + matrix[10] * s4 - matrix[14] * s3) * d;

			result[1] = (-matrix[1] * c5 + matrix[9] * c2 - matrix[13] * c1) * d;
			result[5] = (matrix[0] * c5 - matrix[8] * c2 + matrix[12] * c1) * d;
			result[9] = (-matrix[3] * s5 + matrix[11] * s2 - matrix[15] * s1) * d;
			result[13] = (matrix[2] * s5 - matrix[10] * s2 + matrix[14] * s1) * d;

			result[2] = (matrix[1] * c4 - matrix[5] * c2 + matrix[13] * c0) * d;
			result[6] = (-matrix[0] * c4 + matrix[4] * c2 - matrix[12] * c0) * d;
			result[10] = (matrix[3] * s4 - matrix[7] * s2 + matrix[15] * s0) * d;
			result[14] = (-matrix[2] * s4 + matrix[6] * s2 - matrix[14] * s0) * d;

			result[3] = (-matrix[1] * c3 + matrix[5] * c1 - matrix[9] * c0) * d;
			result[7] = (matrix[0] * c3 - matrix[4] * c1 + matrix[8] * c0) * d;
			result[11] = (-matrix[3] * s3 + matrix[7] * s1 - matrix[11] * s0) * d;
			result[15] = (matrix[2] * s3 - matrix[6] * s1 + matrix[10] * s0) * d;

			return result;
		}
	}

	/// <summary>
	/// Respresents a simple XML node with a name, a value and attributes.
	/// It can have multiple child nodes, thus creating a tree.
	/// </summary>
	public sealed class XmlNode : List<XmlNode>
	{
		#region Fields

		private string name;
		private string value;
		private Dictionary<string, string> attributes;

		/// <summary>
		/// Settings for the XML reader.
		/// </summary>
		private static readonly XmlReaderSettings readerSettings = new XmlReaderSettings
		{
			CloseInput = false
		};

		/// <summary>
		/// Settings for the XML reader.
		/// </summary>
		private static readonly XmlWriterSettings writerSettings = new XmlWriterSettings
		{
			Indent = true,
			IndentChars = "\t",
			NewLineChars = "\n",
			NewLineOnAttributes = false,
			CloseOutput = false,
		};

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the name of the element.
		/// </summary>
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		/// <summary>
		/// Gets or sets the value of the element.
		/// </summary>
		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		/// <summary>
		/// Gets or sets the attributes of the element.
		/// </summary>
		public Dictionary<string, string> Attributes
		{
			get { return this.attributes; }
			set { this.attributes = value; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new XmlNode.
		/// </summary>
		public XmlNode()
			: this(null)
		{

		}

		/// <summary>
		/// Creates a new XmlNode.
		/// </summary>
		public XmlNode(string name)
			: this(name, null)
		{

		}

		/// <summary>
		/// Creates a new XmlNode.
		/// </summary>
		public XmlNode(string name, string value)
		{
			this.name = name;
			this.value = value;
			this.attributes = new Dictionary<string, string>();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a new node with the specified name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public XmlNode Add(string name)
		{
			XmlNode node = new XmlNode(name);
			this.Add(node);
			return node;
		}

		/// <summary>
		/// Adds a new node with the specified name and value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public XmlNode Add(string name, string value)
		{
			XmlNode node = new XmlNode(name, value);
			this.Add(node);
			return node;
		}

		/// <summary>
		/// Returns the first element with the specified name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public XmlNode GetElement(string name)
		{
			foreach (XmlNode element in this)
			{
				if (element.name == name)
					return element;
			}

			return null;
		}

		/// <summary>
		/// Returns all element with the specified name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IEnumerable<XmlNode> GetElements(string name)
		{
			foreach (XmlNode element in this)
			{
				if (element.name == name)
					yield return element;
			}
		}

		/// <summary>
		/// Returns the value of an attribute.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public string GetAttribute(string name, string defaultValue = null)
		{
			string value;
			if (!this.attributes.TryGetValue(name, out value))
				return defaultValue;

			return value;
		}

		/// <summary>
		/// Parses an attribute.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public T ParseAttribute<T>(string name, T defaultValue = default(T)) where T : struct
		{
			string str = this.GetAttribute(name);
			if (str == null)
				return defaultValue;

			Type type = typeof(T);
			if (type.IsEnum)
			{
				T value = default(T);
				if (!Enum.TryParse(str, out value))
					throw new Exception(string.Format("Could not parse attribute \"{0}\" as {1}.", str, type.Name));

				return value;
			}

			if (type == typeof(sbyte)) return (T)(object)sbyte.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(byte)) return (T)(object)byte.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(short)) return (T)(object)short.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(ushort)) return (T)(object)ushort.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(int)) return (T)(object)int.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(uint)) return (T)(object)uint.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(long)) return (T)(object)long.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(ulong)) return (T)(object)ulong.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(float)) return (T)(object)float.Parse(str, CultureInfo.InvariantCulture);
			if (type == typeof(double)) return (T)(object)double.Parse(str, CultureInfo.InvariantCulture);

			throw new NotSupportedException(string.Format("Type {0} is not supported.", type.Name));
		}

		public override string ToString()
		{
			StringBuilder str = new StringBuilder();

			str.Append(this.name);

			if (this.attributes.Count > 0)
			{
				bool first = true;

				foreach (KeyValuePair<string, string> attribute in this.attributes)
				{
					if (first)
						first = false;
					else
						str.Append(", ");

					str.Append(attribute.Key);
					str.Append(" = \"");
					str.Append(attribute.Value);
					str.Append("\"");
				}
			}

			return str.ToString();
		}

		#region ReadXml

		/// <summary>
		/// Reads a data element tree from the specified stream in the XML format.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static XmlNode ReadXml(Stream stream)
		{
			using (XmlReader xmlReader = XmlReader.Create(stream, readerSettings))
			{
				return readXml(xmlReader);
			}
		}

		/// <summary>
		/// Reads a data element tree from the specified text reader in the XML format.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static XmlNode ReadXml(TextReader reader)
		{
			using (XmlReader xmlReader = XmlReader.Create(reader, readerSettings))
			{
				return readXml(xmlReader);
			}
		}

		/// <summary>
		/// Reads a data element tree from the specified XML reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		private static XmlNode readXml(XmlReader reader)
		{
			while (reader.NodeType != XmlNodeType.Element)
				reader.Read();

			XmlNode element = new XmlNode();
			element.Name = reader.Name;

			if (reader.MoveToFirstAttribute())
			{
				do
				{
					element.attributes[reader.Name] = reader.Value;
				} while (reader.MoveToNextAttribute());
				reader.MoveToElement();
			}

			if (!reader.IsEmptyElement)
			{
				bool exitLoop = false;

				while (!exitLoop)
				{
					if (!reader.Read())
						throw new Exception("XML parsing error.");

					switch (reader.NodeType)
					{
						case XmlNodeType.Element:
							element.Add(readXml(reader));
							break;

						case XmlNodeType.CDATA:
						case XmlNodeType.Text:
							if (element.Value == null)
								element.Value = reader.Value;
							else
								element.Value += reader.Value;

							break;

						case XmlNodeType.EndElement:
							if (reader.Name != element.Name)
								throw new Exception("XML parsing error.");

							exitLoop = true;
							break;
					}
				}
			}

			return element;
		}

		#endregion

		#region WriteXml

		/// <summary>
		/// Writes the data element to the stream using the XML format.
		/// </summary>
		/// <param name="stream"></param>
		public void WriteXml(Stream stream)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(stream, writerSettings))
			{
				this.writeXml(xmlWriter);
			}
		}

		/// <summary>
		/// Writes the data element to the stream using the XML format.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(TextWriter writer)
		{
			using (XmlWriter xmlWriter = XmlWriter.Create(writer, writerSettings))
			{
				this.writeXml(xmlWriter);
			}
		}

		private void writeXml(XmlWriter writer)
		{
			writer.WriteStartElement(this.name);

			foreach (KeyValuePair<string, string> attribute in this.attributes)
			{
				if (attribute.Value == null)
					continue;

				writer.WriteStartAttribute(attribute.Key);
				writer.WriteValue(attribute.Value);
				writer.WriteEndAttribute();
			}

			foreach (XmlNode element in this)
			{
				element.writeXml(writer);
			}

			writer.WriteValue(this.value);
			writer.WriteEndElement();
		}

		#endregion

		#endregion
	}
}
