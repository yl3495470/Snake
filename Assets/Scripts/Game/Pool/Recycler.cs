using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Recycler.
/// </summary>
public class Recycler : MonoBehaviour {
	/// <summary>
	/// 是否可重复利用
	/// </summary>
	public bool isPooled = false;
	
	protected void Recycle () {
		Pool.Recycle (gameObject);
	}
}
