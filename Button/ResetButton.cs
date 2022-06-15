using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class ResetButton : MonoBehaviour
{
	//-----------------------------------------------
	//リセットボタンの処理
	//-----------------------------------------------

	public void OnClick()
	{
		GameObject _ga = GameObject.Find("GameMain");
		MainScript _ms = _ga.GetComponent<MainScript>();
		int _msStageNo = _ms.Serialize_StageNo;
		Debug.Log($"[ResetButton]StageNO--->{_msStageNo}");
		_ms.StartReset_Fnc(_msStageNo);

		_ms.CostDown(_ms.resetCost);
	}
}