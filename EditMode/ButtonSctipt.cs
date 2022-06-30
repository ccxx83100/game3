using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSctipt : MonoBehaviour
{
	public void OnClickGnerate()
	{
		EditModeMain em = GameObject.Find("EditModeMain").GetComponent<EditModeMain>();
		em.EnterAndClick();
	}

	public void OnClickOutput()
	{
		EditModeMain em = GameObject.Find("EditModeMain").GetComponent<EditModeMain>();
		em.OutputScript();
	}

	public void OnClickGameMain()
	{
		SceneManager.LoadScene("Scene_GameMain");
	}
}
