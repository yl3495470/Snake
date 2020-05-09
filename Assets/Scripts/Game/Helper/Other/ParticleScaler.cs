using UnityEngine;
using System.Collections;

/// <summary>
/// Particle scaler.
/// </summary>
public class ParticleScaler : MonoBehaviour {
	
	private ParticleSystem ps;
	private float startSize;
	private float gravityModifier;
	private float startSpeed;
//	private Vector3 localPos;
	
	void Awake () {
		ps = GetComponent<ParticleSystem> ();
        if (ps == null)
            Debug.Log(name + " cannot find particle system");
		
		// store particle system config
		startSize = ps.startSize;
		gravityModifier = ps.gravityModifier;
		startSpeed = ps.startSpeed;
//		localPos = transform.localPosition;
	}
	
	/// <summary>
	/// Sets the scale.
	/// </summary>
	/// <param name='scale'>
	/// Scale.
	/// </param>
	public void SetScale (float scale) {
		if (ps == null) return;
		
		ps.startSize = startSize * scale;
		ps.gravityModifier = gravityModifier * scale;
		ps.startSpeed = startSpeed * scale;
//		transform.localPosition = localPos * scale;
	}
	
	/// <summary>
	/// Resets the scale.
	/// </summary>
	public void ResetScale () {
		SetScale (1f);
	}
}
