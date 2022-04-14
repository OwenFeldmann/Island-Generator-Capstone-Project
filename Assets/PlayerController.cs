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
using UnityEngine.AI;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    
	private NavMeshAgent agent;
	private MeshCollider island;
	private Camera cam;
	private CinemachineVirtualCamera vCam;
	
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
		island = GameObject.Find("IslandGenerator").GetComponent<MeshCollider>();
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		vCam = GameObject.Find("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
		vCam.Follow = transform;
		vCam.LookAt = transform;
		
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			/*Debug.Log(ray);
			Debug.DrawRay(ray.origin, ray.direction * 300f, Color.red, 2f);
			Debug.Log(island.Raycast(ray, out hit, 300f));*/
			
			if(island.Raycast(ray, out hit, 300f))
			{
				//Debug.Log(hit.point);
				//Debug.DrawLine(ray.origin, hit.point);
				agent.SetDestination(hit.point);
			}
		}
    }
}
