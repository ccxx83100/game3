using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SecondAlphaCollider : MonoBehaviour
{
	private GameObject go;
	private JumpGameMain JGM;
	DebugListLog dll = new DebugListLog();
	void Start()
	{
		go = GameObject.Find("GameMain");
		JGM = go.GetComponent<JumpGameMain>();
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 当たり判定
	/// </summary>
	///-------------------------------------------------------------------------------
	void OnCollisionEnter(Collision collision)
	{
		GameObject destroyPanel = collision.gameObject;
		if (destroyPanel.name != "Grid")
		{
			Destroy(destroyPanel);
		}
	}
}
