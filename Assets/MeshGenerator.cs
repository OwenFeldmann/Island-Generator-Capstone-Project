using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
	//Generated mesh and relevant variables
	Mesh mesh;
	Vector3[] vertices;
	int[] triangles;
	Color[] vertexColors;
	
	//Color gradient between seaLevel and the highest point
	public Gradient gradient;
	
	[Header("World Shape")]
	//Length of the mesh in the x direction.
	public int xSize = 256;
	//Length of the mesh in the z direction.
	public int zSize = 256;
	//X coordinate for the center point of the island.
	public int xCenter = 128;
	//Z coordinate for the center point of the island.
	public int zCenter = 128;
	//How quickly the terrain descends into the water. Smaller values result in larger islands.
	public float distanceFromCenterFalloffRate = 0.1f;
	//Vertices above here are considered land.
	public float seaLevel = 3f;
	//Color used by underwater vertices.
	public Color underWaterColor;
	
	[Header("Noise Settings")]
	//Seeds the noise function. Uses a random seed when set to 0.
	public int noiseSeed = 0;
	private float noiseXOffset = 0;
	private float noiseZOffset = 0;
	//Decrease to stretch terrain along the x-axis.
	public float noiseXScale = 0.3f;
	//Decrease to stretch terrain along the z-axis.
	public float noiseZScale = 0.3f;
	/*
	How many octaves to apply to the noise function.
	Noise is not normalize. This will affect the heigh of the terrain!
	*/
	public int noiseOctaves = 5;
	public float octaveFrequencyScale = 0.5f;
	public float octaveAmplitudeScale = 2f;
	
	//Heighest point on the generated terrain. Used for coloring.
	private float maxTerrainHeight;
	
	/*
	Script starting location. Creates and displays generated terrain mesh.
	*/
    void Start()
    {
        mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		
		SeedNoise(noiseSeed);
		
		CreateMesh();
		UpdateMesh();
		
    }
	
	/*
	Seeds noise function by deciding an offset. Chooses a random seed when the given seed is 0.
	*/
	void SeedNoise(int seed)
	{
		if(seed == 0)
			seed = Random.Range(-10000,10000);
		Random.InitState(seed);
		noiseXOffset = Random.Range(-10000, 10000);
		noiseZOffset = Random.Range(-10000, 10000);
	}
	
	/*
	Returns a heigh value at the specified location, using the noise parameters specified in the editor.
	*/
	private float HeightAt(int x, int z)
	{
		float y = 0f;
		float scale = 1;
		float amplitude = 1;
		for(int i = 0; i < noiseOctaves; i++)
		{
			y += Mathf.PerlinNoise((x * noiseXScale * scale) + noiseXOffset, (z * noiseZScale * scale) + noiseZOffset) * amplitude;
			scale *= octaveFrequencyScale;
			amplitude *= octaveAmplitudeScale;
		}
		
		float distanceToCenter = Vector2.Distance(new Vector2(xCenter, zCenter), new Vector2(x, z));
		y -= distanceToCenter * distanceFromCenterFalloffRate;
		
		if(y < 0f)
			y = 0f;
		return y;
	}
	
	/*
	Creates the data for a terrain mesh. UpdateMesh() still needs to be executed to finalize changes.
	*/
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
		
	}
	
	/*
	Must executed once changes to the mesh are done.
	*/
	void UpdateMesh()
	{
		mesh.Clear();
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.colors = vertexColors;
		
		mesh.RecalculateNormals();
	}
	
}
