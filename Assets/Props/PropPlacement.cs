using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropPlacement
{
	private MeshCollider island;
	private MeshGenerator mg;
	private BiomeGenerator biomeGen;
	private VolcanoGenerator vg;
	private GameObject propPrefab;
	
	private float minDistanceBetweenProps = 0.5f;
	private float maxVolcanoRadius;
	
	public PropPlacement(MeshCollider islandCollider, MeshGenerator mg, BiomeGenerator biomeGen, VolcanoGenerator vg, GameObject propPrefab)
	{
		this.island = islandCollider;
		this.mg = mg;
		this.biomeGen = biomeGen;
		this.vg = vg;
		this.propPrefab = propPrefab;
		maxVolcanoRadius = vg.MaxVolcanoRadius();
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
					PlaceProp(hit.point, hit.normal, propHolder);
			}
		}
		
	}
	
	private void PlaceProp(Vector3 location, Vector3 normal, Transform propHolder)
	{
		Biome biome = biomeGen.ClosestBiome(location);
		PropPlacementData[] propPlacementData = biome.props;
		PropPlacementData[] validProps = new PropPlacementData[propPlacementData.Length];
		int nextValidPropIndex = 0;
		
		//Check what props can actually be placed here
		for(int i = 0; i < propPlacementData.Length; i++)
		{
			if(CanPlacePropAt(propPlacementData[i], location, biome, propHolder))
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
		SetupProp(prop, propData, normal);
	}
	
	private void SetupProp(GameObject prop, PropPlacementData ppd, Vector3 normal)
	{
		prop.name = ppd.propName;
		
		prop.GetComponentInChildren<MeshFilter>().mesh = ppd.mesh;
		prop.GetComponentInChildren<MeshRenderer>().materials = ppd.materials;
		
		CapsuleCollider collider = prop.GetComponent<CapsuleCollider>();
		if(ppd.hasCollider)
		{
			collider.enabled = true;
			collider.radius = ppd.colliderRadius;
			collider.height = ppd.colliderHeight;
			collider.center = new Vector3(0, ppd.colliderHeight/2, 0);
		}
		else
			collider.enabled = false;
		
		Transform pt = prop.transform;
		//Set prop to be perpendicular to ground, with an upwards bias
		pt.LookAt(pt.position + normal + Vector3.up*2);
		//Set random rotation
		pt.eulerAngles = new Vector3(pt.eulerAngles.x, pt.eulerAngles.y, Random.Range(0, 360));
		//Adjust size slightly
		pt.localScale *= Random.Range(0.8f, 1.2f);
	}
	
	/*
	No props placed on the volcano.
	
	No props placed on spiky mountain spikes.
	
	Prevents props from being placed too close to existing props.
	
	Checks if the noise value is within tolerance where the prop is placed.
	Encourages similar props to be clustered on the noise hills.
	*/
	private bool CanPlacePropAt(PropPlacementData ppd, Vector3 loc, Biome biome, Transform propHolder)
	{
		if(vg.generateVolcano && Mathf.Sqrt(Mathf.Pow(loc.x - vg.centerX, 2) + Mathf.Pow(loc.z - vg.centerZ, 2))
				<= maxVolcanoRadius)
			return false;
		
		if(biome.name == "Pointy Mountains" && loc.y >= mg.seaLevel + 5f)
			return false;
		
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
