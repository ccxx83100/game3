using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{

	List<string> stageList;
	private int stageMax;
	private int _msStageNo;

	//-------------------------------------------------------------------
	//ドロップダウンリスト生成
	//-------------------------------------------------------------------
	void Start()
	{
		StageList stl = new StageList();
		stageMax = stl.switchStage;
		stageList = new List<string>();
		for (int i = 1; i <= stageMax; i++)
		{
			stageList.Add("Stage " + i);
		}

		Dropdown dropDown = GetComponent<Dropdown>();
		dropDown.ClearOptions();

		foreach (var opt in stageList)
		{
			Dropdown.OptionData optionData = new Dropdown.OptionData(opt);
			dropDown.options.Add(optionData);
		}

		GameObject _ga = GameObject.Find("GameMain");
		MainScript _ms = _ga.GetComponent<MainScript>();
		_msStageNo = _ms.Serialize_StageNo - 1;
		dropDown.captionText.text = $"Stage {_ms.Serialize_StageNo}";

		dropDown.value = _msStageNo;

	}

	//-------------------------------------------------------------------
	//ドロップダウン更新後処理
	//-------------------------------------------------------------------
	public void OnValueChanged(int value)
	{

		GameObject _bg = GameObject.Find("BackGroundCollider");
		Vector3 _bgPos = new Vector3(0, 1, -1);
		_bg.transform.position = _bgPos;
		GameObject _ga = GameObject.Find("GameMain");
		MainScript _ms = _ga.GetComponent<MainScript>();
		bool _flg = _ms.escapeFlg;

		if (!_flg)
		{
			///Debug.Log($"{value}番目の要素が選ばれた");
			int _msStageNo = value + 1;
			Debug.Log($"[StageSelect]StageNO--->{_msStageNo}");
			_ms.Serialize_StageNo = _msStageNo;

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
			Debug.Log(allObjects);
			foreach (GameObject t in allObjects)
			{
				Destroy(t);
			}

			_ms.Start(); //GameMain のスタート再実行
		}

	}

}
