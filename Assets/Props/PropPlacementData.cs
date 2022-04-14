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
