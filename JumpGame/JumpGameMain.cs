using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

///-------------------------------------------------------------------------------
/// <summary>
/// ジャンプゲームメイン
/// </summary>
///-------------------------------------------------------------------------------
public class JumpGameMain : MonoBehaviour
{

	public GameObject PanelPrefab, EffectPrefab, NumberPrefab;
	private const float panelSizeX = 1.12f;                 //パネルの横サイズ
	private const float panelSizeY = 0.15f;                 //パネルの縦サイズ
	private const float panelZeroPosY = -2.7f;              //パネルの初期Y位置
	private const float cameraSlideX = 4.0f;
	private const float alphaColliderSideX = -5.2f;
	private const float secondAlphaColliderSideX = -15.0f;
	public Rigidbody rbBall;
	public GameObject BallObject;
	public Camera mainCamera;
	public Vector3 cameraPos;
	DebugArrayLog dal = new DebugArrayLog();
	public bool ballJumpFlg;
	public List<string> destroyList = new List<string>();
	private int[] mapArray;
	private float velocityJumpY = 8.0f;
	private int panelCount = 0;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// START
	/// </summary>
	///-------------------------------------------------------------------------------

	public float nowMeter, startMeter, moveMeter;
	void Start()
	{
		mapArray = csvDataSet.CsvMapLoad();                 //データ読み込み
		Physics.gravity = new Vector3(0, -20, 0);           //世界の重力
		PanelGenerate(0, 300);
		BallObject = GameObject.Find("Ball");
		rbBall = BallObject.GetComponent<Rigidbody>();
		AlphaCollider = GameObject.Find("AlphaCollider");
		AlphaCollider.GetComponent<MeshRenderer>().material.color = new Color(255, 255, 255, 0);
		SecondAlphaCollider = GameObject.Find("SecondAlphaCollider");

		startMeter = GameObject.Find("Ball").transform.position.x;

		//.anchoredPosition
		//-393 -303 -213
		/*
				Transform parent = GameObject.Find("ScoreSet").transform;
				GameObject numIns = Instantiate(NumberPrefab, new Vector3(0, 0, 0), Quaternion.identity, parent);
				RectTransform rectNumIns = numIns.GetComponent<RectTransform>();
				rectNumIns.anchoredPosition = new Vector3(-213f, 715f, 0);
				NumberImage NI = numIns.GetComponent<NumberImage>();
				NI.SetSprite('1');
				*/
		GameObject destNum = GameObject.Find("NumberPlus");
		Destroy(destNum);

	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// パネルを生成  引数1:最小値 引数2:最大値
	/// </summary>
	///-------------------------------------------------------------------------------
	private void PanelGenerate(int _min, int _max)
	{
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

	public Vector3 ballVelocity;
	GameObject AlphaCollider, SecondAlphaCollider;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// UPDATE
	/// </summary>
	///-------------------------------------------------------------------------------
	void Update()
	{
		//ballVelocity = rbBall.velocity;     //velocity表示用

		//INPUT操作
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0)) Jump();

		//追従
		cameraPos = new Vector3(BallObject.transform.localPosition.x + cameraSlideX, BallObject.transform.localPosition.y + 3.0f, -20.0f);
		mainCamera.transform.localPosition = cameraPos;
		Vector3 backPos = new Vector3(BallObject.transform.localPosition.x + alphaColliderSideX, BallObject.transform.localPosition.y, 0);
		AlphaCollider.transform.localPosition = backPos;
		Vector3 backPosSe = new Vector3(BallObject.transform.localPosition.x + secondAlphaColliderSideX, BallObject.transform.localPosition.y, 0);
		SecondAlphaCollider.transform.localPosition = backPosSe;

		//移動
		const float targetVelocity = 8;
		const float power = 20;
		if (ballJumpFlg)
		{
			rbBall.AddForce(Vector3.right * ((targetVelocity - rbBall.velocity.x) * power), ForceMode.Acceleration);
		}

		nowMeter = GameObject.Find("Ball").transform.position.x;
		moveMeter = nowMeter - startMeter;


		GameObject[] numObjects;
		numObjects = GameObject.FindGameObjectsWithTag("Numbers");
		foreach (GameObject i in numObjects)
		{
			Destroy(i);
		}

		float score = moveMeter;
		string scoreStr = score.ToString("N1");         //スコアを小数点1位で切り上げて文字列に
		int scoreLen = scoreStr.Length;

		Debug.Log(scoreStr);

		for (int i = 0; i < scoreLen; i++)
		{
			char spritChar = scoreStr[i];
			Transform parent = GameObject.Find("ScoreSet").transform;
			GameObject numIns = Instantiate(NumberPrefab, new Vector3(0, 0, 0), Quaternion.identity, parent);
			RectTransform rectNumIns = numIns.GetComponent<RectTransform>();
			float _x = -393 + i * 90;
			rectNumIns.anchoredPosition = new Vector3(_x, 715f, 0);
			NumberImage NI = numIns.GetComponent<NumberImage>();
			NI.SetSprite(spritChar);
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
		GameObject IO = Instantiate(EffectPrefab, _pos, Quaternion.identity);
		Destroy(breakPanel);
		destroyList.Remove(listName);

		StartCoroutine(DelayDestroy(2.0f, IO));     //コールチン
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
	private void Jump()
	{
		if (ballJumpFlg)
		{
			ballJumpFlg = false;
			rbBall.velocity = new Vector3(rbBall.velocity.x, velocityJumpY, 0);
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

