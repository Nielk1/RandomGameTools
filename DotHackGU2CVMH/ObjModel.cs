using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotHackGU2CVMH
{
    class ObjModel
    {
        public struct Vector3
        {
            public float x;
            public float y;
            public float z;
        }

        public class Material
        {
            public string Name;
            public Vector3 Ka = new Vector3() { x = 1.000f, y = 1.000f, z = 1.000f };
            public Vector3 Kd = new Vector3() { x = 1.000f, y = 1.000f, z = 1.000f };
            public Vector3 Ks = new Vector3() { x = 0.000f, y = 0.000f, z = 0.000f };
            public float d = 1.0f;
            public int illum = 2;
            public string map_Kd;
        }

        public string name;
        public List<Vector3> vertices;
        public List<Vector3> normals;
        public List<Vector3> uv;
        public Dictionary<string, List<int>> faces;
        public List<Material> mats;
        //public List<int> triangles;

        public ObjModel(string name)
        {
            this.name = name;
            this.vertices = new List<Vector3>();
            this.normals = new List<Vector3>();
            this.uv = new List<Vector3>();
            //this.triangles = new List<int>();
            this.faces = new Dictionary<string, List<int>>();
            this.mats = new List<Material>();
        }

        public string MeshToString()//MeshFilter mf)
        {
            //Mesh m = mf.mesh;
            //Material[] mats = mf.renderer.sharedMaterials;

            StringBuilder sb = new StringBuilder();

            faces.Keys.ToList().ForEach(mat =>
            {
                sb.Append("mtllib ").Append(mat + @".mtl").Append("\n");
            });

            sb.Append("g ").Append(name).Append("\n");
            foreach (Vector3 v in vertices)
            {
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in normals)
            {
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
            }
            sb.Append("\n");
            //int j = 0;
            foreach (Vector3 v in uv)
            {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
                //sb.Append(string.Format("vt {0} {1}\n", 0, (j * 1.0 / vertices.Count)));
                //j++;
            }
            //for (int material = 0; material < triangles.Count; material++)
            foreach (string materialName in faces.Keys)
            {
                sb.Append("\n");
                sb.Append("usemtl ").Append(materialName).Append("\n");
                sb.Append("usemap ").Append(materialName).Append("\n");

                List<int> triangles = faces[materialName];
                for (int i = 0; i < triangles.Count; i += 3)
                {
                    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
                }
            }
            //for (int i = 0; i < triangles.Count; i += 3)
            //{
            //    sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
            //        triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            //}
            return sb.ToString();
        }

        public string MaterialToFileToString()//MeshFilter mf)
        {
            //Mesh m = mf.mesh;
            //Material[] mats = mf.renderer.sharedMaterials;

            StringBuilder sb = new StringBuilder();

            mats.ForEach(mat =>
            {
                sb.Append(string.Format("newmtl {0}\n", mat.Name));
                sb.Append(string.Format("Ka {0} {1} {2}\n", mat.Ka.x, mat.Ka.y, mat.Ka.z));
                sb.Append(string.Format("Kd {0} {1} {2}\n", mat.Kd.x, mat.Kd.y, mat.Kd.z));
                sb.Append(string.Format("Ks {0} {1} {2}\n", mat.Ks.x, mat.Ks.y, mat.Ks.z));
                sb.Append(string.Format("d {0}\n", mat.d));
                sb.Append(string.Format("illum {0}\n", mat.illum));
                sb.Append(string.Format("map_Kd {0}\n", mat.map_Kd));
                sb.Append("\n");
            });

            return sb.ToString();
        }

        public void MeshToFile(/*MeshFilter mf, */string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(MeshToString());//mf));
            }
        }

        public void MaterialToFile(/*MeshFilter mf, */string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(MaterialToFileToString());//mf));
            }
        }
    }
}
