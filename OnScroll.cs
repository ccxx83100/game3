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
		Vector3 _bgPos = new Vector3(15, 1, -1);
		_bg.transform.position = _bgPos;
	}
}