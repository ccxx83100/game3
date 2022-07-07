using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.UI;

///-------------------------------------------------------------------------------
/// <summary>
/// ジャンプゲームメイン
/// </summary>
///-------------------------------------------------------------------------------
public class JumpGameMain : MonoBehaviour
{
	public GameObject PanelPrefab, EffectPrefab, NumberPrefab;
	public GameObject AlphaCollider, SecondAlphaCollider, BallObject;
	private const float panelSizeX = 1.00f;                 //パネルの横サイズ
	private const float panelSizeY = 0.25f;                 //パネルの縦サイズ  (0.18f)
	private const float panelZeroPosY = -0.7f;              //パネルの初期Y位置
	private const float cameraSlideX = 5f;                  //メインカメラをずらす
	private const float alphaColliderSideX = -4.65f;
	private const float secondAlphaColliderSideX = -15.0f;
	public Rigidbody rbBall, rbAlpha;
	public Camera mainCamera, subCamera;
	public Vector3 cameraPos, subCameraPos;
	DebugArrayLog dal = new DebugArrayLog();
	DebugListLog dll = new DebugListLog();
	public bool ballJumpFlg;
	public List<string> destroyList = new List<string>();
	private int[] mapArray;
	private int[,] mapArray2;
	private float velocityJumpY = 10.0f; //10.0f
	private int panelCount = 0;
	private const int generateNum = 100;
	private GameObject Circle;
	private Image fill;
	private List<int> mapList;
	private int checkPoint = 100;
	private float startXPos = 0, startYpos = 2;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 処理落ちするのでFPS調整
	/// </summary>
	///-------------------------------------------------------------------------------
	void Awake()
	{
		Application.targetFrameRate = 60;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// START
	/// </summary>
	///-------------------------------------------------------------------------------
	public float nowMeter, startMeter, moveMeter;
	void Start()
	{
		mapList = new List<int>()
		{ 0, 100, 200, 300, 400, 500, 600, 700, 800, 900 };             //マップが増えたら追加
																		//mapArray = csvDataSet.CsvMapLoad();                             //データ読み込み
																		//mapArray = csvDataSet.CsvMapLoad();
		csvDataSet cs = new csvDataSet();
		mapArray2 = cs.CsvMapLoad2();
		Physics.gravity = new Vector3(0, -20, 0);                       //世界の重力

		BallObject = GameObject.Find("Ball");
		rbBall = BallObject.GetComponent<Rigidbody>();
		AlphaCollider = GameObject.Find("AlphaCollider");
		rbAlpha = BallObject.GetComponent<Rigidbody>();
		//AlphaCollider.GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255, 0);
		SecondAlphaCollider = GameObject.Find("SecondAlphaCollider");
		SecondAlphaCollider.GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255, 0);
		startMeter = BallObject.transform.position.x;
		Circle = GameObject.Find("Circle01");
		fill = Circle.GetComponent<Image>();

		Vector3 ballStartPostion = new Vector3(startXPos, startYpos, 0);
		BallObject.transform.position = ballStartPostion;
		PanelGenerate(0);

		//スコア周りの初期処理
		Destroy(GameObject.Find("NumberPlus"));                         //ダミーデータ削除
		Transform parent = GameObject.Find("ScoreSet").transform;       //親
		numInst = new GameObject[metorStrLength];
		float leftXpos = -393.0f;
		for (int i = 0; i < metorStrLength; i++)
		{
			//最大値分のインスタンスを用意 0→10000  1→1000  2→100  3→10  4→1  5→. 6→.0
			numInst[i] = Instantiate(NumberPrefab, new Vector3(0, 0, 0), Quaternion.identity, parent);
			RectTransform rectNumIns = numInst[i].GetComponent<RectTransform>();
			float _x = leftXpos + i * 60;
			rectNumIns.anchoredPosition = new Vector3(_x, 690f, 0);
			numInst[i].GetComponent<NumberImage>().SetSprite('n');
		}
	}

