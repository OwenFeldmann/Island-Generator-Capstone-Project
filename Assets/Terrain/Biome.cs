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
	
	//props to be generated in the biome
	public PropPlacementData[] props;
	
}
