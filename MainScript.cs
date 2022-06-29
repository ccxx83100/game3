using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class MainScript : MonoBehaviour
{
	public GameObject PanelPrefab, BallPrefab, BallPrefab2, BallPrefab3, BreakEffectPrefab, PanelBreakPrefab;      //プレハブ設定用のGameObject
	public float panelOneSize, panelScale;
	public float defaultX, defaultY, panelScaleMin, cameraSize, panelSize;
	public float[,,] panelVecter2XY;
	public string[,] goArray;
	public int[,] panelArray, hintArray;
	private int[] nowPosition;
	private int breakCount;
	[SerializeField] public int Serialize_StageNo;
	private int onePanelPoint = 30;
	public int resetCost = 30;
	public int hintCost = 200;
	public int[] loadCsvArray;
	public bool startFlg, endFlg;                           //スタート　エンドフラグ
	public bool escapeFlg = true;                           //ステージセレクトバグ回避フラグ
	private bool hintFlg;                                   //ヒントは1回なので
	DebugArrayLog dal = new DebugArrayLog();                //配列デバッグ用のインスタンス
	CSVImporter csvi = new CSVImporter();                   //CSV読み書き込み用のインスタンス
	StageList stl = new StageList();                        //ステージリストのインスタンス
	public int bestScore, nowScore, nowScoreView, totalScore;
	public float ballMove = 0.15f;                          //ボールの移動速度
	public const float ballRotation = 11.0f;                //ボールの回転速度
	public float ballMoveTime;                              //ボール速度がバグるのでTime.deltaTimeを仮実装

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
			AnimationStop(StageClearAnimObject, "thisPlay");
			AnimationStop(NewRecoadAnimObject, "thisPlay");
			GameObject.FindGameObjectWithTag("NewRecord").transform.position = new Vector3(-1000, 0, -0.1f);
			GameObject.FindGameObjectWithTag("SettingWindow").GetComponent<RectTransform>()
			.anchoredPosition = new Vector3(1800.0f, 0.0f, -110.0f);
		}

		testTest(); //後で消す

	}
	private GameObject Bo;
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
				ClickBreak();
			}
		}
		if (panelArray != null)
		{
			int iCount = 0;
			foreach (int i in panelArray) if (i == 1) iCount++;
			if (iCount == 1)
			{
				AnimationPlay(StageClearAnimObject, "thisPlay");
				Clear_func();
			}
		}
	}

	private int panelNumX, panelNumY;
	private int maxDeleteCount, nowDeleteCount;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタートとリセット処理・共通部と各処理
	/// </summary>
	///-------------------------------------------------------------------------------
	public void StartReset_Fnc(int stgNo)
	{
		ballMoveTime = ballMove * Time.deltaTime * 100.0f;

		GameObject.Find("NextButton").transform.position = new Vector3(0f, 7.0f, 0f);
		GameObject.Find("BackGroundCollider").transform.position = new Vector3(0, 0, -0.5f);

		cameraSize = Camera.main.orthographicSize;
		CustumLib.ScreenData();

		//ステージリストから値を取得		
		var (_startPos, _stageArray, _hintArray, _breakCount) = stl.StageSetUP(stgNo);

		//現在地配列 [] [,]
		nowPosition = _startPos;
		panelArray = _stageArray;
		hintArray = _hintArray;
		breakCount = _breakCount;

		panelNumX = panelArray.GetLength(1);            //パネルの横数
		panelNumY = panelArray.GetLength(0);            //パネルの縦数

		if (panelNumX >= panelNumY)
		{
			panelScale = 0.50f / 1.125f / (panelNumX - 2);
		}
		else
		{
			panelScale = 0.50f / 1.125f / (panelNumY - 2);
		}

		panelScaleMin = panelScale * 1.0f;              //マージン分を縮小する
		panelOneSize = (panelScale * cameraSize) * 2;   //パネルの移動(複製パネルの移動座標)

		//パネルセットの初期位置
		panelSize = panelScale * cameraSize;

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
		ImageNo imgNo_Now = GameObject.Find("BreakCountNow").GetComponent<ImageNo>();
		ImageNo imgNo_Max = GameObject.Find("BreakCountMax").GetComponent<ImageNo>();
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
		PanelSetUP_Fnc();       //パネル配置

		Vector3 ballPos = new Vector3(0, 0, 0);
		ballPos.x = defaultX + panelOneSize * nowPosition[1];
		ballPos.y = defaultY - panelOneSize * nowPosition[0];
		Bo = Instantiate(BallPrefab3, ballPos, Quaternion.identity) as GameObject;
		float _ranX = Random.Range(0.0f, 360.0f);
		float _ranY = Random.Range(0.0f, 360.0f);
		float _ranZ = Random.Range(0.0f, 360.0f);
		Quaternion rotQ = Quaternion.Euler(_ranX, _ranY, _ranZ);
		Bo.transform.rotation = rotQ;
		Bo.name = "Ball";
		float _prefabScale = 0.176f;         //BallPrefab=1  BallPrefab2=0.19f	 BallPrefab2=0.176f
		float ballScale = cameraSize * 2 * panelScale * _prefabScale;
		Bo.transform.localScale = new Vector3(ballScale, ballScale, ballScale);

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

		BoStop_Fnc();

		GameObject.FindGameObjectWithTag("NewRecord").transform.position = new Vector3(-1000, 0, -0.1f);

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
		var (_startPos, _stageArray, _hintArray, _breakCount) = stl.StageSetUP(Serialize_StageNo);

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
		AnimationStop(StageClearAnimObject, "thisPlay");
	}

	public Animator StageClearAnimObject, NewRecoadAnimObject;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// アニメ停止 (引数1:Animator 引数2:Motion		速度調整で停止して、0フレーム目に移動)
	/// </summary>
	///-------------------------------------------------------------------------------
	void AnimationStop(Animator _anim, string _str)
	{
		_anim.Play(_str, 0, 0);
		_anim.speed = 0;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// アニメ再生 (引数1:Animator 引数2:Motion		速度調整で再生)
	/// </summary>
	///-------------------------------------------------------------------------------
	void AnimationPlay(Animator _anim, string _str)
	{
		_anim.speed = 1;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// タグ検索してTextMeshProのテキストを変更 (引数1:string 検索タグ  引数2:T 変更後のテキスト)
	/// </summary>
	///-------------------------------------------------------------------------------
	public void TagSarchAndChangeText<T>(string _tagName, T _param)
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
	/// [!スコア更新] クリア後の処理
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

			if (Serialize_StageNo != stl.switchStage)
			{
				GameObject NextButton = GameObject.Find("NextButton");
				NextButton.transform.position = new Vector3(0.0f, -2.9f, 0f);
			}
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
				csvi.CsvSave(loadCsvArray);

				GameObject.FindGameObjectWithTag("NewRecord").transform.position = new Vector3(0, 0, -0.6f);
				AnimationPlay(NewRecoadAnimObject, "thisPlay");
			}
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// [!スコア更新] 総コストを減らす (引数:コストの額)
	/// </summary>
	///-------------------------------------------------------------------------------
	public void CostDown(int _cost)
	{
		if (totalScore >= _cost)
		{
			totalScore -= _cost;
			loadCsvArray = csvi.loadScoreArray;
			loadCsvArray[0] = totalScore;
			TagSarchAndChangeText("ScoreCost", totalScore);

			csvi.CsvSave(loadCsvArray);
		}
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// [!スコア更新] スコア計算と表示
	/// </summary>
	///-------------------------------------------------------------------------------
	void ScoreSet()
	{
		nowScore = 0;
		nowScoreView = 0;

		totalScore = csvi.loadScoreArray[0];
		bestScore = csvi.loadScoreArray[Serialize_StageNo];

		TagSarchAndChangeText("ScoreBest", bestScore);
		TagSarchAndChangeText("ScoreNow", nowScore);
		TagSarchAndChangeText("ScoreCost", totalScore);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// [!スコア更新] スコアアップ処理 (1フレーム毎にスコアを加算する)
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

	///-------------------------------------------------------------------------------
	/// <summary>
	/// クリックしたパネルを削除
	/// </summary>
	///-------------------------------------------------------------------------------
	void ClickBreak()
	{
		if (nowDeleteCount != 0)
		{
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			bool destroyFlg = false;
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
							destroyFlg = true;
							break;
						}
					}
				}
			}
			if (destroyFlg && panelArray[_row, _col] != 0)
			{
				string destroyName = "Po_R" + _row + "C" + _col;
				if (_row == nowPosition[0] && _col == nowPosition[1])
				{
					//ボールがあるパネルは削除できません
				}
				else
				{
					Debug.Log("Delete?? " + _col + ":" + _row);
					Delete_Fnc(destroyName);
					nowDeleteCount--;

					ImageNo imgNo_Now = GameObject.Find("BreakCountNow").GetComponent<ImageNo>();
					imgNo_Now.SpriteNumSet(nowDeleteCount);
				}
			}
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// パネルの配置/インスタンスを生成し座標を配列に入れる
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


	///-------------------------------------------------------------------------------
	/// <summary>
	/// [!スコア更新] [重要]ボールオブジェクトの移動・停止・回転・移動した場所の削除
	/// </summary>
	///-------------------------------------------------------------------------------
	private void BoMove_Fnc()
	{
		string md_mouseLRUD = MouseDrag.mouseLRUD;
		Vector3 _pos = Bo.transform.position;

		float _ballMoveTime = ballMove * Time.deltaTime * 100.0f;

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
						_pos.x = _pos.x + _ballMoveTime;
						Bo.transform.Rotate(0.0f, (ballRotation * -1), 0.0f, Space.World);
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
						_pos.x = _pos.x - _ballMoveTime;
						Bo.transform.Rotate(0.0f, ballRotation, 0.0f, Space.World);
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
						_pos.y = _pos.y + _ballMoveTime;
						Bo.transform.Rotate(ballRotation, 0.0f, 0.0f, Space.World);
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
						_pos.y = _pos.y - _ballMoveTime;
						Bo.transform.Rotate((ballRotation * -1), 0.0f, 0.0f, Space.World);
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
		MouseDrag MouseDragScript = GameObject.Find("BackGroundCollider").GetComponent<MouseDrag>();
		MouseDragScript.OnMouseDown();
		MouseDrag.mouseLRUD = "STOP";
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// オブジェクトを削除する (引数1:オブジェクトではなく、オブジェクト名)
	/// </summary>
	///-------------------------------------------------------------------------------
	private void Delete_Fnc(string deleteObjectName)
	{
		GameObject DeleteObject = GameObject.Find(deleteObjectName);
		Destroy(DeleteObject);

		//配列から削除
		int getC = int.Parse(CustumLib.substRC_Num(DeleteObject.name, "C"));
		int getR = int.Parse(CustumLib.substRC_Num(DeleteObject.name, "R"));
		panelArray[getC, getR] = 0;
		goArray[getC, getR] = null;

		Particle_Fnc(DeleteObject);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 爆発エフェクト (引数1:爆発させたいインスタンスがある場所を指定)
	/// ※※パーティクルからスプライトアニメになっているのできれいにしたい
	/// </summary>
	///-------------------------------------------------------------------------------
	private void Particle_Fnc(GameObject obj)
	{
		GameObject _effectGO = obj;
		GameObject Pto = Instantiate(PanelBreakPrefab, new Vector3(_effectGO.transform.position.x, _effectGO.transform.position.y, -0.09f), Quaternion.identity) as GameObject;
		float _effectScale = panelScaleMin * 7.3f;      //1080/512??
		Pto.transform.localScale = new Vector3(_effectScale, _effectScale, _effectScale);
		StartCoroutine(DestroyPaticle(2.0f, Pto));      //コールチンを使用し遅延処理
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// コールチンでパーティクルを再生後に削除
	/// </summary>
	///-------------------------------------------------------------------------------
	IEnumerator DestroyPaticle(float _delay, GameObject _destroyObject)
	{
		yield return new WaitForSeconds(_delay);
		if (_destroyObject != null)
		{
			Destroy(_destroyObject);
		}
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
	/// <summary>
	/// 後で削除
	/// </summary>
	///-------------------------------------------------------------------------------
	private void testTest()
	{
		ScoreClass SC = new ScoreClass();

		IndexerTest IT = new IndexerTest();
		int a = IT[0];
		//Debug.Log(IT.GetSetProperty);

		IndexerTest_old ITO = new IndexerTest_old();
		ITO.SetProperty(100);

		TryCatch TC = new TryCatch();
		//TC.TryC();


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

///-------------------------------------------------------------------------------
/// <summary>
/// try catch
/// </summary>
///-------------------------------------------------------------------------------

public class TryCatch
{

	int waru = 1;
	public void TryC()
	{
		Debug.Log("tryc");

		try
		{
			int _a = 3 / waru;
			Debug.Log(_a);
		}
		catch
		{
			Debug.Log("hogehoge");
		}
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
	public void ScoreSet()
	{
		CSVImporter csvi = new CSVImporter();
		MainScript _ms = GameObject.Find("GameMain").GetComponent<MainScript>();
		//GameMainを探してコンポーネントでスクリプトを参照

		_ms.nowScore = 0;
		_ms.nowScoreView = 0;

		_ms.totalScore = csvi.loadScoreArray[0];
		_ms.bestScore = csvi.loadScoreArray[_ms.Serialize_StageNo];

		_ms.TagSarchAndChangeText("ScoreBest", _ms.bestScore);
		_ms.TagSarchAndChangeText("ScoreNow", _ms.nowScore);
		_ms.TagSarchAndChangeText("ScoreCost", _ms.totalScore);
	}
}


//--------------------------------------------------------------------------------
//あとで分離するかもしれない
//--------------------------------------------------------------------------------

///-------------------------------------------------------------------------------
/// <summary>
/// ライブラリーセット　別のプロジェクトでも使うかもしれない
/// </summary>
///-------------------------------------------------------------------------------
public class CustumLib
{
	///-------------------------------------------------------------------------------
	/// <summary>
	/// テキストからRCの数字を取得 (引数1:テキスト 引数2:RかCの1文字)
	/// 配列取得 testBase[i] にすると、バグる？
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
		float setCameraSize = cameraSize * 0.5625f / ratio;

		//スクリーン座標
		float yMin = cameraSize;
		float yMax = yMin * -1f;
		float xMin = yMin * ratio * -1f;
		float xMax = xMin * -1f;

	}
}


