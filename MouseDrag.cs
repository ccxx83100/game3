using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------
/// <summary>
/// ドラッグ検知
/// </summary>
///-------------------------------------------------------------------------------
public class MouseDrag : MonoBehaviour
{
	public static string mouseLRUD;
	public float sw_minMoveMouse = 0.1f;    //ドラックの最小距離

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタートしてコリジョンを移動
	/// </summary>
	///-------------------------------------------------------------------------------
	void Start()
	{
		mouseLRUD = "STOP";
		//マウス用のコリジョンを透明にしてZ軸を手前にする
		SpriteRenderer _sr = GetComponent<SpriteRenderer>();
		_sr.material.color = _sr.material.color - new Color32(0, 0, 0, 255);
		Vector3 _pos = gameObject.transform.position;
		_pos.z = -1;
		gameObject.transform.position = _pos;
	}

	public Vector3 sw_startPos;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// クリックしたとき
	/// </summary>
	///-------------------------------------------------------------------------------

	public void OnMouseDown()
	{
		Vector3 startPos = Input.mousePosition;
		sw_startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Debug.Log(sw_startPos);
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// クリックしてドラッグをしている間、呼び出され続ける
	/// </summary>
	///-------------------------------------------------------------------------------
	void OnMouseDrag()
	{
		// Debug.Log(Input.mousePosition.x);
		float _nowPosX = Input.mousePosition.x;
		float _nowPosY = Input.mousePosition.y;

		Vector3 _convert = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		float sw_nowPosX = _convert.x;
		float sw_nowPosY = _convert.y;

		float moveX = sw_startPos.x - sw_nowPosX;
		float moveY = sw_startPos.y - sw_nowPosY;

		float mathAbsX = Mathf.Abs(moveX);
		float mathAbsY = Mathf.Abs(moveY);

		if (mouseLRUD == "STOP")
		{
			if (mathAbsX > mathAbsY && mathAbsX >= sw_minMoveMouse)
			{
				if (moveX < 0)
				{
					//Debug.Log("右移動");
					mouseLRUD = "R";
				}
				else
				{
					//Debug.Log("左移動");
					mouseLRUD = "L";
				}
			}
			else if (mathAbsX < mathAbsY && mathAbsY >= sw_minMoveMouse)
			{
				if (moveY < 0)
				{
					//Debug.Log("上移動");
					mouseLRUD = "U";
				}
				else
				{
					//Debug.Log("下移動");
					mouseLRUD = "D";
				}
			}
		}
	}
}

