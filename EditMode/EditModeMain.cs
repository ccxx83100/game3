using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditModeMain : MonoBehaviour
{
	public GameObject GridPrefab, BallPrefab;
	private TMP_InputField inputF_col, inputF_row, inputF_console;
	private DebugArrayLog dal = new DebugArrayLog();

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタート
	/// </summary>
	///-------------------------------------------------------------------------------
	void Start()
	{
		inputF_col = GameObject.Find("InputField(columnX1)").GetComponent<TMP_InputField>();
		inputF_row = GameObject.Find("InputField(rowY0)").GetComponent<TMP_InputField>();
		inputF_console = GameObject.Find("InputField(console)").GetComponent<TMP_InputField>();
		inputF_col.Select();
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// UPDATE
	/// </summary>
	///-------------------------------------------------------------------------------
	void Update()
	{
		///-------------------------------------------------------------------------------
		/// <summary>
		/// タブ移動
		/// </summary>
		///-------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (inputF_col.GetComponent<TMP_InputField>().isFocused == true)
			{
				inputF_row.Select();
			}
			else if (inputF_row.GetComponent<TMP_InputField>().isFocused == true)
			{
				inputF_col.Select();
			}
		}

		///-------------------------------------------------------------------------------
		/// <summary>
		///　エンターで生成
		/// </summary>
		///-------------------------------------------------------------------------------
		if (Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			Debug.Log(inputF_col.text);
			if (inputF_col.text != null && inputF_row.text != null)
			{
				EnterAndClick();
			}
			else
			{
				Debug.Log("エラー");
			}
		}
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// 生成ボタンと、クリックの共通化
	/// </summary>
	///-------------------------------------------------------------------------------
	public void EnterAndClick()
	{
		if (inputF_col.text != "" && inputF_row.text != "")
		{
			int col = int.Parse(inputF_col.text);
			int row = int.Parse(inputF_row.text);
			GenGlid(col, row);
		}
		else
		{
			AddText("Error:Input");
		}
	}

	public int[,] panelArray;      //OK
	private int[] nowPosition;      //OK
	private string[,] goArray;
	private float panelScale;
	private float defaultX, defaultY;
	public float panelOneSize, panelScaleMin;
	public float[,,] panelVecter2XY;
	public float panelSize;

	GameObject[] ballObjects;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// グリッドの生成
	/// </summary>
	///-------------------------------------------------------------------------------
	private void GenGlid(int _col, int _row)
	{
		//----------------
		GameObject[] pnlObjects;
		pnlObjects = GameObject.FindGameObjectsWithTag("PanelObject");
		foreach (GameObject i in pnlObjects)
		{
			Destroy(i);
		}

		GameObject delBall = GameObject.Find("Ball");
		if (delBall != null)
		{
			Destroy(delBall);
		}

		MouseDragEdit _MD = GameObject.Find("BackGroundCollider2").GetComponent<MouseDragEdit>();
		_MD.startFlg = false;

		int colAll = _col + 2;
		int rowAll = _row + 2;
		panelArray = new int[rowAll, colAll];
		goArray = new string[rowAll, colAll];
		panelVecter2XY = new float[rowAll, colAll, 2];
		float cameraSize = Camera.main.orthographicSize;
		int panelNumX = colAll;
		int panelNumY = rowAll;
		if (panelNumX >= panelNumY)
		{
			panelScale = 0.50f / 1.125f / (panelNumX - 2);
		}
		else
		{
			panelScale = 0.50f / 1.125f / (panelNumY - 2);
		}
		panelSize = panelScale * cameraSize;
		defaultX = (panelSize * panelNumX - panelSize) * -1;
		defaultY = (panelSize * panelNumY - panelSize);
		panelScaleMin = panelScale * 1.0f;                   //マージン分を縮小する
		panelOneSize = (panelScale * cameraSize) * 2;        //パネルの移動(複製パネルの移動座標)
		for (int i = 0; i < panelArray.GetLength(0); i++)
		{
			for (int j = 0; j < panelArray.GetLength(1); j++)
			{
				if (i == 0 || i == panelArray.GetLength(0) - 1 || j == 0 || j == panelArray.GetLength(1) - 1)
				{
					panelArray[i, j] = 0;
				}
				else
				{
					panelArray[i, j] = 2;                   // ( 2 ) にしています
				}
			}
		}
		dal.Array2DLog(panelArray);
		PanelSetUP_Fnc();

	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// 行を取得
	/// </summary>
	///-------------------------------------------------------------------------------
	private void GetRows(string str)
	{
		string before = str;
		string after = str.Replace("\n", "");
		int rows = before.Length - after.Length;
		Vector2 rowhight = new Vector2(0.0f, (33.3f * (rows + 1)) - 270f - 33.3f);      //細かい数値は手動
		RectTransform rt = GameObject.Find("TextArea").GetComponent<RectTransform>();
		rt.sizeDelta = rowhight;
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// コンソールにテキストの追加
	/// </summary>
	///-------------------------------------------------------------------------------
	public void AddText<T>(T str)
	{
		inputF_console.text += (str + "\n").ToString();
		GetRows(inputF_console.text);
		Scrollbar sb = GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
		sb.value = -5.0f;
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// パネル配置　※共通化したかったけど、メインをインスタンスにするとバグるので保留
	/// </summary>
	///-------------------------------------------------------------------------------
	public void PanelSetUP_Fnc()
	{
		GameObject Po;
		for (int i = 0; i < panelArray.GetLength(0); i++)
		{
			for (int j = 0; j < panelArray.GetLength(1); j++)
			{
				//インスタンスの生成位置
				float panel_xPos = defaultX + (j * panelOneSize);
				float panel_yPos = defaultY + (i * panelOneSize) * -1;

				if (panelArray[i, j] >= 1)
				{
					// プレハブを指定位置に生成
					Po = Instantiate(GridPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
					Po.name = "Go_" + "R" + i + "C" + j;
					Po.transform.localScale = new Vector2(panelScaleMin, panelScaleMin);

					//配列にインスタンス名を追加
					goArray[i, j] = Po.name;
				}
				panelVecter2XY[i, j, 0] = panel_xPos;
				panelVecter2XY[i, j, 1] = panel_yPos;
			}
		}
	}
}


/*
				startPos = new int[] { 1, 1 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,0,0,0,0,0,0}
				};
				hintArray = new int[,] { };
				breakCount = 0;
				*/