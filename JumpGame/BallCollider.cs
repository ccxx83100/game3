using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BallCollider : MonoBehaviour
{

	public GameObject go;
	JumpGameMain JGM;
	DebugListLog dll = new DebugListLog();
	void Start()
	{
		go = GameObject.Find("GameMain");
		JGM = go.GetComponent<JumpGameMain>();

	}

	//当たり判定
	void OnCollisionEnter(Collision collision)
	{
		JGM.ballJumpFlg = true;
		JGM.jumpCount = 0;
		GameObject destroyPanel = collision.gameObject;

		if (!JGM.destroyList.Contains(destroyPanel.name))
		{
			JGM.destroyList.Add(destroyPanel.name);
		}

		//dll.List1DLog(JGM.destroyList);
	}
}