	private int metorStrLength = 6;     //桁数
	GameObject[] numInst;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// パネルを生成  引数1:最小値
	/// </summary>
	///-------------------------------------------------------------------------------
	private void PanelGenerate(int _min)
	{
		int _max = _min + generateNum;
		GameObject PanelObject;
		Transform parent;
		float _xPos, _yPos;
		/*
		for (int i = _min; i < _max; i++)      //int i = 0; i < mapArray.GetLength(0); i++
		{
			if (mapArray[i] != 0)
			{
				panelCount++;
				_xPos = i * panelSizeX;
				_yPos = (mapArray[i] - 1) * panelSizeY + panelZeroPosY;
				parent = GameObject.Find("GameTop").transform;
				PanelObject = Instantiate(PanelPrefab, new Vector3(_xPos, _yPos, 0f), Quaternion.Euler(90f, 0f, 0f), parent) as GameObject;
				PanelObject.name = "PanelObj" + panelCount;
			}
		}
		*/

		for (int i = _min; i < _max; i++)      //int i = 0; i < mapArray.GetLength(0); i++
		{
			//1行目
			if (mapArray2[0, i] != 0)
			{
				panelCount++;
				_xPos = i * panelSizeX;
				_yPos = (mapArray2[0, i] - 1) * panelSizeY + panelZeroPosY;
				parent = GameObject.Find("GameTop").transform;
				PanelObject = Instantiate(PanelPrefab, new Vector3(_xPos, _yPos, 0f), Quaternion.Euler(90f, 0f, 0f), parent) as GameObject;
				PanelObject.name = "PanelObj" + panelCount;
			}

			//2行目
			if (mapArray2[1, i] != 0)
			{
				panelCount++;
				_xPos = i * panelSizeX;
				_yPos = (mapArray2[1, i] - 1) * panelSizeY + panelZeroPosY;
				parent = GameObject.Find("GameTop").transform;
				PanelObject = Instantiate(PanelPrefab, new Vector3(_xPos, _yPos, 0f), Quaternion.Euler(90f, 0f, 0f), parent) as GameObject;
				PanelObject.name = "PanelObj" + panelCount;
			}
		}
	}

	private float targetVelocity = 5;                   //初期速度
	private float targetVelocityMin = 5;                //最低速度
	private float targetVelocityMax = 9;                //最高速度
	private float power = 20;                           //なんだろう？
	public Vector3 ballVelocity;

