using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailSoftArray<T>
{
	private T[] _a;
	public int Length { get; private set; }
	public bool IsError { get; private set; }   //直前の操作の結果を表す

	/// <summary>
	/// サイズを指定して配列を作る
	/// </summary>
	public FailSoftArray(int size)
	{
		_a = new T[size];
		Length = size;
	}

	/// <summary>
	/// FailSoftArrayオブジェクトのためのインデクサー
	/// </summary>
	public T this[int index]
	{
		get
		{
			if (!IsSafe(index))
			{
				IsError = true;
				return default(T);
			}
			IsError = false;
			return _a[index];
		}
		set
		{
			if (!IsSafe(index))
			{
				IsError = true;
				return;
			}
			IsError = false;
			_a[index] = value;
		}
	}

	/// <summary>
	/// インデックスが配列の上限と下限の範囲内ならtrueを返す
	/// </summary>
	private bool IsSafe(int index)
	{
		if (index < 0) return false;
		if (index >= Length) return false;

		return true;
	}
}


public class Hoge : MonoBehaviour
{
	private void Start()
	{
		FailSoftArray<int> failSoftArray = new FailSoftArray<int>(10);

		failSoftArray[5] = 1;                                       //これは正常に代入される
		if (failSoftArray.IsError) Debug.Log("配列の範囲外です");   //これは実行されない

		int x = failSoftArray[12];                                  //これは配列の範囲外なので0が代入される
		if (failSoftArray.IsError) Debug.Log("配列の範囲外です");   //これは実行される
	}

	private void aaa()
	{
		FailSoftArray<int> testArray = new FailSoftArray<int>(10);

		testArray[0] = 1;
	}

}