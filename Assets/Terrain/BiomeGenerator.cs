/*
Copyright (C) 2022 Owen Feldmann

This file is part of Island Generator.

Island Generator is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License 
as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Island Generator is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with Island Generator. If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
	public bool generateBiomes = true;
	
	//How many biomes should be generated
	public int biomesToPlace = 10;
	//How far can biome points be from the center of the map
	public float maxBiomeSpread = 50f;
	//Biome points to be used with voronoi noise
	public BiomePoint[] biomePoints;
	//An array of biomes to be defined in the editor
	public Biome[] biomes;
	//Biome for when no biomes are generated
	public Biome nullBiome;
	
    public IEnumerator GenerateBiomes()
    {
		if(!generateBiomes)
			yield break;
		
        MeshGenerator mg = GetComponent<MeshGenerator>();
		GenerateBiomePoints(mg.xCenter, mg.zCenter);
		yield return StartCoroutine(GenerateBiomeTerrain(mg.vertices, mg.vertexColors, mg.biomes, mg.seaLevel));
    }
	
	/*
	Generate and place each biome point
	*/
    void GenerateBiomePoints(float xCenter, float zCenter)
	{
		biomePoints = new BiomePoint[biomesToPlace];
		Vector3 center = new Vector3(xCenter, 0f, zCenter);
		for(int i = 0; i < biomesToPlace; i++)
		{
			Biome biome = biomes[Random.Range(0, biomes.Length)];
			Vector2 randomPoint = Random.insideUnitCircle * maxBiomeSpread;
			Vector3 location = center + new Vector3(randomPoint.x, 10f, randomPoint.y);
			biomePoints[i] = new BiomePoint(biome, location);
		}
	}
	
	/*
	Generate terrain based on the relevant biome
	*/
	private IEnumerator GenerateBiomeTerrain(Vector3[] vertices, Color[] colors, Biome[] biomes, float seaLevel)
	{
		IslandUI islandUI = GameObject.Find("UI").GetComponent<IslandUI>();
		islandUI.SetGenerationText("Determining Biome Regions");
		
		//setup biomes array and color map with Voronoi noise
		for(int i = 0; i < vertices.Length; i++)
		{
			if(vertices[i].y >= seaLevel)
			{
				biomes[i] = ClosestBiome(vertices[i]);
				
				colors[i] = biomes[i].gradient.Evaluate(.3f);
			}
			else
			{
				//Sea doesn't have a biome
				biomes[i] = null;
			}
		}
		
		MeshGenerator mg = GetComponent<MeshGenerator>();
		if(mg.animateGeneration)
		{//wait after Voronoi Noise drawn
			mg.UpdateMesh();
			yield return new WaitForSeconds(1f);
		}
		
		islandUI.SetGenerationText("Generating Biome Terrain");
		//Generate actual biome heights and colors
		for(int i = 0; i < vertices.Length; i++)
		{
			if(biomes[i] != null)
			{
				//Vertex height
				float noiseValue = Noise.NoiseValue(vertices[i].x, vertices[i].z, biomes[i].noiseXScale, biomes[i].noiseZScale, biomes[i].noiseOctaves, biomes[i].octaveFrequencyScale, biomes[i].octaveAmplitudeScale, true);
				float y = biomes[i].heightCurve.Evaluate(noiseValue) * biomes[i].amplitudeScale;
				vertices[i].y += y;
				
				//Color biome
				colors[i] = biomes[i].gradient.Evaluate(noiseValue);
			}
		}
		
	}
	
	/*
	Returns the closest biome to the provided location.
	
	Returns the nullBiome if no biomes are generated
	*/
	public Biome ClosestBiome(Vector3 location)
	{
		if(!generateBiomes)
			return nullBiome;
		
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
		
		return closest.biome;
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