using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoGenerator : MonoBehaviour
{
	public bool generateVolcano = true;
	
	public bool chooseRandomCenter = true;
    //Center and radius on the volcano rim
	public float centerX = 80f;
	public float centerZ = 80f;
	public float rimRadius = 15f;
	//Points inside the volcano will be asigned the lava color if below this height
	public float lavaLevel = 4f;
	
	//how many circle iterations to build up
	public int iterations = 200;
	//how much each iteration raises the land
	public float iterationStrength = 0.1f;
	//radius of the first circle iteration starts here (larger)
	public float iterationStartRadius = 5f;
	private float iterationCurrentRadius;
	//radius of the last circle iteration ends here (smaller)
	public float iterationEndRadius = 1.5f;
	//max distance each circle iteration can be from the volcano rim at the first iteration (larger)
	public float startSpread = 5f;
	private float currentSpread;
	//distance each circle iteration from the volcano rim at the last iteration (smaller)
	public float endSpread = 0f;
	
	public Color stoneColor;
	public Color lavaColor;
	
	public IEnumerator BuildVolcano()
	{
		if(!generateVolcano)
			yield break;
		
		MeshGenerator mg = GetComponent<MeshGenerator>();
		
		if(chooseRandomCenter)
		{
			centerX = Random.Range(MaxVolcanoRadius(), mg.xSize-MaxVolcanoRadius());
			centerZ = Random.Range(MaxVolcanoRadius(), mg.zSize-MaxVolcanoRadius());
		}
		
		Circle volcanoMaxCircle = new Circle(centerX, centerZ, MaxVolcanoRadius());
		int[] volcanoVertices = new int[mg.vertices.Length];
		int nextVolcanoVertexIndex = 0;
		
		/*
		Figure out which vertices are affected by the volcano and 
		flatten the ground to prevent brown spiky mountains to raise up
		*/
		for(int i = 0; i < mg.vertices.Length; i++)
		{
			if(volcanoMaxCircle.ContainsPoint(mg.vertices[i]))
			{
				volcanoVertices[nextVolcanoVertexIndex] = i;
				nextVolcanoVertexIndex++;
				//averages height down towards sealevel
				mg.vertices[i].y = (mg.vertices[i].y + 2*mg.seaLevel)/3;
				mg.vertexColors[i] = stoneColor;
			}
		}
		
		Circle rim = new Circle(centerX, centerZ, rimRadius);
		currentSpread = startSpread;
		iterationCurrentRadius = iterationStartRadius;
		
		//Build the volcano
		for(int i = 0; i < iterations; i++)
		{
			//find a point on the volcano rim
			Vector2 rimPoint = rim.RandomPointOnCircle();
			//find a point within allowable spread radius of the rim point
			Vector2 iterationCenter = new Circle(rimPoint, currentSpread).RandomPointOnCircle();
			
			Circle iterationCircle = new Circle(iterationCenter, iterationCurrentRadius);
			
			for(int p = 0; p < nextVolcanoVertexIndex; p++)
			{
				if(iterationCircle.ContainsPoint(mg.vertices[volcanoVertices[p]]))
				{
					mg.vertices[volcanoVertices[p]].y += iterationStrength;
				}
			}
			
			currentSpread -= (startSpread - endSpread) / iterations;
			iterationCurrentRadius -= (iterationStartRadius - iterationEndRadius) / iterations;
			
			if(mg.animateGeneration && i % 100 == 0)
			{//wait after several iterations
				mg.UpdateMesh();
				yield return new WaitForSeconds(0.05f);
			}
		}
		
		//Color the center a lava color
		for(int p = 0; p < nextVolcanoVertexIndex; p++)
		{
			if(rim.ContainsPoint(mg.vertices[volcanoVertices[p]]) && mg.vertices[volcanoVertices[p]].y < lavaLevel)
				mg.vertexColors[p] = lavaColor;
		}
		
	}
	
	public float MaxVolcanoRadius()
	{
		return rimRadius + startSpread + iterationStartRadius;
	}
	
	/*
	Debug visualization of volcano center
	*/
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(new Vector3(centerX, 15f, centerZ), 5);
	}
	
}

public class Circle
{
	public Vector2 center;
	public float radius;
	
	public Circle(Vector2 center, float radius)
	{
		this.center = center;
		this.radius = radius;
	}
	
	public Circle(float x, float z, float radius)
	{
		center = new Vector2(x, z);
		this.radius = radius;
	}
	
	public Vector2 RandomPointOnCircle()
	{
		float angle = Random.Range(0f, Mathf.PI * 2);
		return center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
	}
	
	public bool ContainsPoint(Vector3 p)
	{
		Vector2 point = new Vector2(p.x, p.z);
		return (center - point).magnitude <= radius;
	}
	
}
