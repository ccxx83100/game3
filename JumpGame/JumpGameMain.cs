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
	private const float panelSizeY = 0.15f;                 //パネルの縦サイズ
	private const float panelZeroPosY = -0.7f;              //パネルの初期Y位置
	private const float cameraSlideX = 4.0f;
	private const float alphaColliderSideX = -4.65f;
	private const float secondAlphaColliderSideX = -15.0f;
	public Rigidbody rbBall;
	public Camera mainCamera, subCamera;
	public Vector3 cameraPos, subCameraPos;
	DebugArrayLog dal = new DebugArrayLog();
	public bool ballJumpFlg;
	public List<string> destroyList = new List<string>();
	private int[] mapArray;
	private float velocityJumpY = 10.0f;
	private int panelCount = 0;
	private const int generateNum = 100;
	private GameObject Circle;
	private Image fill;
	private List<int> mapList;
	private int checkPoint = 100;
	private float startXPos = 0, startYpos = 2;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// START
	/// </summary>
	///-------------------------------------------------------------------------------

	public float nowMeter, startMeter, moveMeter;
	void Start()
	{
		mapList = new List<int>() { 0, 100, 200, 300, 400, 500, 600, 700, 800, 900 };

		mapArray = csvDataSet.CsvMapLoad();                             //データ読み込み
		Physics.gravity = new Vector3(0, -20, 0);                       //世界の重力

		BallObject = GameObject.Find("Ball");
		rbBall = BallObject.GetComponent<Rigidbody>();
		AlphaCollider = GameObject.Find("AlphaCollider");
		AlphaCollider.GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255, 0);
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
			float _x = leftXpos + i * 90;
			rectNumIns.anchoredPosition = new Vector3(_x, 715f, 0);
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


	}

	private float targetVelocity = 5;                   //初期速度
	private float targetVelocityMin = 5;                //最低速度
	private float targetVelocityMax = 9;                //最高速度
	private float power = 20;                           //なんだろう？
	public Vector3 ballVelocity;
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
		targetVelocity += 0.003f;
		if (targetVelocity >= targetVelocityMax)
		{
			targetVelocity = targetVelocityMax;
		}
		if (ballJumpFlg)
		{
			rbBall.AddForce(Vector3.right * ((targetVelocity - rbBall.velocity.x) * power), ForceMode.Acceleration);
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
}

