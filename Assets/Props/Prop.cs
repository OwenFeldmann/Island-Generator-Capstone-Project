using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
	public Transform childTransform;
	
	public void Setup(PropPlacementData propData)
	{
		this.name = propData.propName;
		
		GetComponentInChildren<MeshFilter>().mesh = propData.mesh;
		GetComponentInChildren<MeshRenderer>().materials = propData.materials;
		
		CapsuleCollider collider = GetComponent<CapsuleCollider>();
		if(propData.hasCollider)
		{
			collider.enabled = true;
			collider.radius = propData.colliderRadius;
			collider.height = propData.colliderHeight;
			collider.center = new Vector3(0, propData.colliderHeight/2, 0);
			childTransform.localPosition = new Vector3(0, 0, 0);
		}
		else
			collider.enabled = false;
		
		//Set random rotation
		transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
		//Adjust size slightly
		transform.localScale *= Random.Range(0.8f, 1.2f);
	}
}
