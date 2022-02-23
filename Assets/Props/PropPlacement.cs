using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropPlacement
{
	private MeshCollider island;
	private BiomeGenerator biomeGen;
	private GameObject propPrefab;
	
	private float minDistanceBetweenProps = 0.5f;
	
	public PropPlacement(MeshCollider islandCollider, BiomeGenerator biomeGenerator, GameObject propPrefab)
	{
		this.island = islandCollider;
		this.biomeGen = biomeGenerator;
		this.propPrefab = propPrefab;
	}
	
	public void PlaceProps(Transform propHolder, int propsToPlace, int xMax, int zMax, float seaLevel)
	{
		
		for(int i = 0; i < propsToPlace; i++)
		{
			float x = Random.Range(0f, xMax);
			float z = Random.Range(0f, zMax);
			
			Ray ray = new Ray(new Vector3(x, 100f, z), Vector3.down);
			RaycastHit hit;
			
			if(island.Raycast(ray, out hit, 300f))
			{
				if(hit.point.y >= seaLevel)//no underwater props
					PlaceProp(hit.point, propHolder);
			}
		}
		
	}
	
	private void PlaceProp(Vector3 location, Transform propHolder)
	{
		PropPlacementData[] propPlacementData = biomeGen.ClosestBiomePoint(location).biome.props;
		PropPlacementData[] validProps = new PropPlacementData[propPlacementData.Length];
		int nextValidPropIndex = 0;
		
		//Check what props can actually be placed here
		for(int i = 0; i < propPlacementData.Length; i++)
		{
			if(CanPlacePropAt(propPlacementData[i], location, propHolder))
			{
				validProps[nextValidPropIndex] = propPlacementData[i];
				nextValidPropIndex++;
			}
		}
		
		//Check if no props can be placed here
		if(nextValidPropIndex == 0)
			return;
		
		//Place a random valid prop
		PropPlacementData propData = validProps[Random.Range(0, nextValidPropIndex)];
		GameObject prop = GameObject.Instantiate(propPrefab, location, Quaternion.identity, propHolder);
		prop.GetComponent<Prop>().Setup(propData);
	}
	
	/*
	Checks if the noise value is within tolerance where the prop is placed.
	Encourages similar props to be clustered on the noise hills.
	
	Prevents props from being placed too close to existing props
	*/
	private bool CanPlacePropAt(PropPlacementData ppd, Vector3 loc, Transform propHolder)
	{
		foreach(Transform child in propHolder)
		{
			if(Vector3.Distance(loc, child.position) < minDistanceBetweenProps)
				return false;
		}
		
		//% 1000 to keep it in reasonable bounds.
		//If coordinates are too big, PerlinNoise returns the same value every time.
		int offset = ppd.propName.GetHashCode() % 1000;
		return Mathf.PerlinNoise(offset + loc.x * ppd.noiseXScale, offset + loc.z * ppd.noiseZScale) >= ppd.tolerance;
	}
	
}
