using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------
/// <summary>
///デバッグ用 --unityで配列出力できないので
///
///使い方 インスタンスを作る
///DebugArrayLog dal = new DebugArrayLog();
///dal.Array1DLog(*****配列****);
///dal.Array2DLog(**二次元配列**);
/// </summary>
///-------------------------------------------------------------------------------
public class DebugArrayLog
{
	///-------------------------------------------------------------------------------
	/// <summary>
	/// 配列出力 (型指定なし 引数1:array)
	/// </summary>
	///-------------------------------------------------------------------------------
	public void Array1DLog<T>(T[] arr)
	{
		string arrayPrint = "(--DebugArrayLog-(1D)--)\n";
		for (int i = 0; i < arr.GetLength(0); i++)
		{
			if (i != arr.GetLength(0) - 1)
			{
				if (arr[i] != null)
				{
					arrayPrint += arr[i].ToString() + ",";
				}
				else
				{
					arrayPrint += null + ",";
				}
			}
			else
			{
				if (arr[i] != null)
				{
					arrayPrint += arr[i].ToString();
				}
				else
				{
					arrayPrint += null;
				}
			}
		}
		Debug.Log(arrayPrint);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 二次元配列出力 (型指定なし 引数1:array)
	/// </summary>
	///-------------------------------------------------------------------------------
	public void Array2DLog<T>(T[,] arr)
	{
		string arrayPrint = "(--DebugArrayLog-(2D)--)\n";
		for (int i = 0; i < arr.GetLength(0); i++)
		{
			for (int j = 0; j < arr.GetLength(1); j++)
			{
				if (j != arr.GetLength(1) - 1)
				{
					if (arr[i, j] != null)
					{
						arrayPrint += arr[i, j].ToString() + ",";
					}
					else
					{
						arrayPrint += null + ",";
					}
				}
				else
				{
					if (arr[i, j] != null)
					{
						arrayPrint += arr[i, j];
					}
					else
					{
						arrayPrint += null;
					}
				}
			}

			if (i != arr.GetLength(0) - 1)
			{
				arrayPrint += "\n";
			}
		}
		Debug.Log(arrayPrint);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 三次元配列出力 (型指定なし 引数1:array)
	/// </summary>
	///-------------------------------------------------------------------------------
	public void Array3DLog<T>(T[,,] arr)
	{
		string arrayPrint = "(--DebugArrayLog-(3D)--)\n";
		for (int i = 0; i < arr.GetLength(0); i++)
		{
			for (int j = 0; j < arr.GetLength(1); j++)
			{
				for (int k = 0; k < arr.GetLength(2); k++)
				{
					if (k == 0)
					{
						arrayPrint += "[";
					}
					if (k == arr.GetLength(2) - 1)
					{
						if (arr[i, j, k] != null)
						{
							arrayPrint += arr[i, j, k].ToString();
						}
						else
						{
							arrayPrint += null;
						}
						if (j != arr.GetLength(1) - 1)
						{
							arrayPrint += "],";
						}
						else
						{
							arrayPrint += "]";
						}
					}
					else
					{
						if (arr[i, j, k] != null)
						{
							arrayPrint += arr[i, j, k].ToString() + ",";
						}
						else
						{
							arrayPrint += null + ",";
						}
					}
				}
			}
			if (i != arr.GetLength(0) - 1)
			{
				arrayPrint += "\n";
			}
		}
		Debug.Log(arrayPrint);
	}
}

///-------------------------------------------------------------------------------
/// <summary>
///デバッグ用 --unityでリスト出力できないので
///
///使い方
///DebugListLog dal = new DebugListLog();
///dal.List1DLog(*****リスト****);
/// </summary>
///-------------------------------------------------------------------------------
public class DebugListLog2
{
	///-------------------------------------------------------------------------------
	/// <summary>
	/// リスト出力
	/// </summary>
	///-------------------------------------------------------------------------------
	public void List1DLog<T>(List<T> arr)
	{
		string arrayPrint = "(--DebugListLog-(1D)--)\n";
		for (int i = 0; i < arr.Count; i++)
		{
			if (i != arr.Count - 1)
			{
				if (arr[i] != null)
				{
					arrayPrint += arr[i].ToString() + ",";
				}
				else
				{
					arrayPrint += null + ",";
				}
			}
			else
			{
				if (arr[i] != null)
				{
					arrayPrint += arr[i].ToString();
				}
				else
				{
					arrayPrint += null;
				}
			}
		}
		Debug.Log(arrayPrint);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// 二次元リスト出力 (バグるので開発中)
	/// </summary>
	///-------------------------------------------------------------------------------
	public void List2DLog<T>(List<T>[] arr)
	{

	}
}