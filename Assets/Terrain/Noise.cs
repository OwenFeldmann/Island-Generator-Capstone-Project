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
	Seeds noise function by deciding an offset.
	*/
	public static void SeedNoise(int seed)
	{
		Random.InitState(seed);
		xOffset = Random.Range(-10000, 10000);
		zOffset = Random.Range(-10000, 10000);
	}
}
