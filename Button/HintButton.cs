using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HintButton : MonoBehaviour
{
	CSVImporter csvi = new CSVImporter();
	GameObject totalScoreOBJ;
	TextMeshProUGUI totalScoreGUI;
	public void OnClick()
	{
		GameObject _ga = GameObject.Find("GameMain");
		MainScript _ms = _ga.GetComponent<MainScript>();
		_ms.Hint_Fnc();


	}
}

