using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
	public void OnClickTitle()
	{
		SceneManager.LoadScene("Scene_Title");
	}

	public void OnClickReset()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}