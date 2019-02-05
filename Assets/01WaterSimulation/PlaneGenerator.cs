using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : MonoBehaviour {

	public float Size;
	public float Step;
	int numDiv;
	Vector3[] vertices;
	int[] triangles;
	int triangleIndex  = 0;

	Mesh mesh;

	// Use this for initialization
	void Start () {
		numDiv = (int) (Mathf.Floor(Size / Step) + 1f);
		vertices = new Vector3[numDiv * numDiv];
		triangles = new int[6 * (numDiv - 1) * (numDiv - 1)];
		triangleIndex = 0;
		mesh = GetComponent<MeshFilter>().mesh;
		GeneratePlane();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void GeneratePlane()
	{
		for (int i = 0; i < numDiv; i ++)
			for(int j = 0; j < numDiv; j ++)
				vertices[i * numDiv + j] = new Vector3(j * Step, 0f, i * Step);

		for (int i = 0; i < numDiv - 1; i ++)
		{
			for(int j = 0; j < numDiv - 1; j ++)
			{
				AddTriangle(i * numDiv + j, (i + 1) * numDiv + j, i * numDiv + j + 1);
				AddTriangle(i * numDiv + j + 1, (i + 1) * numDiv + j, (i + 1) * numDiv + j + 1);
			}
		}

		mesh.vertices = vertices;
		mesh.triangles = triangles;
	}

	void AddTriangle(int a, int b, int c)
	{
		triangles[triangleIndex] = a;
		triangles[triangleIndex+1] = b;
		triangles[triangleIndex+2] = c;
		
		triangleIndex += 3;
	}
}
