using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraceControl : MonoBehaviour {

	public int m;
	public int n;

	public int sphere_m;		
	public int sphere_n;	

	public Transform[] controlPoints;
	[HideInInspector]
	public GameObject[] Spheres;
	public Transform SphereParent;

	RawImage r;

	public Transform GetControlPoint(int pM, int pN)
	{
		if (pM >= m || pN >= n)
			return null;
		return controlPoints [pM * n + pN];
	}

	public Transform GetSphere(int pM, int pN)
	{
		if (pM * pN >= Spheres.Length)
			return null;
		return Spheres [pM * sphere_n + pN].transform;
	}

	// Use this for initialization
	void Start () {
		Spheres = new GameObject[sphere_m * sphere_n];
		for (int i = 0; i < sphere_m * sphere_n; ++i) {
			Spheres [i] = new GameObject (i.ToString());
			Spheres [i].transform.parent = SphereParent;
		}
	}
	
//	// Update is called once per frame
//	void OnDrawGizmos() {
//		Gizmos.color = Color.yellow;
//		for (int i = 0; i < controlPoints.Length; ++i) {
//			Gizmos.DrawWireCube (controlPoints [i].position, Vector3.one * 0.2f);
//		}
//
//		Gizmos.color = Color.blue;
//		for (int i = 0; i < Spheres.Length; ++i) {
//			if(Spheres [i] != null)
//				Gizmos.DrawSphere (Spheres [i].transform.position, 0.2f);
//		}
//	}
}
