using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnScroll : MonoBehaviour
{
	///-------------------------------------------------------------------------------
	/// <summary>
	/// ドロップダウンのスクロール中にコリダーを移動
	/// ※スライダーを抜けた時、どうやって戻すか検討
	/// </summary>
	///-------------------------------------------------------------------------------
	public void Check()
	{
		GameObject _bg = GameObject.Find("BackGroundCollider");
		_bg.transform.position = new Vector3(20, 0, -1);
	}
}