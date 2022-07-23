using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
	GameObject HallCollider, GameMain;
	GolfGameMain MainScript;
	void Start()
	{
		HallCollider = GameObject.Find("HallCollider");
		GameMain = GameObject.Find("GameMain");
		MainScript = GameMain.GetComponent<GolfGameMain>();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject == HallCollider)
		{
			MainScript.BallManager(-1);
			MainScript.BallRemoveManager(this.gameObject);
		}
	}
}
