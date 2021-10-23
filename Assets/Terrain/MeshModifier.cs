using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
A method wrapper class for modifying the vertices of a terrain mesh
*/
public class MeshModifier
{
    
	private Vector3[] vertices;
	
	public MeshModifier(Vector3[] vertices)
	{
		this.vertices = vertices;
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
	
}
