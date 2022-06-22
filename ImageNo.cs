using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ImageNo : MonoBehaviour
{
	[SerializeField]
	private GameObject numberPrefab;  // スプライト表示用オブジェクト(プレハブ)
	[SerializeField]
	private Sprite _0, _1, _2, _3, _4, _5, _6, _7, _8, _9, _Minus;      // 数字スプライト
	[SerializeField]
	private GameObject[] numSpriteGird;             // 表示用スプライトオブジェクトの配列
	private Dictionary<char, Sprite> dicSprite;     // スプライトディクショナリ

	public void SpriteNumSet(int value)
	{

		float width = 0;        //2桁にした時用
		SpriteRenderer _spr = GetComponent<SpriteRenderer>();
		_spr.color = new Color(0f / 255.0f, 0.0f / 255.0f, 0.0f / 255.0f, 0f / 255.0f);
		//数字
		string numStr = value.ToString();

		// 現在表示中のオブジェクト削除
		if (numSpriteGird != null)
		{
			foreach (var i in numSpriteGird)
			{
				GameObject.Destroy(i);
			}
		}

		//数字ディクショナリ
		dicSprite = new Dictionary<char, Sprite>() { { '0', _0 }, { '1', _1 }, { '2', _2 }, { '3', _3 }, { '4', _4 }, { '5', _5 }, { '6', _6 }, { '7', _7 }, { '8', _8 }, { '9', _9 }, { '-', _Minus }, };

		Vector3 _localScale = transform.localScale / 186;           //インスタンスにしたらサイズが合わないので無理やり調整

		GameObject _ui = GameObject.Find("UIheader");
		Transform _tf = _ui.transform;

		//桁数分オブジェクトを生成 一桁なのでいずれ変更
		numSpriteGird = new GameObject[numStr.Length];
		for (var i = 0; i < numSpriteGird.Length; ++i)
		{

			//インスタンス作成
			numSpriteGird[i] = Instantiate(numberPrefab, transform.position + new Vector3((float)i * width, 0), Quaternion.identity) as GameObject;
			numSpriteGird[i].transform.localScale = _localScale;
			numSpriteGird[i].transform.SetParent(_tf);
			//表示する数値指定
			numSpriteGird[i].GetComponent<SpriteRenderer>().sprite = dicSprite[numStr[i]];
		}
	}
}

