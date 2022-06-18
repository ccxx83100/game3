using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class MainScript : MonoBehaviour
{
	public GameObject PanelPrefab, BallPrefab, ParticlePrefab;      //プレハブ設定用のGameObject
	private float panelOneSize, panelScale, xMin, xMax, yMin, yMax;
	private float defaultX, defaultY, panel_xPos, panel_yPos, panelScaleMin, cameraSize;
	private float[,,] panelVecter2XY;
	private string[,] goArray;
	private int[,] panelArray, hintArray;
	private int[] nowPosition;
	private int breakCount;
	[SerializeField]
	public int Serialize_StageNo;
	public int resetCost = 30;
	public int hintCost = 200;
	public int[] loadCsvArray;
	public bool startFlg, endFlg;  //スタート　エンドフラグ
	public bool escapeFlg = true;   //ステージセレクトバグ回避フラグ
	private bool hintFlg;
	DebugArrayLog dal = new DebugArrayLog();    //配列デバッグ用のインスタンス
	CSVImporter csvi = new CSVImporter();   //CSV読み書き込み用のインスタンス
	StageList stl = new StageList();    //ステージリストのインスタンス

	//--------------------------------------------------------------------------------
	//START
	//--------------------------------------------------------------------------------
	public void Start()
	{
		csvi.CsvLoad();     //csvをロード
		loadCsvArray = csvi.loadScoreArray;

		if (Serialize_StageNo >= 1)
		{
			escapeFlg = false;
			startFlg = true;
			StartReset_Fnc(Serialize_StageNo);
			startFlg = false;
			AnimationStop();
		}

		testTest(); //後で消す

	}


	//--------------------------------------------------------------------------------
	//UPDATE
	//--------------------------------------------------------------------------------
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
			if (Input.GetMouseButtonDown(0))
			{
				Vector3 mouseClickPosition = Input.mousePosition;
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

	///-------------------------------------------------------------------------------
	/// <summary>
	/// タグ検索してTextMeshProのテキストを変更
	/// </summary>
	///-------------------------------------------------------------------------------
	private void TagSarchAndChangeText<T>(string _tagName, T _param)
	{
		GameObject[] _go = GameObject.FindGameObjectsWithTag(_tagName);
		foreach (GameObject i in _go)
		{
			TextMeshProUGUI _tM = i.GetComponent<TextMeshProUGUI>();
			_tM.text = _param.ToString();
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// クリア後の処理
	/// </summary>
	///-------------------------------------------------------------------------------
	void Clear_func()
	{
		if (endFlg)
		{
			nowScore = nowScore + onePanelPoint;        //最後のパネルは消さないので1個分のスコア加算
			nowScoreView = nowScore;
			/*
			nowScoreGUI = nowScoreOBJ.GetComponent<TextMeshProUGUI>();
			nowScoreGUI.text = nowScoreView.ToString();
			*/

			TagSarchAndChangeText("ScoreNow", nowScoreView);
			endFlg = false;

			//ベストスコア更新時のみ
			if (nowScore > bestScore)
			{
				loadCsvArray = csvi.loadScoreArray;
				loadCsvArray[Serialize_StageNo] = nowScore;


				/*
								bestScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_BEST").gameObject;
								bestScoreGUI = bestScoreOBJ.GetComponent<TextMeshProUGUI>();
								bestScoreGUI.text = nowScore.ToString();
								*/

				TagSarchAndChangeText("ScoreBest", nowScore);



				int _addScore = nowScore - bestScore;
				totalScore += _addScore;
				loadCsvArray[0] = totalScore;
				/*	
							totalScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_TOTAL").gameObject;
							totalScoreGUI = totalScoreOBJ.GetComponent<TextMeshProUGUI>();
							totalScoreGUI.text = totalScore.ToString();
							*/

				TagSarchAndChangeText("ScoreTotal", totalScore);

				/*
								costScoreOBJ = GameObject.Find("UICanvas").gameObject.transform.Find("Button_window").gameObject.transform.Find("Score_COST").gameObject;
								costScoreGUI = totalScoreOBJ.GetComponent<TextMeshProUGUI>();
								costScoreGUI.text = totalScore.ToString();
								*/

				TagSarchAndChangeText("ScoreCost", totalScore);
				dal.Array1DLog(loadCsvArray);
				csvi.CsvSave(loadCsvArray);
			}
		}
	}


	private GameObject Bo;
	private int panelNum, panelNumX, panelNumY;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタートとリセット処理・共通部と各処理
	/// </summary>
	///-------------------------------------------------------------------------------
	public void StartReset_Fnc(int stgNo)
	{
		float screenX = Screen.width;
		float screenY = Screen.height;
		Camera MainCamera = Camera.main;
		float ratio = screenX / screenY;
		cameraSize = MainCamera.orthographicSize;

		//ステージリストから値を取得		
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

		panelNum = (int)(panelNumX * panelNumY);    //パネルの総数(穴抜き)
		panelOneSize = (panelScale * cameraSize) * 2;   //パネルの移動(複製パネルの移動座標)

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
		hintFlg = true;

		ScoreSet();
		//各コスト表示
		TagSarchAndChangeText("ResetButton", resetCost);
		TagSarchAndChangeText("HintButton", hintCost);

		if (startFlg)
		{
			//--------------------------------------------------------------------------------------------------------
			//START
			//--------------------------------------------------------------------------------------------------------
			goArray = new string[panelNumY, panelNumX];
			//パネル配置
			PanelSetUP_Fnc();
			//--------------------------------------------------------------------------------------------------------
			//ボール配置
			//--------------------------------------------------------------------------------------------------------
			//ボールプレハブを指定位置に生成
			Vector3 ballPos = new Vector3(0, 0, 0);
			ballPos.x = defaultX + panelOneSize * nowPosition[1];
			ballPos.y = defaultY - panelOneSize * nowPosition[0];
			Bo = Instantiate(BallPrefab, ballPos, Quaternion.identity) as GameObject;
			Bo.name = "Ball";
			float ballScale = cameraSize * 2 * panelScale;
			Vector3 v3_ballScale = new Vector3(ballScale, ballScale, ballScale);
			Bo.transform.localScale = v3_ballScale;
			//--------------------------------------------------------------------------------------------------------
			//UIの操作
			//--------------------------------------------------------------------------------------------------------
			//GameObject DeleteCountObjectUIText = GameObject.Find("UICanvas").transform.Find("DeleteCount").gameObject.transform.Find("DeleteCountText").gameObject; ;
			//pd_UIText = DeleteCountObjectUIText.GetComponent<TextMeshProUGUI>();
			maxDeleteCount = breakCount;
			nowDeleteCount = maxDeleteCount;
			//pd_UIText.text = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;

			string _deateCountText = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
			TagSarchAndChangeText("DeleteCount", _deateCountText);

		}
		else
		{
			//--------------------------------------------------------------------------------------------------------
			//RESET
			//--------------------------------------------------------------------------------------------------------
			Debug.Log("-----RESET-----");
			nowDeleteCount = maxDeleteCount;
			//pd_UIText.text = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
			string _deateCountText = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
			TagSarchAndChangeText("DeleteCount", _deateCountText);
			MouseDrag.mouseLRUD = "STOP";

			BoStop_Fnc();   //ボールを止める
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
			foreach (GameObject i in ptcObjects)
			{
				Destroy(i);
			}
			AnimationStop();
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 修正します 修正します 修正します 修正します 修正します
	/// </summary>
	///-------------------------------------------------------------------------------
	void StartOnly()
	{
		goArray = new string[panelNumY, panelNumX];
		//パネル配置
		PanelSetUP_Fnc();
		//ボール配置
		Vector3 ballPos = new Vector3(0, 0, 0);
		ballPos.x = defaultX + panelOneSize * nowPosition[1];
		ballPos.y = defaultY - panelOneSize * nowPosition[0];
		Bo = Instantiate(BallPrefab, ballPos, Quaternion.identity) as GameObject;
		Bo.name = "Ball";
		float ballScale = cameraSize * 2 * panelScale;
		Vector3 v3_ballScale = new Vector3(ballScale, ballScale, ballScale);
		Bo.transform.localScale = v3_ballScale;
		//UIの操作
		maxDeleteCount = breakCount;
		nowDeleteCount = maxDeleteCount;

		string _deateCountText = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
		TagSarchAndChangeText("DeleteCount", _deateCountText);

	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 修正します 修正します 修正します 修正します 修正します
	/// </summary>
	///-------------------------------------------------------------------------------
	void ResetOnly()
	{
		Debug.Log("-----RESET-----");
		nowDeleteCount = maxDeleteCount;
		string _deateCountText = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
		TagSarchAndChangeText("DeleteCount", _deateCountText);
		MouseDrag.mouseLRUD = "STOP";

		BoStop_Fnc();   //ボールを止める

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
		var (_startPos, _stageArray, _hintArray, _stageScale, _breakCount) = stl.StageSetUP(Serialize_StageNo);

		//パネル初期化
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
		foreach (GameObject i in ptcObjects)
		{
			Destroy(i);
		}
		AnimationStop();
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// 総コストを減らす (引数:コストの額)
	/// </summary>
	///-------------------------------------------------------------------------------
	public void CostDown(int _cost)
	{
		if (totalScore >= _cost)
		{
			totalScore -= _cost;

			loadCsvArray = csvi.loadScoreArray;
			dal.Array1DLog(loadCsvArray);

			loadCsvArray[0] = totalScore;

			/*
			totalScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_TOTAL").gameObject;
						totalScoreGUI = totalScoreOBJ.GetComponent<TextMeshProUGUI>();
						totalScoreGUI.text = totalScore.ToString();
						costScoreGUI = costScoreOBJ.GetComponent<TextMeshProUGUI>();
						costScoreGUI.text = totalScore.ToString();
						*/

			TagSarchAndChangeText("ScoreTotal", totalScore);
			TagSarchAndChangeText("ScoreCost", totalScore);

			csvi.CsvSave(loadCsvArray);
		}
	}


	private int bestScore, nowScore, nowScoreView, totalScore;
	private int onePanelPoint;
	//public GameObject bestScoreOBJ, nowScoreOBJ, totalScoreOBJ, costScoreOBJ;
	//public TextMeshProUGUI nowScoreGUI, bestScoreGUI, totalScoreGUI, costScoreGUI;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スコア計算と表示
	/// </summary>
	///-------------------------------------------------------------------------------
	void ScoreSet()
	{
		nowScore = 0;
		nowScoreView = 0;
		onePanelPoint = 10;

		/*
				bestScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_BEST").gameObject;
				nowScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_NOW").gameObject;
				totalScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_TOTAL").gameObject;
				costScoreOBJ = GameObject.Find("UICanvas").gameObject.transform.Find("Button_window").gameObject.transform.Find("Score_COST").gameObject;

				bestScoreGUI = bestScoreOBJ.GetComponent<TextMeshProUGUI>();
				nowScoreGUI = nowScoreOBJ.GetComponent<TextMeshProUGUI>();
				totalScoreGUI = totalScoreOBJ.GetComponent<TextMeshProUGUI>();
				costScoreGUI = costScoreOBJ.GetComponent<TextMeshProUGUI>();
				*/

		totalScore = csvi.loadScoreArray[0];
		bestScore = csvi.loadScoreArray[Serialize_StageNo];

		/*	bestScoreGUI.text = bestScore.ToString();
			nowScoreGUI.text = nowScore.ToString();
			totalScoreGUI.text = totalScore.ToString();
			costScoreGUI.text = totalScore.ToString();
			*/

		TagSarchAndChangeText("ScoreBest", bestScore);
		TagSarchAndChangeText("ScoreNow", nowScore);
		TagSarchAndChangeText("ScoreTotal", totalScore);
		TagSarchAndChangeText("ScoreCost", totalScore);

	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// スコアアップ処理 (1フレーム毎にスコアを加算する)
	/// </summary>
	///-------------------------------------------------------------------------------
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

			/*
						nowScoreOBJ = GameObject.Find("UICanvas").transform.Find("ScoreUI").gameObject.transform.Find("Score_NOW").gameObject;
						nowScoreGUI = nowScoreOBJ.GetComponent<TextMeshProUGUI>();
						nowScoreGUI.text = nowScoreView.ToString();
						*/

			TagSarchAndChangeText("ScoreNow", nowScoreView);
		}
	}


	//private TextMeshProUGUI pd_UIText;
	private int maxDeleteCount, nowDeleteCount;

	/// <summary>
	/// クリックして削除 (クリック位置から近いオブジェクトを探して削除)
	/// </summary>
	void BreakClick_Fnc()
	{
		Vector2 mousePos = Input.mousePosition;
		Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(mousePos);

		if (nowDeleteCount != 0)
		{
			float minDistance = panelOneSize * 0.7f; //クリックした位置とパネルの距離
			GameObject[] TargetObjects = GameObject.FindGameObjectsWithTag("PanelObject");
			GameObject ConfirmObject = null;
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

					string _deateCountText = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
					TagSarchAndChangeText("DeleteCount", _deateCountText);
					//pd_UIText.text = "Break!!  " + nowDeleteCount + " / " + maxDeleteCount;
				}
			}
		}
	}


	/// <summary>
	/// パネルの配置/インスタンスを生成し座標を配列に入れる
	/// </summary>
	private void PanelSetUP_Fnc()
	{
		GameObject Po;
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
	}


	/// <summary>
	/// テキストからRCの数字を取得 (引数1:テキスト 引数2:RかCの1文字)
	/// </summary>
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

	/// <summary>
	/// [重要]ボールオブジェクトの移動・停止・回転・移動した場所の削除
	/// </summary>
	private void BoMove_Fnc()
	{
		float ballMove = 0.15f; //ボールの移動距離
		float ballRotation = 6.0f;  //ボールの回転速度
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
			/*
			GameObject BraekObject = GameObject.Find(_daleteName);
			SpriteRenderer _sr = BraekObject.GetComponent<SpriteRenderer>();
			Color32 col32 = new Color32(150, 150, 150, 255);
			*/

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
			if (Bo) Bo.transform.position = _pos;

		}
	}


	/// <summary>
	/// ボールの停止
	/// </summary>
	private void BoStop_Fnc()
	{
		GameObject BackGroundObject = GameObject.Find("BackGroundCollider");   //インスタンスにして処理しないとバグる
		MouseDrag MouseDragScript = BackGroundObject.GetComponent<MouseDrag>();
		MouseDragScript.OnMouseDown();
		MouseDrag.mouseLRUD = "STOP";
	}


	/// <summary>
	/// オブジェクトを削除する (引数1:オブジェクトではなく、オブジェクト名)
	/// </summary>
	private void Delete_Fnc(string deleteObjectName)
	{
		GameObject DeleteObject = GameObject.Find(deleteObjectName);
		Destroy(DeleteObject);
		int getC = int.Parse(substRC_Num(DeleteObject.name, "C"));
		int getR = int.Parse(substRC_Num(DeleteObject.name, "R"));
		panelArray[getC, getR] = 0;
		goArray[getC, getR] = null;
		Particle_Fnc(DeleteObject);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 爆発エフェクト (引数1:爆発させたいインスタンスがある場所を指定)
	/// </summary>
	///-------------------------------------------------------------------------------
	private void Particle_Fnc(GameObject obj)
	{
		GameObject _effectGO = obj;
		float ptc_Xpos = _effectGO.transform.position.x;
		float ptc_Ypos = _effectGO.transform.position.y;
		Vector3 instancePos = new Vector3(ptc_Xpos, ptc_Ypos, -1);
		GameObject Pto = Instantiate(ParticlePrefab, instancePos, new Quaternion(90f, 0f, 0f, 1.0f)) as GameObject;
		Pto.name = "ParticleObject";
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ヒント表示
	/// </summary>
	///-------------------------------------------------------------------------------
	public void Hint_Fnc()
	{
		int _hintR = 0;
		int _hintC = 0;
		int _iCount = 0;
		if (hintFlg)
		{
			hintFlg = false;
			foreach (int i in hintArray)
			{
				_iCount++;
				if (_iCount % 2 == 0)
				{
					_hintC = i;
					string _hintName = "Po_R" + _hintR + "C" + _hintC;
					GameObject HintObject = GameObject.Find(_hintName);
					if (HintObject != null)
					{
						HintObject.GetComponent<SpriteRenderer>().color = new Color(200.0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 255.0f / 255.0f);
						CostDown(hintCost);
					}
					else
					{
						hintFlg = false;
					}
				}
				else
				{
					_hintR = i;
				}
			}
		}
	}

	public Animator anim;
	/// <summary>
	/// アニメ停止 (速度調整で停止して、0フレーム目に移動)
	/// </summary>

	void AnimationStop()
	{
		anim.Play("ClearAnimation", 0, 0);
		anim.speed = 0;
	}

	/// <summary>
	/// アニメ再生 (速度調整で再生)
	/// </summary>
	void AnimationPlay()
	{
		anim.speed = 1;
	}

	//---------------------------------------------------------------------------------------------------------------------

	private void testTest()
	{
		ScoreClass SC = new ScoreClass();
		SC.a();
	}

}





//---------------------------------------------------------------------------------
//
//---------------------------------------------------------------------------------


/// <summary>
/// まだ作っている途中
/// </summary>
public class ScoreClass
{
	/// <summary>
	/// まだ作っている途中
	/// </summary>
	public void a()
	{

		GameObject _gm = GameObject.Find("GameMain");   //インスタンスにして処理しないとバグる
		MainScript _ms = _gm.GetComponent<MainScript>();
		bool a = _ms.endFlg;



	}

}

public class Indexer
{
	public string this[string str]
	{
		get
		{
			/// 本来は何か処理をして返す
			/// 今回はそのまま引数の文字列を返す
			return str;
		}
	}
}


