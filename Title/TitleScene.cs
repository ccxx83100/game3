using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
	//タイトル自動切り替え時間
	public float changeTime;
	void Start()
	{
		Invoke("ChangeScene", changeTime);
	}

	void Update()
	{
		//クリックの処理
		if (Input.GetMouseButtonDown(0))
		{
			Invoke("ChangeScene", 0.2f);
		}
	}

	void ChangeScene()
	{
		SceneManager.LoadScene("Scene_GameMain");
	}

}