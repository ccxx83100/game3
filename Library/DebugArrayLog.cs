using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
デバッグ用 --unityで配列出力できないので

使い方
DebugArrayLog dal = new DebugArrayLog();
dal.Array1DLog(*****配列****);
dal.Array2DLog(**二次元配列**);

*/


public class DebugArrayLog
{
	//-------------------------------------------------------------------
	//配列出力
	//-------------------------------------------------------------------
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

	//-------------------------------------------------------------------
	//二次元配列出力
	//-------------------------------------------------------------------
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

	//-------------------------------------------------------------------
	//三次元配列出力
	//-------------------------------------------------------------------
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