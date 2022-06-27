using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditModeMain : MonoBehaviour
{

	public GameObject PanelPrefab;
	private TMP_InputField inputF_col, inputF_row, inputF_startX, inputF_startY, inputF_console;
	private DebugArrayLog dal = new DebugArrayLog();

	void Start()
	{
		inputF_col = GameObject.Find("InputField(columnX1)").GetComponent<TMP_InputField>();
		inputF_row = GameObject.Find("InputField(rowY0)").GetComponent<TMP_InputField>();
		inputF_startX = GameObject.Find("InputField(StartPosX)").GetComponent<TMP_InputField>();
		inputF_startY = GameObject.Find("InputField(StartPosY)").GetComponent<TMP_InputField>();
		inputF_console = GameObject.Find("InputField(console)").GetComponent<TMP_InputField>();
		inputF_col.Select();
	}

	// Update is called once per frame
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
				inputF_startX.Select();
			}
			else if (inputF_startX.GetComponent<TMP_InputField>().isFocused == true)
			{
				inputF_startY.Select();
			}
			else if (inputF_startY.GetComponent<TMP_InputField>().isFocused == true)
			{
				inputF_col.Select();
			}
			else
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
			if (inputF_col.text != null && inputF_row.text != null && inputF_startX.text != null && inputF_startY.text != null)
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
		if (inputF_col.text != "" && inputF_row.text != "" && inputF_startX.text != "" && inputF_startY.text != "")
		{
			int col = int.Parse(inputF_col.text);
			int row = int.Parse(inputF_row.text);
			int startX = int.Parse(inputF_startX.text);
			int startY = int.Parse(inputF_startY.text);
			GenGlid(col, row, startX, startY);
		}
		else
		{
			AddText("Error:Input");
		}
	}


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
					Po = Instantiate(PanelPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
					Po.name = "Po_" + "R" + i + "C" + j;
					Po.transform.localScale = new Vector2(panelScaleMin, panelScaleMin);

					//配列にインスタンス名を追加
					goArray[i, j] = Po.name;
				}
				panelVecter2XY[i, j, 0] = panel_xPos;
				panelVecter2XY[i, j, 1] = panel_yPos;
			}
		}
	}

	private int[,] panelArray;      //OK
	private int[] nowPosition;      //OK
	private string[,] goArray;
	private float panelScale;
	private float defaultX, defaultY;
	private float panelOneSize, panelScaleMin;
	private float[,,] panelVecter2XY;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// グリッドの生成
	/// </summary>
	///-------------------------------------------------------------------------------
	private void GenGlid(int _col, int _row, int _startX, int _startY)
	{
		int[] nowPosition = { _startX, _startY };
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
		float panelSize = panelScale * cameraSize;
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
					panelArray[i, j] = 1;
				}
			}
		}
		dal.Array2DLog(panelArray);

		MainScript _ms = new MainScript();
		PanelSetUP_Fnc();

		AddText((_col + ":" + _row + "\n" + _startX + ":" + _startY + "\n"));
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
		Vector2 rowhight = new Vector2(0.0f, (33.3f * (rows + 1)) - 270f - 33.3f);
		RectTransform rt = GameObject.Find("TextArea").GetComponent<RectTransform>();
		rt.sizeDelta = rowhight;
	}



	///-------------------------------------------------------------------------------
	/// <summary>
	/// コンソールにテキストの追加
	/// </summary>
	///-------------------------------------------------------------------------------
	private void AddText<T>(T str)
	{
		inputF_console.text += (str + "\n").ToString();
		GetRows(inputF_console.text);
		Scrollbar sb = GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
		sb.value = -5.0f;
	}
}

