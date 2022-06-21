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
		//_ms.Serialize_StageNo += 1;
		int _msStageNo = _ms.Serialize_StageNo;

		GameObject _dd = GameObject.Find("SelectButton");
		Dropdown dropDown = _dd.GetComponent<Dropdown>();
		dropDown.captionText.text = $"Stage {_ms.Serialize_StageNo}";
		dropDown.value = _msStageNo;


		///-------------------------------------------------------------------------------
		/// <summary>
		/// ほぼセレクトボタンと一緒の処理。一括にできないか検討
		/// </summary>
		///-------------------------------------------------------------------------------
		GameObject _bg = GameObject.Find("BackGroundCollider");
		Vector3 _bgPos = new Vector3(0, 1, -1);
		_bg.transform.position = _bgPos;

		GameObject[] ptcObjects;
		ptcObjects = GameObject.FindGameObjectsWithTag("ParticleObject");
		foreach (GameObject t in ptcObjects)
		{
			Destroy(t);
		}

		GameObject[] plObjects;
		plObjects = GameObject.FindGameObjectsWithTag("PanelObject");
		foreach (GameObject t in plObjects)
		{
			Destroy(t);
		}

		GameObject[] allObjects;
		allObjects = GameObject.FindGameObjectsWithTag("DestoryObject");
		foreach (GameObject t in allObjects)
		{
			Destroy(t);
		}

		_ms.Start(); //GameMain のスタート再実行

	}
}