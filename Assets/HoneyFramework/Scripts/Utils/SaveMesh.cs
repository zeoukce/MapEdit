using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Text;  
using UnityEngine; 

public class SaveMesh
{
	static public void SaveOBJ(MeshFilter mf, string name, string dir){
		if (mf == null) return;


		string dirPath = Application.dataPath + "/../" + dir;
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}

		FileStream file = File.Open( dirPath+"/" + name + @".obj", FileMode.Create);
		BinaryWriter binary = new BinaryWriter(file);
		byte[] data =Encoding.ASCII.GetBytes(parseMeshFilter(mf));
		binary.Write(data);
		file.Close();
	}
	private static string FloatFormat(float value){
		return value.ToString ("F4");
	}
	/// <summary>
	/// 解析MeshFilter数据
	/// </summary>
	private static string parseMeshFilter( MeshFilter mf )
	{
		StringBuilder buf = new StringBuilder(); 
		int offsetVertices = 0;
		Mesh mesh = mf.sharedMesh;
		if (!mesh)
		{
			return "";
		}
		buf.AppendFormat("# {0}.obj\n" , mf.name );
		buf.AppendFormat("# File Created: {0} {1}\n" ,System.DateTime.Now.ToLongDateString(),System.DateTime.Now.ToLongTimeString());
		buf.AppendLine("#-------\n\n");
		buf.AppendFormat("mtllib {0}.mtl\n\n", mf.name);
		Transform meshTrans = mf.transform;
		int vertices = 0;
		Vector3[] allVertices = mesh.vertices;
		foreach (Vector3 vertice in allVertices)
		{
			vertices++;
			buf.AppendFormat("v  {0} {1} {2} \n", FloatFormat(vertice.x), FloatFormat(vertice.y), FloatFormat(vertice.z));
		}
		buf.AppendFormat ("# {0} vertices \n\n",vertices);
		vertices = 0;
		foreach (Vector3 normal in mesh.normals)
		{
			vertices++;
			buf.AppendFormat("vn {0} {1} {2} \n", FloatFormat(normal.x), FloatFormat(normal.y), FloatFormat(normal.z));
		}
		buf.AppendFormat ("# {0} vertex normals \n\n",vertices);
		vertices = 0;
		foreach (Vector2 uv in mesh.uv)
		{
			vertices++;
			buf.AppendFormat("vt {0} {1}\n", FloatFormat(uv.x), FloatFormat(uv.y));
		}
		buf.AppendFormat ("# {0} texture coords \n\n",vertices);
		buf.AppendFormat("g {0} \n", mf.name);
		Material[] mats = meshTrans.GetComponent<Renderer>().sharedMaterials;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			buf.Append ("\n");
			buf.Append("usemtl ").Append(mats[i].name).Append("\n");
			buf.Append("usemap ").Append(mats[i].name).Append("\n");
			buf.AppendFormat ("s {0}\n", i);
			int[] triangles = mesh.GetTriangles(i);
			for (int j = 0; j < triangles.Length; j += 3)
			{
				buf.AppendFormat("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
					triangles[j] + 1 + offsetVertices, triangles[j + 1] + 1 + offsetVertices, triangles[j + 2] + 1 + offsetVertices);
			}
			buf.AppendFormat ("# {0} polygons - {1} triangles\n",i,triangles.Length);
		}
		return buf.ToString();
	}

}


