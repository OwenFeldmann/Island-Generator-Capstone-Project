using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoGenerator : MonoBehaviour
{
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
	
	public void BuildVolcano(Vector3[] vertices, Color[] colors, float seaLevel)
	{
		Circle rim = new Circle(centerX, centerZ, rimRadius);
		currentSpread = startSpread;
		iterationCurrentRadius = iterationStartRadius;
		
		for(int i = 0; i < iterations; i++)
		{
			//find a point on the volcano rim
			Vector2 rimPoint = rim.RandomPointOnCircle();
			//find a point within allowable spread radius of the rim point
			Vector2 iterationCenter = new Circle(rimPoint, currentSpread).RandomPointOnCircle();
			
			Circle iterationCircle = new Circle(iterationCenter, iterationCurrentRadius);
			
			for(int p = 0; p < vertices.Length; p++)
			{
				if(iterationCircle.ContainsPoint(vertices[p]))
				{
					vertices[p].y += iterationStrength;
					if(vertices[p].y >= seaLevel)
						colors[p] = stoneColor;
				}
			}
			
			currentSpread -= (startSpread - endSpread) / iterations;
			iterationCurrentRadius -= (iterationStartRadius - iterationEndRadius) / iterations;
		}
		
		for(int p = 0; p < vertices.Length; p++)
		{
			if(rim.ContainsPoint(vertices[p]) && vertices[p].y < lavaLevel)
				colors[p] = lavaColor;
		}
		
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
