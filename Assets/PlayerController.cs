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
