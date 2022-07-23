using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GolfGameMain : MonoBehaviour
{
	public GameObject PartsPrefabA, PartsPrefabB, PartsPrefabC, CircleMap, BallPrefab;
	public Material MaterialWhite, MaterialRed;
	private GameObject SwitchObject;
	[SerializeField] Block[] Blocks;
	private float maxHp = 100, nowHp, nowHp2;
	GameObject HpBar, HpBar2;
	private Vector3 hpV3 = Vector3.one;
	private Vector3 hp2V3 = Vector3.one;
	TextMeshProUGUI getScore;

	///-------------------------------------------------------------------------------
	/// <summary>
	///	スタート前処理
	/// </summary>
	///-------------------------------------------------------------------------------
	void Awake()
	{
		Application.targetFrameRate = 60;
		HpBar = GameObject.Find("HpBarNow");
		HpBar2 = GameObject.Find("HpBarNow2");
		getScore = GameObject.Find("GetScore").GetComponent<TextMeshProUGUI>();
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// スタート処理
	/// </summary>
	///-------------------------------------------------------------------------------
	void Start()
	{
		nowHp = maxHp;
		nowHp2 = maxHp;
		hpUpDown();
		BallArray = new GameObject[ballMax];
		//初期180度 --> 22.5づつ
		Transform _parent = CircleMap.transform;
		int[] mapArray = new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		for (int i = 0; i < 16; i++)
		{
			if (mapArray[i] == 1)
			{
				SwitchObject = PartsPrefabC;
			}
			else
			{
				SwitchObject = PartsPrefabA;
			}
			GameObject FildObject = Instantiate(SwitchObject, Vector3.zero,
			Quaternion.Euler(-90, 22.5f * i, 180), _parent);
			FildObject.name = "Parts" + i;
		}
		CircleMap.transform.rotation = new Quaternion(0, 0, 0, -1);     //1回転させないとバグる

		BallManager(1);
		SpawnBlock();

		gsAnimator.speed = 0;
	}


	private int generateTime = 120;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// Update
	/// </summary>
	///-------------------------------------------------------------------------------
	void Update()
	{
		///ボール生成
		if (Time.frameCount % generateTime == 0)
		{
			BallManager(1);
		}

		///キー入力と自動で初期位置に戻る処理
		if (Input.GetKey(KeyCode.UpArrow)) { MapRotation(0); }
		else if (Input.GetKey(KeyCode.RightArrow)) { MapRotation(1); }
		else if (Input.GetKey(KeyCode.DownArrow)) { MapRotation(2); }
		else if (Input.GetKey(KeyCode.LeftArrow)) { MapRotation(3); }
		else
		{
			float _xRot = CircleMap.transform.rotation.x * 2;
			float _yRot = CircleMap.transform.rotation.y * 2;
			float _zRot = CircleMap.transform.rotation.z * 2;
			Quaternion qTarget = Quaternion.Euler(_xRot, _yRot, _zRot);
			Quaternion qNow = CircleMap.transform.rotation;
			CircleMap.transform.rotation = qTarget * qNow;
		}

		nowHp2 -= 1.0f;
		if (nowHp2 <= nowHp)
		{
			nowHp2 = nowHp;
		}
		hp2V3.x = nowHp2 / maxHp;
		HpBar2.transform.localScale = hp2V3;
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// HP増減
	/// </summary>
	///-------------------------------------------------------------------------------
	public void hpUpDown()
	{
		hpV3.x = nowHp / maxHp;
		HpBar.transform.localScale = hpV3;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// おじゃまブロック生成
	/// </summary>
	///-------------------------------------------------------------------------------
	public Block SpawnBlock()
	{
		Transform _parent = CircleMap.transform;
		Block _block = Instantiate(GetRandomBlock(), Vector3.zero,
		Quaternion.Euler(-90, Random.Range(0, 359), 0), _parent);
		return _block;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// おじゃまブロックランダム取得
	/// </summary>
	///-------------------------------------------------------------------------------
	Block GetRandomBlock()
	{
		int i = Random.Range(0, Blocks.Length);
		if (Blocks[i])
		{
			return Blocks[i];
		}
		else
		{
			return null;
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ボールの落下位置
	/// </summary>
	///-------------------------------------------------------------------------------
	private Vector3 BallGenPosition()
	{
		int rand = Random.Range(0, 4);
		Vector3 v3 = new Vector3();
		switch (rand)
		{
			case 0:
				v3 = new Vector3(-4.5f, 4f, 4.5f);
				break;
			case 1:
				v3 = new Vector3(-4.5f, 4f, -4.5f);
				break;
			case 2:
				v3 = new Vector3(4.5f, 4f, 4.5f);
				break;
			case 3:
				v3 = new Vector3(4.5f, 4f, -4.5f);
				break;
			default:
				v3 = new Vector3(0f, 0f, 0f);
				break;
		}
		return v3;
	}

	private int ballCount = 0, ballMax = 5, redCount = 1, ballTotalCount;
	public GameObject[] BallArray;
	public Animator gsAnimator;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// ボールの管理
	/// </summary>
	///-------------------------------------------------------------------------------
	public void BallManager(int value)
	{
		if (value > 0)      //value + -で処理
		{
			if (ballCount < ballMax)
			{
				ballCount += value;
				ballTotalCount += value;
				for (int i = 0; i < value; i++)
				{
					int rand = Random.Range(0, 5);
					GameObject Ball = Instantiate(BallPrefab, BallGenPosition(), Quaternion.identity);
					MeshRenderer BallMat = Ball.GetComponent<MeshRenderer>();
					bool _red = false;
					foreach (GameObject _go in BallArray)
					{
						if (_go != null)
						{
							MeshRenderer _m = _go.GetComponent<MeshRenderer>();
							if (materialName(_m.material.name) == MaterialRed.name)
							{
								_red = true;
							}
						}
					}
					if (_red) rand = 1;
					if (rand == 0)
					{
						BallMat.material = MaterialRed;
						Ball.name = "BallRed" + ballTotalCount;
					}
					else
					{
						BallMat.material = MaterialWhite;
						Ball.name = "BallWhite" + ballTotalCount;
					}
					BallAddManager(Ball);
				}
			}
		}
		else
		{
			ballCount += value;
		}
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// マテリアル名の(Instance)を削除
	/// </summary>
	///-------------------------------------------------------------------------------
	private string materialName(string _name)
	{
		string value = _name.Replace(" (Instance)", "");
		return value;
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ボール配列の追加処理  //削除処理
	/// </summary>
	///-------------------------------------------------------------------------------
	private void BallAddManager(GameObject _name)
	{
		for (int i = 0; i < ballMax; i++)
		{
			if (BallArray[i] == null)
			{
				BallArray[i] = _name;
				break;
			}
		}
		//DebugArrayClass.Array1DLog(BallArray);
	}

	private float hpDamage = 25;
	public void BallRemoveManager(GameObject _name)
	{
		Destroy(_name);
		MeshRenderer _m = _name.GetComponent<MeshRenderer>();
		if (materialName(_m.material.name) == MaterialRed.name)
		{
			nowHp -= hpDamage;
			if (nowHp <= 0)
			{
				nowHp = 0;
			}
			hpUpDown();
		}
		else
		{
			int score = 100;
			ScoreScript.SetScore(score);
			getScore.text = score.ToString();
			gsAnimator.speed = 1;
			gsAnimator.Play("StateGetScore", 0, 0.0f);
		}


		int count = 0;
		foreach (GameObject _go in BallArray)
		{
			if (_go == _name)
			{
				BallArray[count] = null;
			}
			count++;
		}
		//DebugArrayClass.Array1DLog(BallArray);
	}



	///-------------------------------------------------------------------------------
	/// <summary>
	/// フィールド回転処理
	/// </summary>
	///-------------------------------------------------------------------------------
	public void MapRotation(int _move)
	{
		float rotMax = 25.0f;
		Quaternion _q = Quaternion.Euler(0, 0, 0);
		float power = 1.0f;
		float mapRotX = 0, mapRotZ = 0;
		CircleMap.transform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
		//無限(Infinity)時のバグ回避
		if (axis.y == float.NaN || axis.y == float.NegativeInfinity || axis.y == float.PositiveInfinity)
		{
			axis = new Vector3(0, 0, 0);
		}
		Vector3 qAngle = Quaternion.AngleAxis(angle, axis).eulerAngles;
		mapRotX = 180 - Mathf.Abs(180 - qAngle.x);
		mapRotZ = 180 - Mathf.Abs(180 - qAngle.z);
		//_move  0:↑  1:→  2:↓  3:← 
		switch (_move)
		{
			case 0:
				if (mapRotX <= rotMax)
				{
					_q = Quaternion.Euler(power, 0, 0);
				}
				break;
			case 1:
				if (mapRotZ <= rotMax)
				{
					_q = Quaternion.Euler(0, 0, power * -1);
				}
				break;
			case 2:
				if (mapRotX <= rotMax)
				{
					_q = Quaternion.Euler(power * -1, 0, 0);
				}
				break;
			case 3:
				if (mapRotZ <= rotMax)
				{
					_q = Quaternion.Euler(0, 0, power);
				}
				break;
			default:
				_q = Quaternion.Euler(0, 0, 0);
				break;
		}
		Quaternion _q2 = CircleMap.transform.rotation;
		CircleMap.transform.rotation = _q2 * _q;
	}

}
