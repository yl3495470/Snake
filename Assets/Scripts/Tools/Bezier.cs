using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Bezier : MonoBehaviour {

	Vector2[,] UV;

	public TraceControl GenerateController;
	public MeshFilter MeshGeneration;

	List<Vector3> verts = new List<Vector3>();

	void Start () {

		UV = new Vector2[GenerateController.sphere_m, GenerateController.sphere_n];
		for (int i = 0; i < GenerateController.sphere_m; i++)
		{ 
			for(int j = 0; j < GenerateController.sphere_n; j++)
			{
				float _u = (float)j /( GenerateController.sphere_n -1);
				float _v = (float)i /( GenerateController.sphere_m -1);
				UV[i, j] = new Vector2(_u, _v);
				// Debug.Log("uv: "+_u+"  "+_v);
			}
		}     

	}

	// Update is called once per frame
	void Update () {
		//to texture vertices
		int total =  GenerateController.sphere_m* GenerateController.sphere_n;
		int index=0;

		for (int i = 0; i < GenerateController.sphere_m; i++)
		{
			for (int j = 0; j < GenerateController.sphere_n; j++)
			{
				Vector3 _p = P (UV [i, j].x, UV [i, j].y);                  
				GenerateController.GetSphere (i, j).position = _p;          

			}
		}

		verts.Clear ();
		for (int i = 0; i < GenerateController.sphere_m; i++)
		{
			for (int j = 0; j < GenerateController.sphere_n; j++)
			{
				verts.Add (GenerateController.GetSphere (i, j).localPosition);
//				MeshGeneration.mesh.vertices [index++] = GenerateController.GetSphere (i, j).position;
			}
		}
		MeshGeneration.mesh.SetVertices (verts);

		index = 0;
	}

	float Factorial(int n)
	{ 
		float product=1;
		while(n!= 0)
		{
			product *= n;
			n--;
		}
		return product;
	}
	float Combin(int n,int k)
	{
		if(n>=k)
		{
			float result = Factorial(n) / (Factorial(k) * Factorial(n - k));
			return result;
		}
		else
		{
			return 0;

		}       
	}

	float BEZ(int k, int n, float u)
	{
		float result = Combin(n, k) * Mathf.Pow(u, k) * Mathf.Pow(1-u,n-k);
		return result;
	}

	//compute the position of the point with (u,v) image coordinate 
	Vector3 P(float u,float v)
	{
		int m = GenerateController.m;
		int n = GenerateController.n;
		float tempX = 0;
		float tempY = 0;
		float tempZ = 0;
		for (int j = 0; j < m; j++)
		{
			for (int k = 0; k < n; k++)
			{
				tempX += GenerateController.GetControlPoint (j, k).position.x * BEZ (j, m - 1, v) * BEZ (k, n - 1, u);
				tempY += GenerateController.GetControlPoint (j, k).position.y * BEZ (j, m - 1, v) * BEZ (k, n - 1, u);
				tempZ += GenerateController.GetControlPoint (j, k).position.z * BEZ (j, m - 1, v) * BEZ (k, n - 1, u);
			}
		}
		return new Vector3(tempX,tempY,tempZ);
	}

}