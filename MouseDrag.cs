using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
マウス操作検知

最小ドラッグ距離
minMoveMouse

*/

public class MouseDrag : MonoBehaviour
{
	public static string mouseLRUD;
	public float minMoveMouse = 10.0f;
	public float sw_minMoveMouse = 0.1f;

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


	public Vector3 startPos;
	public Vector3 endPos;

	public Vector3 sw_startPos;
	//-------------------------------------------------------------------------------------------------------------------------
	//クリックしたとき
	//-------------------------------------------------------------------------------------------------------------------------
	public void OnMouseDown()
	{
		//Debug.Log("Event-----OnMouseDown-----");
		startPos = Input.mousePosition;
		sw_startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Debug.Log(sw_startPos);
	}


	//-------------------------------------------------------------------------------------------------------------------------
	//クリックしてドラッグをしている間、呼び出され続ける
	//-------------------------------------------------------------------------------------------------------------------------
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

		//Debug.Log(mathAbsX + " : " + mathAbsY);


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


//-------------------------------------------------------------------------------------------------------------------------
//クリックしてから、指を離したとき
//-------------------------------------------------------------------------------------------------------------------------
/*
	void OnMouseUp()
	{

				GameObject _ga = GameObject.Find("GameMain");
				MainScript _ms = _ga.GetComponent<MainScript>();
				if (_ms.autoRunFlg == false || _ms.autoRunFlg == true)
				{

					//Debug.Log("Event-----OnMouseUp-----");
					endPos = Input.mousePosition;
					float moveX = startPos.x - endPos.x;
					float moveY = startPos.y - endPos.y;

					float mathAbsX = Mathf.Abs(moveX);
					float mathAbsY = Mathf.Abs(moveY);

					if (mouseLRUD == "STOP")
					{
						if (mathAbsX > mathAbsY && mathAbsX >= minMoveMouse)
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
						else if (mathAbsX < mathAbsY && mathAbsY >= minMoveMouse)
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
	*/





/*
------------------------------------------------
-------------使わないマウスライブラリ-------------
------------------------------------------------

    //クリックしてドラッグをしている間、呼び出され続ける
    void OnMouseDrag()
    {
        // Debug.Log(Input.mousePosition.x);
    }

    //マウスが乗った時
    void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
    }

    //マウスが乗っている間、呼び出され続ける
    void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
    }

    //マウスが離れたとき
    void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
    }
    
        //クリックした後、”オブジェクト上で”　指を離したとき
        void OnMouseUpAsButton()
        {
            Debug.Log("MouseUpAsButton");
        }
*/