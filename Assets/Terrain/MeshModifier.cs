using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
A method wrapper class for modifying the vertices of a terrain mesh
*/
public class MeshModifier
{
    
	private Vector3[] vertices;
	private Color[] colors;
	private Biome[] biomes;
	
	public MeshModifier(Vector3[] vertices, Color[] colors, Biome[] biomes)
	{
		this.vertices = vertices;
		this.colors = colors;
		this.biomes = biomes;
	}
	
	/*
	Reduces the maximum height of terrain to the provied level by pushing down everything above.
	*/
	public void FlattenTerrainToLevel(float level)
	{
		for(int i = 0; i < vertices.Length; i++)
		{
			if(vertices[i].y > level)
				vertices[i].y = level;
		}
	}
	
	/*
	Moves every vertex to a random position within a sphere of radius: maxJiggle.
	Respects wether a point is above or below sea level.
	*/
	public void JiggleVertices(float seaLevel, float maxJiggle = 0.3f)
	{
		bool aboveSeaLevel;
		for(int i = 0; i < vertices.Length; i++)
		{
			
			aboveSeaLevel = vertices[i].y >= seaLevel;
			
			vertices[i] += Random.insideUnitSphere * maxJiggle;
			
			if(aboveSeaLevel && vertices[i].y < seaLevel)
				vertices[i].y = seaLevel;
			else if(!aboveSeaLevel && vertices[i].y >= seaLevel)
				vertices[i].y = seaLevel - 0.01f;
				
		}
	}
	
	/*
	Turns the terrain into terraces of specified height, rather than smooth slopes
	*/
	public void TerraceTerrain(float terraceHeight)
	{
		for(int i = 0; i < vertices.Length; i++)
		{
			vertices[i].y = terraceHeight * Mathf.Floor(vertices[i].y / terraceHeight);
		}
	}
	
	/*
	Averages every point with the surrounding points based on the given square radius.
	Assumes there are no land points within the given radius of the edge of the map.
	*/
	public void BlendBiomes(int radius, int zSize)
	{
		for(int i = 0; i < vertices.Length; i++)
		{
			if(CanBlendThisBiomeAt(i))
			{
				int pointsInHeightAverage = 0;
				float totalHeight = 0f;
				int pointsInColorAverage = 0;
				float totalRed = 0f, totalGreen = 0f, totalBlue = 0f;
				
				for(int z = -radius * zSize; z <= radius * zSize; z += zSize)
				{
					for(int x = -radius; x <= radius; x++)
					{
						int p = i+z+x;
						
						if(!ExcludePointAsHeightOutlier(p, i))
						{
							pointsInHeightAverage++;
							totalHeight += vertices[p].y;
						}
						
						if(!ExcludePointAsColorOutlier(p))
						{
							pointsInColorAverage++;
							totalRed += colors[p].r;
							totalGreen += colors[p].g;
							totalBlue += colors[p].b;
						}
					}
				}
				
				if(pointsInHeightAverage > 0)
					vertices[i].y = totalHeight / pointsInHeightAverage;
				if(pointsInColorAverage > 0)
					colors[i] = new Color(totalRed/pointsInColorAverage, totalGreen/pointsInColorAverage, totalBlue/pointsInColorAverage);
				
			}
		}
	}
	
	/*
	Returns false if the biome at a point should not be blended
	*/
	private bool CanBlendThisBiomeAt(int i)
	{
		if(biomes[i] == null)
			return false;//don't blend the sea
		if(biomes[i].name == "Plateau")
			return false;//don't blend the plateau. Try to maintain the cliff edge
		if(biomes[i].name == "Pointy Mountains" && vertices[i].y >= 10f)
			return false;//don't flatten the spikes in the pointy mountains
		
		return true;
	}
	
	/*
	Returns true if the height at this point should be excluded from the blending average
	*/
	private bool ExcludePointAsHeightOutlier(int i, int basePoint)
	{
		if(biomes[i] == null)
			return false;//needed a null check. Reduce sheer cliffs at sea edge
		
		if(biomes[i].name == "Plateau")
		{
			if(biomes[basePoint].name == "Mountains" && vertices[basePoint].y > vertices[i].y)
			{
				return false;//case that a mountain is next to the plateau and forming a cliff edge
			}
			else
			{
				return true;//exclude the plataeu. Try to maintain the plateau's cliff edge
			}
		}
			
		if(biomes[i].name == "Pointy Mountains" && vertices[i].y >= 10f)
			return true;//the spikes in this biome are outliers
		
		
		return false;
	}
	
	/*
	Return true if the color at this point should be excluded from the blending average
	*/
	private bool ExcludePointAsColorOutlier(int i)
	{
		if(biomes[i] == null)
			return true;//exclude the sea, otherwise the shore turns bluer
		if(biomes[i].name == "Plateau")
			return true;//exclude the plataeu. The color is too abnormal
		//if(biomes[i].name == "Pointy Mountains" && vertices[i].y >= 10f)
		//	return true;
		
		return false;
	}
	
}
