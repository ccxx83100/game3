using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using System.Linq;

public class EditModeMain : MonoBehaviour
{
	public GameObject GridPrefab, BallPrefab, CrossPrefab, PanelPrefab;
	private TMP_InputField inputF_col, inputF_row, inputF_console;
	private DebugArrayLog dal = new DebugArrayLog();
	private CsvAccessEditMode csvAEM = new CsvAccessEditMode();

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
			EnterAndClick();
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 生成ボタンと、クリックの共通化
	/// </summary>
	///-------------------------------------------------------------------------------
	public void EnterAndClick()
	{
		if (inputF_col.text != "" && inputF_row.text != ""
		&& int.Parse(inputF_col.text) <= 12 && int.Parse(inputF_row.text) <= 12)
		{
			int col = int.Parse(inputF_col.text);
			int row = int.Parse(inputF_row.text);
			GenGlid(col, row);
		}
		else
		{
			AddTextConsole("Input Error");
		}
	}

	public int[,] panelArray;
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

		startFlg = false;

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
		PanelSetUP_Fnc();
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

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ダブルクリック時の処理
	/// →　✕パネル →　ヒントパネル　→　グリッド
	/// </summary>
	///-------------------------------------------------------------------------------
	public void DoubleClick_method()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		int _row = 0;
		int _col = 0;
		for (int i = 1; i < panelVecter2XY.GetLength(0) - 1; i++)
		{
			for (int j = 1; j < panelVecter2XY.GetLength(1) - 1; j++)
			{
				if (mousePos.x >= panelVecter2XY[i, j, 0] - panelSize && mousePos.x < panelVecter2XY[i, j, 0] + panelSize)
				{
					if (mousePos.y <= panelVecter2XY[i, j, 1] + panelSize && mousePos.y > panelVecter2XY[i, j, 1] - panelSize)
					{
						_row = i;
						_col = j;
						break;
					}
				}
			}
		}
		GameObject _go = GameObject.Find("Po_R" + _row + "C" + _col);
		// 1:Panel  2:Grid  3:Cross  4:Help
		if (panelArray[_row, _col] == 1 || panelArray[_row, _col] == 2)
		{
			panelArray[_row, _col] = 3;
			GameObject __go = GameObject.Find("Po_R" + _row + "C" + _col);
			Destroy(__go);

			float panel_xPos = __go.transform.position.x;
			float panel_yPos = __go.transform.position.y;

			GameObject Po = Instantiate(CrossPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
			Po.name = "Co_" + "R" + _row + "C" + _col;
			Po.transform.localScale = new Vector2(panelScaleMin, panelScaleMin);
		}
		else if (panelArray[_row, _col] == 3)
		{
			panelArray[_row, _col] = 4;

			GameObject __go = GameObject.Find("Co_R" + _row + "C" + _col);
			float panel_xPos = __go.transform.position.x;
			float panel_yPos = __go.transform.position.y;

			Destroy(__go);

			GameObject Po = Instantiate(PanelPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
			Po.name = "Po_" + "R" + _row + "C" + _col;
			Po.transform.localScale = new Vector2(panelScaleMin, panelScaleMin);
			Po.GetComponent<SpriteRenderer>().color = new Color(200.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f);
		}
		else if (panelArray[_row, _col] == 4)
		{
			panelArray[_row, _col] = 2;
			GameObject __go = GameObject.Find("Po_R" + _row + "C" + _col);

			float panel_xPos = __go.transform.position.x;
			float panel_yPos = __go.transform.position.y;
			Destroy(__go);

			GameObject Po = Instantiate(GridPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
			Po.name = "Go_" + "R" + _row + "C" + _col;
			Po.transform.localScale = new Vector2(panelScaleMin, panelScaleMin);
		}
	}

	public bool startFlg = false;
	private int[] output_startPos;
	private int[,] output_hintArray;
	private int[,] output_stageArray;
	///-------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	///-------------------------------------------------------------------------------
	public void MouseDrag_method()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		int _row = 0;
		int _col = 0;
		for (int i = 1; i < panelVecter2XY.GetLength(0) - 1; i++)
		{
			for (int j = 1; j < panelVecter2XY.GetLength(1) - 1; j++)
			{
				if (mousePos.x >= panelVecter2XY[i, j, 0] - panelSize && mousePos.x < panelVecter2XY[i, j, 0] + panelSize)
				{
					if (mousePos.y <= panelVecter2XY[i, j, 1] + panelSize && mousePos.y > panelVecter2XY[i, j, 1] - panelSize)
					{
						_row = i;
						_col = j;
						//スタート場所にインスタンスを生成
						if (startFlg == false)
						{
							GameObject __go = GameObject.Find("Go_R" + _row + "C" + _col);

							float panel_xPos = __go.transform.position.x;
							float panel_yPos = __go.transform.position.y;

							GameObject Po = Instantiate(BallPrefab, new Vector3(panel_xPos, panel_yPos, -0.1f), Quaternion.identity) as GameObject;
							Po.name = "Ball";
							float ballScale = panelScaleMin * 2.0f;
							Po.transform.localScale = new Vector3(ballScale, ballScale, ballScale);
							startFlg = true;
							output_startPos = new int[] { _row, _col };
						}
						break;
					}
				}
			}
		}

		if (panelArray[_row, _col] == 2)
		{
			panelArray[_row, _col] = 1;
			GameObject _go = GameObject.Find("Go_R" + _row + "C" + _col);

			float panel_xPos = _go.transform.position.x;
			float panel_yPos = _go.transform.position.y;
			Destroy(_go);

			GameObject Po = Instantiate(PanelPrefab, new Vector2(panel_xPos, panel_yPos), Quaternion.identity) as GameObject;
			Po.name = "Po_" + "R" + _row + "C" + _col;
			Po.transform.localScale = new Vector2(panelScaleMin, panelScaleMin);
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// コンソールにMAPスクリプトを出力 してデータを保存
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OutputScript()
	{
		try
		{
			int _out = output_startPos[0];
		}
		catch
		{
			Debug.Log("エラーの為抜けます");
			AddTextConsole("Error");
			return;
		}
		ClearConsole();
		AddTextConsole("case n:");
		AddTextConsole("	startPos = new int[] {" + output_startPos[0] + "," + output_startPos[1] + "};");
		AddTextConsole(StageToText());
		AddTextConsole(HintToText());
		AddTextConsole("	breakCount = " + output_breakCount + ";");
		AddTextConsole("break;");

		csvAEM.CsvSave(output_startPos, output_hintArray, output_breakCount, output_stageArray);
		csvAEM.CsvLoad();
	}

	private int output_breakCount = 0;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// ヒント配列のテキスト化 ついでに -BraekCount-
	/// </summary>
	///-------------------------------------------------------------------------------
	string HintToText()
	{
		//--
		output_breakCount = 0;
		//--
		int hintCount = 0;
		for (int i = 0; i < panelArray.GetLength(0); i++)
		{
			for (int j = 0; j < panelArray.GetLength(1); j++)
			{
				if (panelArray[i, j] == 4)
				{
					hintCount++;
				}
			}
		}
		output_hintArray = new int[hintCount, 2];
		int count = 0;
		for (int i = 0; i < panelArray.GetLength(0); i++)
		{
			for (int j = 0; j < panelArray.GetLength(1); j++)
			{
				if (panelArray[i, j] == 4)
				{
					output_hintArray[count, 0] = i;
					output_hintArray[count, 1] = j;
					count++;
				}
			}
		}
		string hintText = "	hintArray = new int[,] {";
		for (int i = 0; i < output_hintArray.GetLength(0); i++)
		{
			output_breakCount++;
			for (int j = 0; j < output_hintArray.GetLength(1); j++)
			{
				if (j == 0)
				{
					hintText += "{";
					hintText += output_hintArray[i, j] + ",";
				}
				if (j == 1)
				{
					hintText += output_hintArray[i, j];
					if (i == output_hintArray.GetLength(0) - 1)
					{
						hintText += "}";
					}
					else
					{
						hintText += "},";
					}
				}
			}
		}
		hintText += "};";
		return hintText;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ステージ配列のテキスト化
	/// </summary>
	///-------------------------------------------------------------------------------
	string StageToText()
	{
		int[,] _panelArray = panelArray;    //参照型なので、入れ替える
		output_stageArray = new int[_panelArray.GetLength(0), _panelArray.GetLength(1)];
		for (int i = 0; i < output_stageArray.GetLength(0); i++)
		{
			for (int j = 0; j < output_stageArray.GetLength(1); j++)
			{
				output_stageArray[i, j] = _panelArray[i, j];
			}
		}

		string stageText = "	stageArray = new int[,]\n	{\n";
		for (int i = 0; i < output_stageArray.GetLength(0); i++)
		{
			for (int j = 0; j < output_stageArray.GetLength(1); j++)
			{
				//1:Panel  2:Grid  3:Cross  4:Hint
				switch (output_stageArray[i, j])
				{
					case 2:
						output_stageArray[i, j] = 0;
						break;
					case 3:
						output_stageArray[i, j] = 0;
						break;
					case 4:
						output_stageArray[i, j] = 1;
						break;
					default:
						break;
				}
			}
		}

		for (int i = 0; i < output_stageArray.GetLength(0); i++)
		{
			for (int j = 0; j < output_stageArray.GetLength(1); j++)
			{
				if (j == 0)
				{
					stageText += "						{";
				}
				//stageText += "{";
				if (j == output_stageArray.GetLength(1) - 1)
				{
					if (i == output_stageArray.GetLength(0) - 1)
					{
						stageText += output_stageArray[i, j] + "}\n";
					}
					else
					{
						stageText += output_stageArray[i, j] + "},\n";
					}
				}
				else
				{
					stageText += output_stageArray[i, j] + ",";
				}
			}
		}
		stageText += "	};";

		return stageText;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// コンソールクリア
	/// </summary>
	///-------------------------------------------------------------------------------
	public void ClearConsole()
	{
		inputF_console.text = "";
		Scrollbar sb = GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
		sb.value = -5.0f;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// コンソールにテキストの追加
	/// </summary>
	///-------------------------------------------------------------------------------
	public void AddTextConsole<T>(T str)
	{
		inputF_console.text += (str + "\n").ToString();
		GetRows(inputF_console.text);
		Scrollbar sb = GameObject.Find("Scrollbar").GetComponent<Scrollbar>();
		sb.value = -5.0f;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 行を取得 \n の数を検索
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

}









//継承テスト
public class InheritanceTest : MainScript
{
	private string var_str;
	private int var_int;
	public InheritanceTest(string var_str, int var_int)
	{
		this.var_str = "hoge";
		this.var_int = 10;
		string test = this.var_str + this.var_int;
	}
}


/*
public : 全てのクラスからアクセス可能
protected : 自身と子クラス(派生クラス)からアクセス可能
private : 自身からだけアクセス可能
オーバーライド?
抽象クラス

partial
*/



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