using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
A helper class to consolidate noise related methods
*/
public class Noise
{
    
	private static float xOffset = 0f;
	private static float zOffset = 0f;
	
	/*
	Returns a noise value for a point with the given noise parameters.
	*/
	public static float NoiseValue(float x, float z, float xScale, float zScale, float octaves, float octaveFrequencyScale, float octaveAmplitudeScale, bool normalize)
	{
		float y = 0f;
		float maxValue = 0f;
		float scale = 1f;
		float amplitude = 1f;
		
		for(int i = 0; i < octaves; i++)
		{
			y += Mathf.PerlinNoise((x * xScale * scale) + xOffset, (z * zScale * scale) + zOffset) * amplitude;
			maxValue += amplitude;
			scale *= octaveFrequencyScale;
			amplitude *= octaveAmplitudeScale;
		}
		
		if(normalize)
			y /= maxValue;
		
		return y;
	}
	
	/*
	Seeds noise function by deciding an offset. Chooses a random seed when the given seed is 0.
	*/
	public static void SeedNoise(int seed)
	{
		if(seed == 0)
			seed = Random.Range(-10000,10000);
		Random.InitState(seed);
		xOffset = Random.Range(-10000, 10000);
		zOffset = Random.Range(-10000, 10000);
	}
}
