using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AlphaCollider : MonoBehaviour
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
		string destroyName = destroyPanel.name;
		//Debug.Log("destroyPanel:" + destroyPanel);
		int sarch = JGM.destroyList.IndexOf(destroyName);
		if (sarch >= 0 && destroyName != "Grid")
		{
			//			Destroy(destroyPanel);
			//			JGM.destroyList.Remove(destroyName);
			JGM.DestroyEffect(destroyPanel, destroyName);
		}
		//dll.List1DLog(JGM.destroyList);
	}
}