	private float oneFrameSpeedUp = 0.008f;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// UPDATE
	/// </summary>
	///-------------------------------------------------------------------------------
	void Update()
	{
		ballVelocity = rbBall.velocity;     //velocity表示用

		//INPUT操作
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0)) Jump();

		//追従
		cameraPos = new Vector3(BallObject.transform.localPosition.x + cameraSlideX, BallObject.transform.localPosition.y + 2.5f, -20.0f);
		mainCamera.transform.localPosition = cameraPos;
		subCameraPos = new Vector3(BallObject.transform.localPosition.x + 20, 3, -40.0f);
		subCamera.transform.localPosition = subCameraPos;

		Vector3 backPos = new Vector3(BallObject.transform.localPosition.x + alphaColliderSideX, BallObject.transform.localPosition.y, 0);
		AlphaCollider.transform.localPosition = backPos;
		Vector3 backPosSe = new Vector3(BallObject.transform.localPosition.x + secondAlphaColliderSideX, -42, 0);
		SecondAlphaCollider.transform.localPosition = backPosSe;

		//移動	
		targetVelocity += oneFrameSpeedUp;
		if (targetVelocity >= targetVelocityMax)
		{
			targetVelocity = targetVelocityMax;
		}
		if (ballJumpFlg)
		{
			rbBall.AddForce(Vector3.right * ((targetVelocity - rbBall.velocity.x) * power), ForceMode.Acceleration);
			//rbAlpah.AddForce(Vector3.right * ((targetVelocity - rbBall.velocity.x) * power), ForceMode.Acceleration);

			//float targetVelocityY = 20;
			//rbBall.AddForce(Vector3.down * ((targetVelocityY - rbBall.velocity.y)), ForceMode.Acceleration);
		}

		//速度メーター
		float per = (targetVelocity / targetVelocityMax);
		float circleCut = per * 3 / 4;
		fill.fillAmount = circleCut;

		//移動距離
		nowMeter = BallObject.transform.position.x;
		moveMeter = nowMeter - startMeter;
		meterDisplay(moveMeter);

		//パネル生成
		if (nowMeter >= (checkPoint - 50) && mapList.Contains(checkPoint) == true)
		{
			int maplistIndex = mapList.IndexOf(checkPoint);
			PanelGenerate(checkPoint);
			mapList.Remove(checkPoint);
			checkPoint += 100;
		}

	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// メートル表示
	/// </summary>
	///-------------------------------------------------------------------------------
	private void meterDisplay(float _moveMeter)
	{
		float score = _moveMeter;
		string scoreStr = score.ToString("N1");         //スコアを小数点1位で切り上げて文字列に
		int scoreLen = scoreStr.Length;
		Transform parent = GameObject.Find("ScoreSet").transform;

		for (int i = 0; i < scoreLen; i++)
		{
			char spritChar = scoreStr[i];
			numInst[i].GetComponent<NumberImage>().SetSprite(spritChar);
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 爆発して削除
	/// </summary>
	///-------------------------------------------------------------------------------
	public void DestroyEffect(GameObject breakPanel, string listName)
	{
		Vector3 _pos = breakPanel.transform.position;
		GameObject EO = Instantiate(EffectPrefab, _pos, Quaternion.identity);
		Destroy(breakPanel);
		destroyList.Remove(listName);
		StartCoroutine(DelayDestroy(0.6f, EO));     //コールチン
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 遅れて処理
	/// </summary>
	///-------------------------------------------------------------------------------
	IEnumerator DelayDestroy(float delay, GameObject go)
	{
		yield return new WaitForSeconds(delay);
		Destroy(go);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ジャンプ
	/// </summary>
	///-------------------------------------------------------------------------------
	public int jumpCount = 0;
	private void Jump()
	{
		if (ballJumpFlg && jumpCount <= 1)
		{
			jumpCount++;
			//ballJumpFlg = false;
			rbAlpha.velocity = new Vector3(rbAlpha.velocity.x, velocityJumpY, 0);
			//なんか当たる
			rbBall.velocity = new Vector3(rbBall.velocity.x, velocityJumpY, 0);

			targetVelocity -= 0.8f;
			if (targetVelocity <= targetVelocityMin)
			{
				targetVelocity = targetVelocityMin;
			}
			//rbBall.AddForce(Vector3.up * (velocityJumpY * 100), ForceMode.Impulse);
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 小数点1位にする
	/// </summary>
	///-------------------------------------------------------------------------------
	float NumDot(float num)
	{
		float value = num * 10;
		value = Mathf.Floor(value);
		value *= 10;
		return value;
	}
}


///-------------------------------------------------------------------------------
/// <summary>
/// csvクラス
/// </summary>
///-------------------------------------------------------------------------------
public class csvDataSet
{
	DebugArrayLog dal = new DebugArrayLog();
	///-------------------------------------------------------------------------------
	/// <summary>
	/// CSVロード retrun int[,]  System.IO System.LINQ
	/// use:streamingAssets   
	/// </summary>
	///-------------------------------------------------------------------------------
	public static int[] CsvMapLoad()
	{
		string csvName = "MapData.csv";
		string csvPath = System.IO.Path.Combine(Application.streamingAssetsPath, csvName);
		string csvData = File.ReadAllText(csvPath);
		string[] csvArrayString = csvData.Split(",");
		int[] mapArray = csvArrayString.Select(int.Parse).ToArray();
		return mapArray;
	}


	public int[,] CsvMapLoad2()
	{
		string csvName = "MapData.csv";
		string csvPath = System.IO.Path.Combine(Application.streamingAssetsPath, csvName);

		//string[] csvArrayStringStrA = csvData.Split("\n");
		//string[] csvArrayString = csvData.Split(",");

		//int[] mapArray = csvArrayString.Select(int.Parse).ToArray();
		//dal.Array1DLog(csvArrayString);

		//int[] mapArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13, 13, 13, 14, 15, 16, 17, 18, 19, 20, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 1, 1, 0, 0, 1, 1, 1, 3, 3, 3, 5, 6, 6, 7, 7, 7, 9, 9, 9, 10, 10, 10, 11, 11, 11, 12, 12, 12, 13, 13, 0, 0, 0, 14, 14, 15, 15, 15, 16, 16, 16, 0, 0, 0, 16, 16, 16, 15, 15, 14, 13, 12, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 5, 4, 5, 4, 5, 4, 5, 4, 5, 4, 5, 4, 12, 12, 12, 13, 14, 15, 16, 17, 18, 19, 20, 19, 18, 17, 0, 0, 0, 0, 0, 0, 5, 6, 7, 8, 9, 10, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };


		List<string> dataList = new List<string>();
		int count = 0;
		try
		{
			using (StreamReader csvData = new StreamReader(csvPath))
			{
				while (csvData.Peek() >= 0)
				{
					string _data = csvData.ReadLine();
					dataList.Add(_data);
					count++;
				}
			}
		}
		catch (System.Exception)
		{
			Debug.Log("Error");
		}
		finally { }


		Debug.Log(count);
		string[] str0 = dataList[0].Split(",");
		string[] str1 = dataList[1].Split(",");
		int xLen = str0.GetLength(0);

		//	int i0 = int.Parse(s0);
		//	int i1 = int.Parse(s1);
		int[,] mapArray = new int[count, xLen];
		for (int i = 0; i < xLen; i++)
		{
			mapArray[0, i] = int.Parse(str0[i]);
			mapArray[1, i] = int.Parse(str1[i]);
		}

		//		mapArray[0] = int.Parse(str0);
		//		mapArray[1] = int.Parse(str1);


		//int[] mapArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13, 13, 13, 14, 15, 16, 17, 18, 19, 20, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, 1, 1, 0, 0, 1, 1, 1, 3, 3, 3, 5, 6, 6, 7, 7, 7, 9, 9, 9, 10, 10, 10, 11, 11, 11, 12, 12, 12, 13, 13, 0, 0, 0, 14, 14, 15, 15, 15, 16, 16, 16, 0, 0, 0, 16, 16, 16, 15, 15, 14, 13, 12, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0, 0, 5, 4, 5, 4, 5, 4, 5, 4, 5, 4, 5, 4, 12, 12, 12, 13, 14, 15, 16, 17, 18, 19, 20, 19, 18, 17, 0, 0, 0, 0, 0, 0, 5, 6, 7, 8, 9, 10, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
		return mapArray;

	}
}

