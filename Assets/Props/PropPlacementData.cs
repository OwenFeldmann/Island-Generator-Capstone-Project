using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PropPlacementData : ScriptableObject
{
	public string propName;
	//The prop's mesh
    public Mesh mesh;
	//The prop's materials
	public Material[] materials;
	//Should the prop block the player
	public bool hasCollider = false;
	//If the prop can block the player, how big should it be?
	public float colliderRadius;
	public float colliderHeight;
	
	/*
	Using a single octave of Perlin noise to place clusters of props
	*/
	public float noiseXScale = 0.3f;
	public float noiseZScale = 0.3f;
	//Place props if noise is greater than this. Between 0 and 1
	public float tolerance = 0.6f;
}
