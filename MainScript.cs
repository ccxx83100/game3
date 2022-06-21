using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class MainScript : MonoBehaviour
{
	public GameObject PanelPrefab, BallPrefab, ParticlePrefab;      //プレハブ設定用のGameObject
	private float panelOneSize, panelScale;
	private float defaultX, defaultY, panel_xPos, panel_yPos, panelScaleMin, cameraSize, panelSize;
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
	public bool startFlg, endFlg;               //スタート　エンドフラグ
	public bool escapeFlg = true;               //ステージセレクトバグ回避フラグ
	private bool hintFlg;
	DebugArrayLog dal = new DebugArrayLog();    //配列デバッグ用のインスタンス
	CSVImporter csvi = new CSVImporter();       //CSV読み書き込み用のインスタンス
	StageList stl = new StageList();            //ステージリストのインスタンス

	///-------------------------------------------------------------------------------
	/// <summary>
	/// START
	/// </summary>
	///-------------------------------------------------------------------------------
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

	///-------------------------------------------------------------------------------
	/// <summary>
	/// UPDATE
	/// </summary>
	///-------------------------------------------------------------------------------
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
				//Vector3 mouseClickPosition = Input.mousePosition;
				BreakClick_Fnc();
				BreakClick_Fnc2();
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
	private GameObject Bo;
	private int panelNum, panelNumX, panelNumY;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタートとリセット処理・共通部と各処理
	/// </summary>
	///-------------------------------------------------------------------------------
	public void StartReset_Fnc(int stgNo)
	{

		GameObject NextButton = GameObject.Find("NextButton");
		NextButton.transform.position = new Vector3(0f, 7.0f, 0f);

		custumLib.ScreenData();

		Camera MainCamera = Camera.main;
		cameraSize = MainCamera.orthographicSize;

		//ステージリストから値を取得		
		var (_startPos, _stageArray, _hintArray, _stageScale, _breakCount) = stl.StageSetUP(stgNo);

		//現在地配列 [] [,]
		nowPosition = _startPos;
		panelArray = _stageArray;
		hintArray = _hintArray;
		breakCount = _breakCount;

		panelScale = _stageScale;
		panelScaleMin = panelScale * 1.0f;              //マージン分を縮小する

		panelNumX = panelArray.GetLength(1);            //パネルの横数
		panelNumY = panelArray.GetLength(0);            //パネルの縦数

		panelNum = (int)(panelNumX * panelNumY);        //パネルの総数(穴抜き)
		panelOneSize = (panelScale * cameraSize) * 2;   //パネルの移動(複製パネルの移動座標)

		//パネルセットの初期位置
		panelSize = panelScale * cameraSize;
		Debug.Log($"_panelSize:{panelSize}");

		defaultX = (panelSize * panelNumX - panelSize) * -1;
		defaultY = (panelSize * panelNumY - panelSize);

		panelVecter2XY = new float[panelNumY, panelNumX, 2];
		endFlg = true;
		hintFlg = true;

		ScoreSet();
		//各コスト表示
		TagSarchAndChangeText("ResetButton", resetCost);
		TagSarchAndChangeText("HintButton", hintCost);

		maxDeleteCount = breakCount;
		nowDeleteCount = maxDeleteCount;

		//スプライトナンバー処理
		GameObject BreakCountObNow = GameObject.Find("BreakCountNow");
		ImageNo imgNo_Now = BreakCountObNow.GetComponent<ImageNo>();
		GameObject BreakCountObMax = GameObject.Find("BreakCountMax");
		ImageNo imgNo_Max = BreakCountObMax.GetComponent<ImageNo>();
		imgNo_Now.SpriteNumSet(nowDeleteCount);
		imgNo_Max.SpriteNumSet(maxDeleteCount);

		if (startFlg)
		{
			StartOnly();
		}
		else
		{
			ResetOnly();
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタート時のみ実行
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
		float _ranX = Random.Range(0.0f, 360.0f);
		float _ranY = Random.Range(0.0f, 360.0f);
		float _ranZ = Random.Range(0.0f, 360.0f);
		Quaternion rotQ = Quaternion.Euler(_ranX, _ranY, _ranZ);
		Bo.transform.rotation = rotQ;
		Bo.name = "Ball";
		float ballScale = cameraSize * 2 * panelScale;
		Vector3 v3_ballScale = new Vector3(ballScale, ballScale, ballScale);
		Bo.transform.localScale = v3_ballScale;
		//UIの操作
		maxDeleteCount = breakCount;
		nowDeleteCount = maxDeleteCount;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// リセットボタン押した時のみ実行
	/// </summary>
	///-------------------------------------------------------------------------------
	void ResetOnly()
	{
		Debug.Log("-----RESET-----");
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

			TagSarchAndChangeText("ScoreNow", nowScoreView);
			endFlg = false;

			GameObject NextButton = GameObject.Find("NextButton");
			NextButton.transform.position = new Vector3(0.0f, -3.4f, 0f);

			//ベストスコア更新時のみ
			if (nowScore > bestScore)
			{
				loadCsvArray = csvi.loadScoreArray;
				loadCsvArray[Serialize_StageNo] = nowScore;
				TagSarchAndChangeText("ScoreBest", nowScore);

				int _addScore = nowScore - bestScore;
				totalScore += _addScore;
				loadCsvArray[0] = totalScore;

				TagSarchAndChangeText("ScoreCost", totalScore);

				dal.Array1DLog(loadCsvArray);
				csvi.CsvSave(loadCsvArray);
			}
		}
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

			TagSarchAndChangeText("ScoreCost", totalScore);

			csvi.CsvSave(loadCsvArray);
		}
	}

	private int bestScore, nowScore, nowScoreView, totalScore;
	private int onePanelPoint;
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

		totalScore = csvi.loadScoreArray[0];
		bestScore = csvi.loadScoreArray[Serialize_StageNo];

		TagSarchAndChangeText("ScoreBest", bestScore);
		TagSarchAndChangeText("ScoreNow", nowScore);
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
			int oneFrameScore = 2;
			if (nowScoreView < nowScore)
			{
				nowScoreView += oneFrameScore;
			}
			else
			{
				nowScoreView = nowScore;
			}
			TagSarchAndChangeText("ScoreNow", nowScoreView);
		}
	}

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
				int notDelX = int.Parse(custumLib.substRC_Num(ConfirmObject.name, "C"));
				int notDelY = int.Parse(custumLib.substRC_Num(ConfirmObject.name, "R"));

				if (notDelX == nowPosition[0] && notDelY == nowPosition[1])
				{
					//ボールがあるパネルは削除できません
				}
				else
				{
					Debug.Log("Delete? " + notDelX + ":" + notDelY);
					Delete_Fnc(ConfirmObject.name);
					nowDeleteCount--;

					GameObject BreakCountObNow = GameObject.Find("BreakCountNow");
					ImageNo imgNo_Now = BreakCountObNow.GetComponent<ImageNo>();
					imgNo_Now.SpriteNumSet(nowDeleteCount);
				}
			}
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	///
	/// </summary>
	///-------------------------------------------------------------------------------
	void BreakClick_Fnc2()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Debug.Log($"x:{mousePos.x}---y:{mousePos.y}");
		Debug.Log("--------------");
		bool destroyFlg = false;
		int _row = 0;
		int _col = 0;
		for (int i = 0; i < panelVecter2XY.GetLength(0); i++)
		{
			for (int j = 0; j < panelVecter2XY.GetLength(1); j++)
			{
				if (mousePos.x >= panelVecter2XY[i, j, 0] - panelSize && mousePos.x < panelVecter2XY[i, j, 0] + panelSize)
				{
					if (mousePos.y <= panelVecter2XY[i, j, 1] + panelSize && mousePos.y > panelVecter2XY[i, j, 1] - panelSize)
					{

						_row = i;
						_col = j;
						destroyFlg = true;
					}
				}
			}
		}
		if (destroyFlg)
		{
			string destroyName = "Po_R" + _row + "C" + _col;
			Debug.Log(destroyName);
		}


		//Debug.Log($"{row}:{col}");
		//dal.Array3DLog(panelVecter2XY);
		//dal.Array3DLog(panelVecter2XY_ok);



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

	[SerializeField]
	public float ballMove = 0.15f;      //ボールの移動速度
	[SerializeField]
	public float ballRotation = 11.0f;  //ボールの回転速度

	///-------------------------------------------------------------------------------
	/// <summary>
	/// [重要]ボールオブジェクトの移動・停止・回転・移動した場所の削除
	/// </summary>
	///-------------------------------------------------------------------------------
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

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ボールの停止
	/// </summary>
	///-------------------------------------------------------------------------------
	private void BoStop_Fnc()
	{
		GameObject BackGroundObject = GameObject.Find("BackGroundCollider");   //インスタンスにして処理しないとバグる
		MouseDrag MouseDragScript = BackGroundObject.GetComponent<MouseDrag>();
		MouseDragScript.OnMouseDown();
		MouseDrag.mouseLRUD = "STOP";
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// オブジェクトを削除する (引数1:オブジェクトではなく、オブジェクト名)
	/// 削除してパーティクルを実行    
	/// </summary>
	///-------------------------------------------------------------------------------
	private void Delete_Fnc(string deleteObjectName)
	{
		GameObject DeleteObject = GameObject.Find(deleteObjectName);
		Destroy(DeleteObject);

		//配列から削除
		int getC = int.Parse(custumLib.substRC_Num(DeleteObject.name, "C"));
		int getR = int.Parse(custumLib.substRC_Num(DeleteObject.name, "R"));
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

	///-------------------------------------------------------------------------------
	public Animator anim;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// アニメ停止 (速度調整で停止して、0フレーム目に移動)
	/// </summary>
	///-------------------------------------------------------------------------------
	void AnimationStop()
	{
		anim.Play("ClearAnimation", 0, 0);
		anim.speed = 0;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// アニメ再生 (速度調整で再生)
	/// </summary>
	///-------------------------------------------------------------------------------
	void AnimationPlay()
	{
		anim.speed = 1;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 後で削除
	/// </summary>
	///-------------------------------------------------------------------------------
	private void testTest()
	{
		ScoreClass SC = new ScoreClass();
		SC.a();

		IndexerTest IT = new IndexerTest();
		int a = IT[0];
		//Debug.Log(IT.GetSetProperty);

		IndexerTest_old ITO = new IndexerTest_old();
		ITO.SetProperty(100);
		Debug.Log(ITO.GetProperty());

	}
}



//---------------------------------------------------------------------------------
//C#におけるインデクサーは、あたかも配列であるかのように呼び出せる機能で、
//あらかじめユーザーが定義したコードを実行させることができます。
//クラス名などの名前を定義する必要がないため、コーディングに慣れていれば一目でインデクサーだと分かるでしょう。
//インデクサーの中で定義するgetは要素型のデータを返す必要があり、setでは要素型の引数を受け取ることになります。
//
//インデクサーは配列と似ていますが、中身は別物になっています。
//最大の特徴はアクセスする際の添え字がint型である必要がないことで、
//対応したデータを入れるようにすれば文字列でも添え字にすることが可能です。
/// 
//		// [ ] を使って要素を取得しようとするとgetが呼ばれる
//		Debug.Log(cls[2]);
//
//		// [ ] を使って要素を設定しようとするとsetが呼ばれる
//		cls[2] = "zzzz";
//		Debug.Log(cls[2]);
///
/// //---------------------------------------------------------------------------------

public class IndexerTest
{
	private int _sample;

	public int this[int a]
	{
		get
		{
			return _sample;
		}
		set
		{
			_sample = value;
		}
	}

}


public class IndexerTest_old
{
	private int _sample;
	public int GetProperty()
	{
		return _sample;
	}
	public void SetProperty(int value)
	{
		_sample = value;
	}

}







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


//--------------------------------------------------------------------------------
//あとで分離するかもしれない
//--------------------------------------------------------------------------------


/// <summary>
/// ライブラリーセット　別のプロジェクトでも使うかもしれない
/// </summary>
public class custumLib
{
	///-------------------------------------------------------------------------------
	/// <summary>
	/// テキストからRCの数字を取得 (引数1:テキスト 引数2:RかCの1文字)
	/// </summary>
	///-------------------------------------------------------------------------------
	public static string substRC_Num(string textBase, string RC)
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

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スクリーンデータを取得
	/// </summary>
	///-------------------------------------------------------------------------------
	public static void ScreenData()
	{
		Camera MainCamera = Camera.main;
		float cameraSize = MainCamera.orthographicSize;

		float screenX = Screen.width;
		float screenY = Screen.height;
		float ratio = screenX / screenY;

		//スクリーン座標
		float yMin = cameraSize;
		float yMax = yMin * -1f;
		float xMin = yMin * ratio * -1f;
		float xMax = xMin * -1f;

		Debug.Log($"X({xMin})({xMax}) Y({yMin})({yMin})");
	}
}


