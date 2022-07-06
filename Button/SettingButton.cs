using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

///-------------------------------------------------------------------------------
/// <summary>
/// 設定画面のボタンセット
/// </summary>
///-------------------------------------------------------------------------------
public class SettingButton : MonoBehaviour
{
	///-------------------------------------------------------------------------------
	/// <summary>
	/// 設定ボタン
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClickSettingIn()
	{
		GameObject.FindGameObjectWithTag("SettingWindow").GetComponent<RectTransform>()
		.anchoredPosition = new Vector3(0.0f, 0.0f, -110.0f);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	///　外周・バッククリック時
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClickSettingOut()
	{
		GameObject.FindGameObjectWithTag("SettingWindow").GetComponent<RectTransform>()
		.anchoredPosition = new Vector3(1800.0f, 0.0f, -110.0f);

		GameObject.Find("BackGroundCollider").transform.position = new Vector3(0, 0, -0.5f);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// タイトルへ戻る
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClickTitle()
	{
		SceneManager.LoadScene("Scene_Title");
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	///　保存データを初期化
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClickDataReset()
	{
		Debug.Log("データリセット");
		string csvPath = Application.persistentDataPath + "/save_data.csv";
		File.WriteAllText(csvPath, "10000,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0");
		SceneManager.LoadScene("Scene_Title");
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// エディットモードへ
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClickEdit()
	{
		SceneManager.LoadScene("Scene_EditMode");
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// (仮) ジャンプゲーム
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClickJumpGame()
	{
		SceneManager.LoadScene("Scene_JumpMain");
	}
}
