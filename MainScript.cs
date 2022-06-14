using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class MainScript : MonoBehaviour
{
	public GameObject PanelPrefab, BallPrefab, ParticlePrefab;
	private float panelOneSize, panelScale, xMin, xMax, yMin, yMax;
	private float defaultX, defaultY, panel_xPos, panel_yPos, panelScaleMin;
	private float[,,] panelVecter2XY;
	private string[,] goArray;
	private int[,] panelArray, hintArray;
	private int[] nowPosition;
	private int breakCount;
	[SerializeField]
	public int Serialize_StageNo;
	DebugArrayLog dal = new DebugArrayLog();
	CSVImporter csvi = new CSVImporter();

	//-------------------------------------------------------------------------------------------------------------------------
	//GAME START
	//-------------------------------------------------------------------------------------------------------------------------
	private bool startFlg;  //スタートフラグ
	private bool endFlg;    //エンドフラグ
	public bool escapeFlg = true;   //ステージセレクトバグ回避フラグ
	public void Start()
	{
		csvi.CsvLoad();     //csvをロード
		if (Serialize_StageNo >= 1)
		{
			escapeFlg = false;
			startFlg = true;
			StartReset_Fnc(Serialize_StageNo);
			startFlg = false;
			AnimationStop();
		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//UPDATE
	//-------------------------------------------------------------------------------------------------------------------------
	private Vector3 mouseClickPosition;
	void Update()
	{
		if (Bo)
		{
			BoMove_Fnc();
			ScoreUP_Fnc();
		}
		string md_mouseLRUD = MouseDrag.mouseLRUD;
		if (md_mouseLRUD == "STOP")
		{
			//クリック取得
			if (Input.GetMouseButtonDown(0))
			{
				mouseClickPosition = Input.mousePosition;
				BreakClick_Fnc();
			}
		}
		if (panelArray != null)
		{
			int iCount = 0;
			foreach (int i in panelArray)
			{
				if (i == 1) iCount++;
			}
			if (iCount == 1)
			{
				AnimationPlay();
				Clear_func();
			}
		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//クリア後の処理
	//-------------------------------------------------------------------------------------------------------------------------
	void Clear_func()
	{
		if (endFlg)
		{
			nowScore = nowScore + clearPoint + onePanelPoint;
			nowScoreView = nowScore;
			nowScoreGUI = nowScoreOBJ.GetComponent<TextMeshProUGUI>();
			nowScoreGUI.text = nowScoreView.ToString();
			endFlg = false;

			if (nowScore > bestScore)
			{
				int[] _loadArray = csvi.loadScoreArray;
				_loadArray[Serialize_StageNo] = nowScore;
				dal.Array1DLog(_loadArray);

				bestScoreGUI = bestScoreOBJ.GetComponent<TextMeshProUGUI>();
				bestScoreGUI.text = nowScore.ToString();

				int _addScore = nowScore - bestScore;
				totalScore += _addScore;
				_loadArray[0] = totalScore;
				totalScoreGUI = totalScoreOBJ.GetComponent<TextMeshProUGUI>();
				totalScoreGUI.text = totalScore.ToString();

				csvi.CsvSave(_loadArray);
			}
		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//START_RESET
	//-------------------------------------------------------------------------------------------------------------------------
	private GameObject Bo;
	private int panelNum, panelNumX, panelNumY;
	public void StartReset_Fnc(int stgNo)
	{
		//-------------------------------------------------------------------------------------------------------------------------
		//共通
		//-------------------------------------------------------------------------------------------------------------------------

		float screenX = Screen.width;
		float screenY = Screen.height;
		Camera MainCamera = Camera.main;
		float cameraSize = MainCamera.orthographicSize;
		float ratio = screenX / screenY;

		//ステージ設定クラス
		StageList stl = new StageList();
		var (_startPos, _stageArray, _hintArray, _stageScale, _breakCount) = stl.StageSetUP(stgNo);

		//現在地配列 [] [,]
		nowPosition = _startPos;
		panelArray = _stageArray;
		hintArray = _hintArray;
		breakCount = _breakCount;

		panelScale = _stageScale;   //基本設定変数
		panelScaleMin = panelScale * 1.0f;  //マージン分を縮小する

		panelNumX = panelArray.GetLength(1);    //パネルの横数
		panelNumY = panelArray.GetLength(0);    //パネルの縦数

		panelNum = (int)(panelNumX * panelNumY); //パネルの総数(穴抜き)
		panelOneSize = (panelScale * cameraSize) * 2; //パネルの移動(複製パネルの移動座標)

		//スクリーン座標
		yMin = cameraSize;
		yMax = yMin * -1f;
		xMin = yMin * ratio * -1f;
		xMax = xMin * -1f;

		//パネルセットの初期位置
		float _pSize = panelScale * cameraSize;
		defaultX = (_pSize * panelNumX - _pSize) * -1;
		defaultY = (_pSize * panelNumY - _pSize);

		panelVecter2XY = new float[panelNumY, panelNumX, 2];

		endFlg = true;

		ScoreSet();
		//-------------------------------------------------------------------------------------------------------------------------
		//START
		//-------------------------------------------------------------------------------------------------------------------------
		if (startFlg)
		{
			goArray = new string[panelNumY, panelNumX];
			//パネル配置
			PanelSetUP_Fnc();
			//-------------------------------------------------------------------------------------------------------------------------
			//ボール配置
			//-------------------------------------------------------------------------------------------------------------------------
			//ボールプレハブを指定位置に生成
			Vector3 ballPos = new Vector3(0, 0, 0);
			ballPos.x = defaultX + panelOneSize * nowPosition[1];
			ballPos.y = defaultY - panelOneSize * nowPosition[0];
			Bo = Instantiate(BallPrefab, ballPos, Quaternion.identity) as GameObject;
			Bo.name = "Ball";
			float ballScale = cameraSize * 2 * panelScale;
			Vector3 v3_ballScale = new Vector3(ballScale, ballScale, ballScale);
			Bo.transform.localScale = v3_ballScale;
			//-------------------------------------------------------------------------------------------------------------------------
			//UIの操作
			//-------------------------------------------------------------------------------------------------------------------------
			GameObject DeleteCountObjectUIText = GameObject.Find("UICanvas").transform.Find("DeleteCount").gameObject.transform.Find("DeleteCountText").gameObject; ;
			pd_UIText = DeleteCountObjectUIText.GetComponent<TextMeshProUGUI>();
			maxDeleteCount = breakCount;
			nowDeleteCount = maxDeleteCount;
			pd_UIText.text = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
		}
		else
		{
			//-------------------------------------------------------------------------------------------------------------------------
			//RESET
			//-------------------------------------------------------------------------------------------------------------------------
			Debug.Log("-----RESET-----");
			nowDeleteCount = maxDeleteCount;
			pd_UIText.text = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
			MouseDrag.mouseLRUD = "STOP";
			//ボールを止める
			BoStop_Fnc();
			//パネルを全部削除
			for (int i = 0; i < panelArray.GetLength(0); i++)
			{
				for (int j = 0; j < panelArray.GetLength(1); j++)
				{
					if (goArray[i, j] != null)
					{
						Delete_Fnc(goArray[i, j]);
						goArray[i, j] = null;

					}
				}
			}
			//変数初期化
			(_startPos, _stageArray, _hintArray, _stageScale, _breakCount) = stl.StageSetUP(stgNo);

			//パネル初期化関数
			nowPosition = _startPos;
			panelArray = _stageArray;
			PanelSetUP_Fnc();

			//ボール配置
			Vector3 ballPos = new Vector3(0, 0, 0);
			ballPos.x = defaultX + panelOneSize * nowPosition[1];
			ballPos.y = defaultY - panelOneSize * nowPosition[0];
			Bo.transform.position = ballPos;

			GameObject[] ptcObjects;
			ptcObjects = GameObject.FindGameObjectsWithTag("ParticleObject");
			Debug.Log(ptcObjects);
			foreach (GameObject t in ptcObjects)
			{
				Destroy(t);
			}

			AnimationStop();

		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//スコア計算[01]
	//-------------------------------------------------------------------------------------------------------------------------
	private int bestScore, nowScore, nowScoreView, totalScore;
	private int onePanelPoint, clearPoint;
	GameObject bestScoreOBJ, nowScoreOBJ, totalScoreOBJ;
	TextMeshProUGUI nowScoreGUI, bestScoreGUI, totalScoreGUI;
	void ScoreSet()
	{
		nowScore = 0;
		nowScoreView = 0;
		onePanelPoint = 1;
		clearPoint = 0;

		bestScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_BEST").gameObject;
		nowScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_NOW").gameObject;
		totalScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_TOTAL").gameObject;

		bestScoreGUI = bestScoreOBJ.GetComponent<TextMeshProUGUI>();
		nowScoreGUI = nowScoreOBJ.GetComponent<TextMeshProUGUI>();
		totalScoreGUI = totalScoreOBJ.GetComponent<TextMeshProUGUI>();

		totalScore = csvi.loadScoreArray[0];
		bestScore = csvi.loadScoreArray[Serialize_StageNo];
		bestScoreGUI.text = bestScore.ToString();
		nowScoreGUI.text = nowScore.ToString();
		totalScoreGUI.text = totalScore.ToString();

	}
	//-------------------------------------------------------------------------------------------------------------------------
	//スコアアップ
	//-------------------------------------------------------------------------------------------------------------------------
	void ScoreUP_Fnc()
	{
		if (endFlg)
		{
			int oneFrameScore = 1;
			if (nowScoreView < nowScore)
			{
				nowScoreView += oneFrameScore;
			}
			else
			{
				nowScoreView = nowScore;
			}

			nowScoreGUI = nowScoreOBJ.GetComponent<TextMeshProUGUI>();
			nowScoreGUI.text = nowScoreView.ToString();
		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//クリックして削除
	//-------------------------------------------------------------------------------------------------------------------------
	private GameObject[] TargetObjects;
	private GameObject ConfirmObject;
	private TextMeshProUGUI pd_UIText;
	private int maxDeleteCount, nowDeleteCount;

	void BreakClick_Fnc()
	{
		Vector2 mousePos = Input.mousePosition;
		Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(mousePos);

		if (nowDeleteCount != 0)
		{
			float minDistance = panelOneSize * 0.7f;

			TargetObjects = GameObject.FindGameObjectsWithTag("PanelObject");
			ConfirmObject = null;
			foreach (GameObject target in TargetObjects)
			{
				float _distance = Vector2.Distance(mousePos2D, target.transform.position);
				//Debug.Log("クリックした位置との距離:" + _distance);
				//minDistanceとの距離比較
				if (minDistance > _distance)
				{
					//繰り返して近いパネルを検索
					minDistance = _distance;
					//一番近いパネルをオブジェクトに格納
					ConfirmObject = target;
				}
			}

			if (ConfirmObject != null)
			{
				int notDelX = int.Parse(substRC_Num(ConfirmObject.name, "C"));
				int notDelY = int.Parse(substRC_Num(ConfirmObject.name, "R"));

				if (notDelX == nowPosition[0] && notDelY == nowPosition[1])
				{
					//ボールがあるパネルは削除できません
				}
				else
				{
					Debug.Log("Delete? " + notDelX + ":" + notDelY);
					Delete_Fnc(ConfirmObject.name);
					nowDeleteCount--;
					pd_UIText.text = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
				}

			}
		}
	}


	//-------------------------------------------------------------------------------------------------------------------------
	//パネル配置
	//-------------------------------------------------------------------------------------------------------------------------
	private GameObject Po;
	private void PanelSetUP_Fnc()
	{
		for (int i = 0; i < panelArray.GetLength(0); i++)
		{
			for (int j = 0; j < panelArray.GetLength(1); j++)
			{
				//インスタンスの生成位置
				panel_xPos = defaultX + (j * panelOneSize);
				panel_yPos = defaultY + (i * panelOneSize) * -1;

				if (panelArray[i, j] >= 1)
				{
					// プレハブを指定位置に生成
					Vector2 instancePos = new Vector2(panel_xPos, panel_yPos);
					Po = Instantiate(PanelPrefab, instancePos, Quaternion.identity) as GameObject;
					Po.name = "Po_" + "R" + i + "C" + j;
					Vector2 goScale = new Vector2(1, 1);
					goScale.x = panelScaleMin;
					goScale.y = panelScaleMin;
					Po.transform.localScale = goScale;

					//配列にインスタンス名を追加
					goArray[i, j] = Po.name;
				}
				panelVecter2XY[i, j, 0] = panel_xPos;
				panelVecter2XY[i, j, 1] = panel_yPos;
			}
		}
		//dal.Array2DLog(goArray);
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//RCを数字で取り出す関数
	//-------------------------------------------------------------------------------------------------------------------------
	private string substRC_Num(string textBase, string RC)
	{
		string textC = "";
		string textR = "";
		for (int i = 0; i < textBase.Length; i++)
		{
			if (textBase.Substring(i, 1) == "R")
			{
				for (int j = i; j < textBase.Length; j++)
				{
					if (textBase.Substring(j, 1) == "C")
					{
						break;
					}
					else if (
						Regex.IsMatch(textBase.Substring(j, 1), @"^[0-9]+$")
					)
					{
						textC += textBase.Substring(j, 1);
					}
				}
			}
			if (textBase.Substring(i, 1) == "C")
			{
				for (int j = i; j < textBase.Length; j++)
					if (Regex.IsMatch(textBase.Substring(j, 1), @"^[0-9]+$"))
					{
						textR += textBase.Substring(j, 1);
					}
			}
		}

		string returnText = "null";
		if (RC == "R")
		{
			returnText = textR;
		}
		else if (RC == "C")
		{
			returnText = textC;
		}
		return returnText;
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//ボールオブジェクトの移動関数
	//-------------------------------------------------------------------------------------------------------------------------
	private float ballMove = 0.15f; //ボールの移動距離
	private float ballRotation = 6.0f;  //ボールの回転速度
	private void BoMove_Fnc()
	{
		string md_mouseLRUD = MouseDrag.mouseLRUD;
		Vector3 _pos = Bo.transform.position;
		Transform myTransform = Bo.transform;

		if (goArray != null)
		{
			float limitMoveX_Half_Right = panelVecter2XY[nowPosition[0], nowPosition[1], 0] + (panelOneSize / 10);
			float limitMoveX_Half_Left = panelVecter2XY[nowPosition[0], nowPosition[1], 0] - (panelOneSize / 10);
			float limitMoveY_Half_Up = panelVecter2XY[nowPosition[0], nowPosition[1], 1] + (panelOneSize / 10);
			float limitMoveY_Half_Down = panelVecter2XY[nowPosition[0], nowPosition[1], 1] - (panelOneSize / 10);
			string _daleteName = "Po_R" + nowPosition[0] + "C" + nowPosition[1];
			GameObject BraekObject = GameObject.Find(_daleteName);
			SpriteRenderer _sr = BraekObject.GetComponent<SpriteRenderer>();
			Color32 col32 = new Color32(150, 150, 150, 255);

			switch (md_mouseLRUD)
			{
				case "R":
					if (_pos.x >= limitMoveX_Half_Right)
					{
						if (panelArray[(nowPosition[0]), nowPosition[1] + 1] >= 1)
						{
							nowScore += onePanelPoint;
							Delete_Fnc(_daleteName);
							nowPosition[1]++;
						}
						else
						{
							_pos.x = panelVecter2XY[nowPosition[0], nowPosition[1], 0];
							BoStop_Fnc();
						}
					}
					else
					{
						_pos.x = _pos.x + ballMove;
						myTransform.Rotate(0.0f, (ballRotation * -1), 0.0f, Space.World);
					}
					break;
				case "L":
					if (_pos.x <= limitMoveX_Half_Left)
					{
						if (panelArray[(nowPosition[0]), nowPosition[1] - 1] >= 1)
						{
							nowScore += onePanelPoint;
							Delete_Fnc(_daleteName);
							nowPosition[1]--;
						}
						else
						{
							_pos.x = panelVecter2XY[nowPosition[0], nowPosition[1], 0];
							BoStop_Fnc();
						}
					}
					else
					{
						_pos.x = _pos.x - ballMove;
						myTransform.Rotate(0.0f, ballRotation, 0.0f, Space.World);
					}
					break;
				case "U":
					if (_pos.y >= limitMoveY_Half_Up)
					{
						if (panelArray[(nowPosition[0]) - 1, nowPosition[1]] >= 1)
						{
							nowScore += onePanelPoint;
							Delete_Fnc(_daleteName);
							nowPosition[0]--;
						}
						else
						{
							_pos.y = panelVecter2XY[nowPosition[0], nowPosition[1], 1];
							BoStop_Fnc();
						}
					}
					else
					{
						_pos.y = _pos.y + ballMove;
						myTransform.Rotate(ballRotation, 0.0f, 0.0f, Space.World);
					}
					break;
				case "D":
					if (_pos.y <= limitMoveY_Half_Down)
					{
						if (panelArray[(nowPosition[0]) + 1, nowPosition[1]] >= 1)
						{
							nowScore += onePanelPoint;
							Delete_Fnc(_daleteName);
							nowPosition[0]++;
						}
						else
						{
							_pos.y = panelVecter2XY[nowPosition[0], nowPosition[1], 1];
							BoStop_Fnc();
						}
					}
					else
					{
						_pos.y = _pos.y - ballMove;
						myTransform.Rotate((ballRotation * -1), 0.0f, 0.0f, Space.World);
					}
					break;
				default:
					break;
			}
			if (Bo)
			{
				Bo.transform.position = _pos;
			}
		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//ボールオブジェクトの停止関数
	//-------------------------------------------------------------------------------------------------------------------------
	private GameObject BackGroundObject;
	private MouseDrag MouseDragScript;
	private void BoStop_Fnc()
	{
		BackGroundObject = GameObject.Find("BackGroundCollider");
		MouseDragScript = BackGroundObject.GetComponent<MouseDrag>();
		MouseDragScript.OnMouseDown();
		MouseDrag.mouseLRUD = "STOP";
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//オブジェクトの削除関数
	//-------------------------------------------------------------------------------------------------------------------------
	private void Delete_Fnc(string deleteObjectName)
	{
		GameObject DeleteObject = GameObject.Find(deleteObjectName);
		//Debug.Log("削除:" + DeleteObject);
		Destroy(DeleteObject);
		int getC = int.Parse(substRC_Num(DeleteObject.name, "C"));
		int getR = int.Parse(substRC_Num(DeleteObject.name, "R"));
		panelArray[getC, getR] = 0;
		goArray[getC, getR] = null;
		Particle_Fnc(DeleteObject);
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//爆発エフェクト
	//-------------------------------------------------------------------------------------------------------------------------
	private void Particle_Fnc(GameObject obj)
	{
		GameObject _effectGO = obj;
		float ptc_Xpos = _effectGO.transform.position.x;
		float ptc_Ypos = _effectGO.transform.position.y;
		Vector3 instancePos = new Vector3(ptc_Xpos, ptc_Ypos, -1);
		GameObject Pto = Instantiate(ParticlePrefab, instancePos, new Quaternion(90f, 0f, 0f, 1.0f)) as GameObject;
		Pto.name = "ParticleObject";
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//ヒント処理
	//-------------------------------------------------------------------------------------------------------------------------
	public void Hint_Fnc()
	{
		int _hintR = 0;
		int _hintC = 0;
		int _iCount = 0;
		foreach (int i in hintArray)
		{
			_iCount++;
			if (_iCount % 2 == 0)
			{
				_hintR = i;
				string _hintName = "Po_R" + _hintC + "C" + _hintR;
				GameObject HintObject = GameObject.Find(_hintName);
				if (!HintObject)
				{
					HintObject.GetComponent<SpriteRenderer>().color = new Color(200.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f);
				}
			}
			else
			{
				_hintC = i;
			}
		}
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//アニメーション停止
	//-------------------------------------------------------------------------------------------------------------------------

	public Animator anim;
	void AnimationStop()
	{
		anim.Play("ClearAnimation", 0, 0);
		anim.speed = 0;
	}

	//-------------------------------------------------------------------------------------------------------------------------
	//アニメーション再生
	//-------------------------------------------------------------------------------------------------------------------------
	void AnimationPlay()
	{
		anim.speed = 1;
	}
}


