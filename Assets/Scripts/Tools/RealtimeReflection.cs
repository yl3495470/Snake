using UnityEngine;  
using System.Collections;  

public class RealtimeReflection : MonoBehaviour   
{  
	Camera reflectionCamera;  
	RenderTexture cubemap;  

	public Material targetMaterial;

	// Use this for initialization  
	void Start ()   
	{  
		GameObject go = new GameObject("Reflection Camera", typeof(Camera));  

		reflectionCamera = go.GetComponent<Camera>();  

		go.hideFlags = HideFlags.HideAndDontSave;  
		go.transform.position = transform.position;  
		go.transform.rotation = Quaternion.identity;  

		reflectionCamera.farClipPlane = 100;  
		reflectionCamera.enabled = false;  

		cubemap = new RenderTexture(128, 128, 16);  
		cubemap.isCubemap = true;  
		cubemap.hideFlags = HideFlags.HideAndDontSave;  

		targetMaterial.SetTexture("_Cubemap", cubemap);  

		reflectionCamera.transform.position = transform.position;  
		reflectionCamera.RenderToCubemap(cubemap, 63);  
	}  

	void RenderCubemap()  
	{  

	}  

	// Update is called once per frame  
	void Update ()  
	{  

	}  

	void LateUpdate()  
	{  
		reflectionCamera.transform.position = transform.position;  
		reflectionCamera.RenderToCubemap(cubemap, 63);  
	}  
}  