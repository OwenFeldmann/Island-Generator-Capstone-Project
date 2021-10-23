using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
    
	MeshGenerator meshGenerator;
	
	//How many biomes should be generated
	public int biomesToPlace = 10;
	//How far can biome points be from the center of the map
	public float maxBiomeSpread = 50f;
	//Biome points to be used with voronoi noise
	public BiomePoint[] biomePoints;
	//An array of biomes to be defined in the editor
	public Biome[] biomes;
	
    void Start()
    {
        meshGenerator = GetComponent<MeshGenerator>();
		GenerateBiomePoints();
    }
	
	/*
	Generate and place each biome point
	*/
    void GenerateBiomePoints()
	{
		biomePoints = new BiomePoint[biomesToPlace];
		Vector3 center = new Vector3(meshGenerator.xCenter, 0f, meshGenerator.zCenter);
		for(int i = 0; i < biomesToPlace; i++)
		{
			Biome biome = biomes[Random.Range(0, biomes.Length)];
			Vector2 randomPoint = Random.insideUnitCircle * maxBiomeSpread;
			Vector3 location = center + new Vector3(randomPoint.x, 10f, randomPoint.y);
			biomePoints[i] = new BiomePoint(biome, location);
		}
	}
	
	/*
	Testing by coloring terrain flattly based on biome
	*/
	public void TESTGenerateBiomes()
	{
		float seaLevel = meshGenerator.seaLevel;
		Vector3[] vertices = meshGenerator.vertices;
		Color[] colors = meshGenerator.vertexColors;
		
		for(int i = 0; i < vertices.Length; i++)
		{
			if(vertices[i].y >= seaLevel)
			{
				Biome biome = ClosestBiomePoint(vertices[i]).biome;
				colors[i] = biome.gradient.Evaluate(0.5f);
			}
		}
		
	}
	
	/*
	Returns the closest biome point to the provided location
	*/
	BiomePoint ClosestBiomePoint(Vector3 location)
	{
		BiomePoint closest = biomePoints[0];
		float dist, closestDist = Vector3.Distance(location, biomePoints[0].location);
		
		for(int i = 1; i < biomePoints.Length; i++)
		{
			dist = Vector3.Distance(location, biomePoints[i].location);
			if(dist < closestDist)
			{
				closest = biomePoints[i];
				closestDist = dist;
			}
		}
		
		return closest;
	}
	
	/*
	Debug visualization of where each biome point is
	*/
	void OnDrawGizmos()
	{
		foreach(BiomePoint bp in biomePoints)
		{
			Gizmos.color = bp.biome.gradient.Evaluate(0.5f);
			Gizmos.DrawSphere(bp.location, 1);
		}
	}
	
}

/*
A container class to facilitate voronoi noise generation
*/
[System.Serializable]
public class BiomePoint
{
	public Biome biome;
	public Vector3 location;
	
	public BiomePoint(Biome biome, Vector3 location)
	{
		this.biome = biome;
		this.location = location;
	}
	
}