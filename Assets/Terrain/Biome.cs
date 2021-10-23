using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
A container class that provides the needed information to generate a biome
*/
[System.Serializable]
public class Biome
{
    
	public string name;
	public Gradient gradient;
	public AnimationCurve heightCurve;
	
	//Decrease to stretch terrain along the x-axis.
	public float noiseXScale = 0.3f;
	//Decrease to stretch terrain along the z-axis.
	public float noiseZScale = 0.3f;
	//Stretch terrain height uniformly
	public float amplitudeScale = 1f;
	/*
	How many octaves to apply to the noise function.
	Noise is not normalize. This will affect the heigh of the terrain!
	*/
	public int noiseOctaves = 5;
	public float octaveFrequencyScale = 0.5f;
	public float octaveAmplitudeScale = 2f;
	
}
