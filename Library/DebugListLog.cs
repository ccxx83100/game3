using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


/*
デバッグ用 --unityでリスト出力できないので

使い方
DebugListLog dal = new DebugListLog();
dal.List1DLog(*****リスト****);

*/

public class DebugListLog
{

	//-------------------------------------------------------------------
	//リスト出力
	//-------------------------------------------------------------------
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

	//-------------------------------------------------------------------
	//二次元配列出力
	//-------------------------------------------------------------------
	public void List2DLog<T>(List<T>[] arr)
	{
		Debug.Log(arr[0].Count);
		/*
		string arrayPrint = "(--DebugListLog-(2D)--)\n";
		for (int i = 0; i < arr[0].Count; i++)
		{
			for (int j = 0; j < arr[1].Count; j++)
			{
				if (j != arr[1].Count - 1)
				{
					if (arr[i][j] != null)
					{
						arrayPrint += arr[i][j].ToString() + ",";
					}
					else
					{
						arrayPrint += null + ",";
					}
				}
				else
				{
					if (arr[i][j] != null)
					{
						arrayPrint += arr[i][j];
					}
					else
					{
						arrayPrint += null;
					}
				}
			}

			if (i != arr[0].Count - 1)
			{
				arrayPrint += "\n";
			}
		}
		*/
		//Debug.Log(arrayPrint);
	}
}

