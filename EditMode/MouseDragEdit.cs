using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------
/// <summary>
/// ドラッグ検知
/// 
/// </summary>
///-------------------------------------------------------------------------------
public class MouseDragEdit : MonoBehaviour
{
	private EditModeMain EMM;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタートしてコリジョンを移動
	/// </summary>
	///-------------------------------------------------------------------------------
	void Start()
	{
		SpriteRenderer _sr = GetComponent<SpriteRenderer>();
		_sr.material.color = _sr.material.color - new Color32(0, 0, 0, 255);
		Vector3 _pos = this.transform.position;
		_pos.z = -1;
		gameObject.transform.position = _pos;

		EMM = GameObject.Find("EditModeMain").GetComponent<EditModeMain>();
	}

	private int clickCount = 0;
	public bool startFlg = false;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// クリックしたとき
	/// </summary>
	///-------------------------------------------------------------------------------

	public void OnMouseDown()
	{
		clickCount++;
		Invoke("DoubleClick", 0.3f);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ダブルクリック
	/// </summary>
	///-------------------------------------------------------------------------------
	void DoubleClick()
	{
		if (clickCount != 2)
		{
			clickCount = 0;
			return;
		}
		else
		{
			clickCount = 0;
		}
		EMM.DoubleClick_method();
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// クリックしてドラッグをしている間、呼び出され続ける
	/// </summary>
	///-------------------------------------------------------------------------------
	void OnMouseDrag()
	{
		if (clickCount < 2)
		{
			EMM.MouseDrag_method();
		}
	}
}


