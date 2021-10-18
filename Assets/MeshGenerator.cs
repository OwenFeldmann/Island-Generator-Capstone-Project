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
	
	public int xSize = 256;
	public int zSize = 256;
	
	public int xCenter = 128;
	public int zCenter = 128;
	public float distanceFromCenterFalloffRate = 0.1f;
	
	public float seaLevel = 1f;
	public Color underWaterColor;
	
	public int noiseOctaves = 3;
	public float octaveScale = 2;
	
	private float maxTerrainHeight;
	
    void Start()
    {
        mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		
		CreateMesh();
		
    }
	
	float HeightAt(int x, int z)
	{
		float y = 0f;
		float scale = octaveScale;
		float amplitude = octaveScale;
		for(int i = 0; i < noiseOctaves; i++)
		{
			y += Mathf.PerlinNoise(x / scale, z / scale) * amplitude;
			scale *= octaveScale;
			amplitude += octaveScale;
		}
		
		float distanceToCenter = Vector2.Distance(new Vector2(xCenter, zCenter), new Vector2(x, z));
		y -= distanceToCenter * distanceFromCenterFalloffRate;
		
		if(y < 0f)
			y = 0f;
		return y;
	}
	
    void CreateMesh()
	{
		//Define vertex positions
		vertices = new Vector3[(xSize + 1) * (zSize + 1)];
		
		for(int i = 0, z = 0; z <= zSize; z++)
		{
			for(int x = 0; x <= xSize; x++)
			{
				//float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
				float y = HeightAt(x, z);
				vertices[i] = new Vector3(x, y, z);
				
				if(y > maxTerrainHeight)
					maxTerrainHeight = y;
				
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
				if(vertices[i].y >= seaLevel)
				{
					float height = Mathf.InverseLerp(seaLevel, maxTerrainHeight, vertices[i].y);
					vertexColors[i] = gradient.Evaluate(height);
				}
				else
				{
					vertexColors[i] = underWaterColor;
				}
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
