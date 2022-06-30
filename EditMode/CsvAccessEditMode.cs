using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

///-------------------------------------------------------------------------------
/// <summary>
/// CSVクラス
/// </summary>
///-------------------------------------------------------------------------------
public class CsvAccessEditMode
{
	/*
    private int[] output_startPos;
	private int[,] output_hintArray;
	private int[,] output_stageArray;
	private int output_breakCount = 0;
    */
	DebugArrayLog dal = new DebugArrayLog();

	///-------------------------------------------------------------------------------
	/// <summary>
	/// CSVセーブ 引数1:int[] input_startPos    引数2:2int[,] input_hintArray
	/// 引数3:int input_breakCount   引数4:int[,] input_stageArray
	/// </summary>
	///-------------------------------------------------------------------------------
	public void CsvSave(int[] input_startPos,
						int[,] input_hintArray,
						int input_breakCount,
						int[,] input_stageArray)
	{
		int[] output_startPos = input_startPos;
		int[,] output_hintArray = input_hintArray;
		int[,] output_stageArray = input_stageArray;
		int output_breakCount = input_breakCount;

		string csvPath = Application.persistentDataPath + "/map_data.csv";
		string saveTxt = "";                //memo""にしないと+=が使えない
		if (!System.IO.File.Exists(csvPath))
		{
			///-------------------------------------------------------------------------------
			/// ファイルがなければ生成
			///-------------------------------------------------------------------------------

			saveTxt += "1,1\n";                 //startPos
			saveTxt += "2,1,3,1\n";             //hintArray
			saveTxt += "1\n";                   //breakCount
			saveTxt += "0,0,0,0,0,0\n";         //stageArray
			saveTxt += "0,1,1,1,1,0\n";
			saveTxt += "0,0,1,1,1,0\n";
			saveTxt += "0,1,1,1,0,0\n";
			saveTxt += "0,0,0,0,0,0";
			File.WriteAllText(csvPath, saveTxt);
		}
		else
		{
			///-------------------------------------------------------------------------------
			/// 保存できる形に整形
			///-------------------------------------------------------------------------------
			//startPos
			//output_startPos = new int[] { 2, 1 };
			saveTxt += output_startPos[0] + "," + output_startPos[1] + "\n";
			//output_hintArray = new int[,] { { 1, 2 }, { 3, 1 } };

			//hintArray
			if (output_hintArray != null)
			{
				for (int i = 0; i < output_hintArray.GetLength(0); i++)
				{
					for (int j = 0; j < output_hintArray.GetLength(1); j++)
					{
						if (i == output_hintArray.GetLength(0) - 1 && j == output_hintArray.GetLength(1) - 1)
						{
							saveTxt += output_hintArray[i, j];
						}
						else
						{
							saveTxt += output_hintArray[i, j] + ",";
						}
					}
				}
			}
			saveTxt += "\n";

			//breakCount
			//output_breakCount = 4;
			saveTxt += output_breakCount + "\n";

			//stageArray
			//output_stageArray = new int[,] { { 0, 0, 0, 0, 0 }, { 0, 1, 0, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 1, 1, 1, 0 }, { 0, 0, 0, 0, 0 } };
			if (output_stageArray != null)
			{
				for (int i = 0; i < output_stageArray.GetLength(0); i++)
				{
					for (int j = 0; j < output_stageArray.GetLength(1); j++)
					{
						if (i == output_stageArray.GetLength(0) - 1 && j == output_stageArray.GetLength(1) - 1)
						{
							saveTxt += output_stageArray[i, j];
						}
						else if (j == output_stageArray.GetLength(1) - 1)
						{
							saveTxt += output_stageArray[i, j] + "\n";
						}
						else
						{
							saveTxt += output_stageArray[i, j] + ",";
						}
					}
				}
			}

			Debug.Log("保存データ:\n" + saveTxt);
			File.WriteAllText(csvPath, saveTxt);
		}
	}


	///-------------------------------------------------------------------------------
	/// <summary>
	/// CSVロード
	/// </summary>
	///-------------------------------------------------------------------------------
	public (int[], int[,], int[,], int) CsvLoad()
	{
		string csvPath = Application.persistentDataPath + "/map_data.csv";
		List<string> _list = new List<string>();
		int count = 0;
		try
		{
			using (StreamReader csvData = new StreamReader(csvPath))
			{
				while (csvData.Peek() >= 0)
				{
					string _data = csvData.ReadLine();
					_list.Add(_data);
					count++;
				}
			}
		}
		catch (System.Exception)
		{
			Debug.Log("Error");
		}
		finally { }

		//0:startPos 1:hintArray 2:breakCount 3~end:stageArray
		//0:startPos
		//sprit → fix
		string[] sprit_startPos = _list[0].Split(",");
		int[] fix_startPos = sprit_startPos.Select(int.Parse).ToArray();

		//1:hintArray
		string[] sprit_hintArray = _list[1].Split(",");
		int row_hintArray = sprit_hintArray.GetLength(0) / 2;
		int[,] fix_hintArray = new int[row_hintArray, 2];
		int count_hint = 0;
		for (int i = 0; i < row_hintArray; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				fix_hintArray[i, j] = int.Parse(sprit_hintArray[count_hint]);
				count_hint++;
			}
		}

		//2:breakCount
		string[] sprit_breakCount = _list[2].Split(",");
		int fix_breakCount = int.Parse(sprit_breakCount[0]);

		//3~end:stageArray
		int _array0 = count - 3;
		int _array1 = (_list[3].Length - 1) / 2 + 1;
		string[] sprit_stageArray;
		int[,] fix_stageArray = new int[_array0, _array1];
		int count_stage = 0;
		for (int i = 0; i < _array0; i++)
		{
			sprit_stageArray = _list[(i + 3)].Split(",");
			for (int j = 0; j < _array1; j++)
			{
				fix_stageArray[i, j] = int.Parse(sprit_stageArray[j]);

			}
			count_stage++;
		}

		//出力用(後で消す)
		Debug.Log("---ロードデータ---");
		dal.Array1DLog(fix_startPos);
		dal.Array2DLog(fix_hintArray);
		Debug.Log(fix_breakCount);
		dal.Array2DLog(fix_stageArray);
		//var (_startPos, _stageArray, _hintArray, _breakCount)		

		return (fix_startPos, fix_stageArray, fix_hintArray, fix_breakCount);
	}
}