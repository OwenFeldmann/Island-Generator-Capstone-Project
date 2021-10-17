using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
	Mesh mesh;
	
	Vector3[] vertices;
	int[] triangles;
	Color[] vertexColors;
	
	public Gradient gradient;
	
	public int xSize = 50;
	public int zSize = 50;
	
	private float minTerrainHeight;
	private float maxTerrainHeight;
	
    void Start()
    {
        mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		
		CreateMesh();
		
    }

    void CreateMesh()
	{
		//Define vertex positions
		vertices = new Vector3[(xSize + 1) * (zSize + 1)];
		
		for(int i = 0, z = 0; z <= zSize; z++)
		{
			for(int x = 0; x <= xSize; x++)
			{
				float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
				vertices[i] = new Vector3(x, y, z);
				
				if(y > maxTerrainHeight)
					maxTerrainHeight = y;
				if(y < minTerrainHeight)
					minTerrainHeight = y;
				
				i++;
			}
		}
		
		//Define triangles
		triangles = new int[6 * xSize * zSize];
		int vert = 0;
		int tris = 0;
		for(int z = 0; z < zSize; z++)
		{
			for(int x = 0; x < xSize; x++)
			{
				triangles[tris + 0] = vert + 0;
				triangles[tris + 1] = vert + xSize + 1;
				triangles[tris + 2] = vert + 1;
				triangles[tris + 3] = vert + 1;
				triangles[tris + 4] = vert + xSize + 1;
				triangles[tris + 5] = vert + xSize + 2;
				
				vert++;
				tris += 6;
			}
			vert++;
		}
		
		//Define vertex colors
		vertexColors = new Color[vertices.Length];
		
		for(int i = 0, z = 0; z <= zSize; z++)
		{
			for(int x = 0; x <= xSize; x++)
			{
				float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
				vertexColors[i] = gradient.Evaluate(height);
				i++;
			}
		}
		
		UpdateMesh();
	}
	
	void UpdateMesh()
	{
		mesh.Clear();
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.colors = vertexColors;
		
		mesh.RecalculateNormals();
	}
	
}
