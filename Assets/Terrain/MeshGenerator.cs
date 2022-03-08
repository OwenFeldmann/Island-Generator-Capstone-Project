using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
	//Generated mesh and relevant variables
	Mesh mesh;
	[HideInInspector]public Vector3[] vertices;
	int[] triangles;
	[HideInInspector]public Color[] vertexColors;
	//The coresponding biome to each vertex. Hidden for preformance reasons
	[HideInInspector]public Biome[] biomes;
	
	public bool animateGeneration = true;
	public bool jiggleVertices = true;
	public bool SmoothTerraceVertices = false;
	public float terraceHeight = 1f;
	
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
	//Gradient color used by underwater vertices from 0 to seaLevel.
	public Gradient underWaterGradient;
	//Gradient used on island before biomes are applied
	public Gradient landGradient;
	//Decorative water plane
	public GameObject waterPlane;
	
	[Header("Noise Settings")]
	//Seeds the noise function. Uses a random seed when set to 0.
	public int noiseSeed = 0;
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
	
	[Header("Props")]
	public bool generateProps = true;
	public int propsToTryToPlace = 1000;
	public GameObject propPrefab;
	
	[Header("Nav Mesh")]
	public NavMeshSurface surface;
	public GameObject player;
	
	private Coroutine generateIslandCorountine;
	private float maxHeight = 0f;
	
	/*
	Script starting location. Creates and displays generated terrain mesh.
	*/
    void Start()
    {
        generateIslandCorountine = StartCoroutine(GenerateIsland());
    }
	
	IEnumerator GenerateIsland()
	{
		IslandUI islandUI = GameObject.Find("UI").GetComponent<IslandUI>();
		islandUI.SetGenerationPanelActive(animateGeneration);//show readout for generation process if animating
		islandUI.SetGenerationText("Starting Generation Process");
		
		//setup water plane
		//seaLevel-1 so that waves don't come on land
		waterPlane.transform.position = new Vector3(xCenter, seaLevel-1, zCenter);
		
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		
		Noise.SeedNoise(noiseSeed);
		
		islandUI.SetGenerationText("Generating Mesh with Heightmap");
		CreateMesh();
		MeshModifier meshModifier = new MeshModifier(vertices, vertexColors, biomes);
		
		if(animateGeneration)
		{//wait after island shaped
			UpdateMesh();
			yield return new WaitForSeconds(1f);
		}
		
		BiomeGenerator bg = GetComponent<BiomeGenerator>();
		if(bg.generateBiomes)
		{
			
			islandUI.SetGenerationText("Flattening Island");
			meshModifier.FlattenTerrainToLevel(seaLevel);//just need to know what parts of the island are above water
			
			if(animateGeneration)
			{//wait after flatten
				UpdateMesh();
				yield return new WaitForSeconds(1f);
			}
			
			yield return StartCoroutine(bg.GenerateBiomes());
			
			if(animateGeneration)
			{//wait after biomes generated
				UpdateMesh();
				yield return new WaitForSeconds(1f);
			}
			
			islandUI.SetGenerationText("Blending Biomes");
			meshModifier.BlendBiomes(2, zSize);//cleans up biome borders
			
			if(animateGeneration)
			{//wait after biomes blended
				UpdateMesh();
				yield return new WaitForSeconds(1f);
			}
		}
		
		VolcanoGenerator vg  = GetComponent<VolcanoGenerator>();
		if(vg.generateVolcano)
		{
			
			islandUI.SetGenerationText("Building Volcano");
			yield return StartCoroutine(vg.BuildVolcano());
			
			if(animateGeneration)
			{//wait after volcano generated
				UpdateMesh();
				yield return new WaitForSeconds(1f);
			}
		}
		
		if(SmoothTerraceVertices)
		{
			islandUI.SetGenerationText("Smooth Terracing Island");
			meshModifier.SmoothTerraceTerrain(terraceHeight);
			UpdateMesh();
			yield return new WaitForSeconds(1f);
		}
		
		if(jiggleVertices)
		{
			islandUI.SetGenerationText("Jiggling Vertices");
			meshModifier.JiggleVertices(seaLevel);//jiggle to add roughness to terrain
		}
		UpdateMesh();//ensures the mesh is updated on all code paths at the end
		
		if(animateGeneration)
		{//wait after vertices jiggled
			yield return new WaitForSeconds(1f);
		}
		
		//Place props
		if(generateProps)
		{
			islandUI.SetGenerationText("Placing Props");
			PropPlacement propPlacer = new PropPlacement(GetComponent<MeshCollider>(), bg, vg, propPrefab);
			propPlacer.PlaceProps(transform, propsToTryToPlace, xSize, zSize, seaLevel);
		
			if(animateGeneration)
			{//wait after props generated
				yield return new WaitForSeconds(1f);
			}
		}
		
		islandUI.SetGenerationText("Pausing for Viewing Pleasure");
		if(animateGeneration)//pause at end of generation
			yield return new WaitForSeconds(3f);
		
		islandUI.SetGenerationText("Readying Player");
		Instantiate(player, vertices[vertices.Length/2], Quaternion.identity);
		surface.BuildNavMesh();
		
		islandUI.SetGenerationPanelActive(false);//Hide readout panel
	}
	
	/*
	Returns a height value at the specified location, using the noise parameters specified in the editor.
	*/
	private float HeightAt(int x, int z)
	{
		float y = Noise.NoiseValue((float)x, (float)z, noiseXScale, noiseZScale, noiseOctaves, octaveFrequencyScale, octaveAmplitudeScale, false);
		
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
		biomes = new Biome[vertices.Length];
		
		for(int i = 0, z = 0; z <= zSize; z++)
		{
			for(int x = 0; x <= xSize; x++)
			{
				float y = HeightAt(x, z);
				if(y > maxHeight)
					maxHeight = y;
				
				vertices[i] = new Vector3(x, y, z);
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
		
		//Define vertex colors of underwater vertices
		vertexColors = new Color[vertices.Length];
		
		for(int i = 0, z = 0; z <= zSize; z++)
		{
			for(int x = 0; x <= xSize; x++)
			{
				if(vertices[i].y < seaLevel)
				{
					float height = Mathf.InverseLerp(0, seaLevel, vertices[i].y);
					vertexColors[i] = underWaterGradient.Evaluate(height);
				}
				else
				{
					float height = Mathf.InverseLerp(seaLevel, maxHeight, vertices[i].y);
					vertexColors[i] = landGradient.Evaluate(height);
				}
				i++;
			}
		}
		
	}
	
	/*
	Must executed once changes to the mesh are done.
	*/
	public void UpdateMesh()
	{
		mesh.Clear();
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.colors = vertexColors;
		
		mesh.RecalculateNormals();
		
		GetComponent<MeshCollider>().sharedMesh = mesh;
		
		surface.BuildNavMesh();
	}
	
}
