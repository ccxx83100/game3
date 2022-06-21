using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------
/// <summary>
/// ヒントボタン
/// </summary>
///-------------------------------------------------------------------------------
public class HintButton : MonoBehaviour
{
	///-------------------------------------------------------------------------------
	/// <summary>
	/// ヒントボタンの処理
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClick()
	{
		GameObject _ga = GameObject.Find("GameMain");
		MainScript _ms = _ga.GetComponent<MainScript>();
		_ms.Hint_Fnc();
	}
}
