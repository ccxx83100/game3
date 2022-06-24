using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
		GameObject _goUI = GameObject.FindGameObjectWithTag("SettingWindow");
		RectTransform _rectTransform = _goUI.GetComponent<RectTransform>();
		_goUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0f, 0.0f, -110.0f);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	///　外周・バック
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnClickSettingOut()
	{
		GameObject _goUI = GameObject.FindGameObjectWithTag("SettingWindow");
		RectTransform _rectTransform = _goUI.GetComponent<RectTransform>();
		_goUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(1800.0f, 0.0f, -110.0f);

		GameObject _bg = GameObject.Find("BackGroundCollider");
		_bg.transform.position = new Vector3(0, 0, -0.5f);
	}
}
