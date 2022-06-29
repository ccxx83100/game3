using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------
/// <summary>
/// ドラッグ検知
/// </summary>
///-------------------------------------------------------------------------------
public class MouseDragEdit : MonoBehaviour
{
	private EditModeMain EMM;
	DebugArrayLog dal = new DebugArrayLog();
	public GameObject PanelPrefab, GridPrefab, CrossPrefab, BallPrefab;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタートしてコリジョンを移動
	/// </summary>
	///-------------------------------------------------------------------------------
	void Start()
	{
		SpriteRenderer _sr = GetComponent<SpriteRenderer>();
		//マウス用のコリジョンを透明にしてZ軸を手前にする
		_sr.material.color = _sr.material.color - new Color32(0, 0, 0, 255);
		Vector3 _pos = this.transform.position;
		_pos.z = -1;
		gameObject.transform.position = _pos;

		EMM = GameObject.Find("EditModeMain").GetComponent<EditModeMain>();
		Debug.Log(EMM);
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


		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		int _row = 0;
		int _col = 0;
		for (int i = 1; i < EMM.panelVecter2XY.GetLength(0) - 1; i++)
		{
			for (int j = 1; j < EMM.panelVecter2XY.GetLength(1) - 1; j++)
			{
				if (mousePos.x >= EMM.panelVecter2XY[i, j, 0] - EMM.panelSize && mousePos.x < EMM.panelVecter2XY[i, j, 0] + EMM.panelSize)
				{
					if (mousePos.y <= EMM.panelVecter2XY[i, j, 1] + EMM.panelSize && mousePos.y > EMM.panelVecter2XY[i, j, 1] - EMM.panelSize)
					{
						_row = i;
						_col = j;
						break;
					}
				}
			}
		}
		GameObject _go = GameObject.Find("Po_R" + _row + "C" + _col);
		// 1:Panel  2:Grid  3:Help  4:Cross
		if (EMM.panelArray[_row, _col] == 1 || EMM.panelArray[_row, _col] == 2)
		{
			EMM.panelArray[_row, _col] = 3;
			_go.GetComponent<SpriteRenderer>().color = new Color(200.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f);
		}
		else if (EMM.panelArray[_row, _col] == 3)
		{
			EMM.panelArray[_row, _col] = 4;
			GameObject __go = GameObject.Find("Po_R" + _row + "C" + _col);
			Destroy(__go);

			float panel_xPos = __go.transform.position.x;
			float panel_yPos = __go.transform.position.y;

			GameObject Po = Instantiate(CrossPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
			Po.name = "Co_" + "R" + _row + "C" + _col;
			Po.transform.localScale = new Vector2(EMM.panelScaleMin, EMM.panelScaleMin);
			dal.Array2DLog(EMM.panelArray);
		}
		else if (EMM.panelArray[_row, _col] == 4)
		{
			EMM.panelArray[_row, _col] = 2;
			GameObject __go = GameObject.Find("Co_R" + _row + "C" + _col);
			Destroy(__go);

			float panel_xPos = __go.transform.position.x;
			float panel_yPos = __go.transform.position.y;

			GameObject Po = Instantiate(GridPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
			Po.name = "Go_" + "R" + _row + "C" + _col;
			Po.transform.localScale = new Vector2(EMM.panelScaleMin, EMM.panelScaleMin);
			dal.Array2DLog(EMM.panelArray);
		}
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
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int _row = 0;
			int _col = 0;
			for (int i = 1; i < EMM.panelVecter2XY.GetLength(0) - 1; i++)
			{
				for (int j = 1; j < EMM.panelVecter2XY.GetLength(1) - 1; j++)
				{
					if (mousePos.x >= EMM.panelVecter2XY[i, j, 0] - EMM.panelSize && mousePos.x < EMM.panelVecter2XY[i, j, 0] + EMM.panelSize)
					{
						if (mousePos.y <= EMM.panelVecter2XY[i, j, 1] + EMM.panelSize && mousePos.y > EMM.panelVecter2XY[i, j, 1] - EMM.panelSize)
						{
							_row = i;
							_col = j;
							if (startFlg == false)
							{
								GameObject __go = GameObject.Find("Go_R" + _row + "C" + _col);

								float panel_xPos = __go.transform.position.x;
								float panel_yPos = __go.transform.position.y;

								GameObject Po = Instantiate(BallPrefab, new Vector3(panel_xPos, panel_yPos, -0.1f), Quaternion.identity) as GameObject;
								Po.name = "Ball";
								float ballScale = EMM.panelScaleMin * 2.0f;
								Po.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
								startFlg = true;
								EMM.AddText("startPos = new int[]{" + _col + "," + _row + "};");
								//startPos = new int[] { 1, 1 };
							}
							break;
						}
					}
				}
			}

			if (EMM.panelArray[_row, _col] == 2)
			{
				EMM.panelArray[_row, _col] = 1;
				GameObject _go = GameObject.Find("Go_R" + _row + "C" + _col);

				float panel_xPos = _go.transform.position.x;
				float panel_yPos = _go.transform.position.y;
				Destroy(_go);

				GameObject Po = Instantiate(PanelPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
				Po.name = "Po_" + "R" + _row + "C" + _col;
				Po.transform.localScale = new Vector2(EMM.panelScaleMin, EMM.panelScaleMin);
			}
		}

	}
	void OnMouseUp()
	{
		//Debug.Log("mouseUP");
	}



}


