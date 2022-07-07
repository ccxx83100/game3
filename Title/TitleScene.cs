using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScene : MonoBehaviour
{
	//タイトル自動切り替え時間
	public float changeTime;
	public void Start()
	{
		//Invoke("ChangeScene", changeTime);

		RectTransform _tf = GameObject.Find("Canvas").GetComponent<RectTransform>();
		var canvasSize = _tf.sizeDelta;
		float _verX = canvasSize.x / 2 - 80.0f;
		float _varY = canvasSize.y / 2 * -1.0f + 110.0f;

		GameObject VersionObj = GameObject.Find("Version");
		VersionObj.GetComponent<RectTransform>()
		.anchoredPosition = new Vector2(_verX, _varY);

		VersionObj.GetComponent<TextMeshProUGUI>().text = "Version : " + Application.version + "\n developed by Takehiko Yamamamoto";

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