using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///-------------------------------------------------------------------------------
/// <summary>
/// NEXTボタンの処理
/// </summary>
///-------------------------------------------------------------------------------
public class NextButton : MonoBehaviour
{
	///-------------------------------------------------------------------------------
	/// <summary>
	/// NEXTボタンクリック
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClick()
	{
		GameObject _ga = GameObject.Find("GameMain");
		MainScript _ms = _ga.GetComponent<MainScript>();
		int _msStageNo = _ms.Serialize_StageNo;

		GameObject _dd = GameObject.Find("SelectButton");
		Dropdown dropDown = _dd.GetComponent<Dropdown>();
		dropDown.captionText.text = $"Stage {_ms.Serialize_StageNo}";
		dropDown.value = _msStageNo;

	}
}