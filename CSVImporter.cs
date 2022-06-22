using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

///-------------------------------------------------------------------------------
/// <summary>
/// CSVの読み書き
/// </summary>
///-------------------------------------------------------------------------------
public class CSVImporter
{
	public int[] loadScoreArray;
	string saveTxt;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// CSVを保存 (引数:int 配列　※多次元不可能)
	/// パスはWindowsのみ確認
	/// </summary>
	///-------------------------------------------------------------------------------
	public void CsvSave(int[] arr)
	{
		string csvPath = Application.persistentDataPath + "/save_data.csv";
		saveTxt = "";
		for (int i = 0; i < arr.GetLength(0); i++)
		{
			if (i == 0)
			{
				saveTxt = saveTxt + arr[i];
			}
			else
			{
				saveTxt = saveTxt + "," + arr[i];
			}
		}
		File.WriteAllText(csvPath, saveTxt);
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// CSVの読み込み　ファイルがない場合は作成
	/// </summary>
	///-------------------------------------------------------------------------------
	public void CsvLoad()
	{
		string csvPath = Application.persistentDataPath + "/save_data.csv";
		saveTxt = "";
		Debug.Log($"{csvPath}");
		if (!System.IO.File.Exists(csvPath))
		{
			//ファイルがなければ生成
			File.WriteAllText(csvPath, "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0");
			Debug.Log("デフォルト設定を保存しました");
		}
		string csvData = File.ReadAllText(csvPath);
		string[] csvArrayString = csvData.Split(",");
		loadScoreArray = csvArrayString.Select(int.Parse).ToArray();

		for (int i = 0; i < loadScoreArray.GetLength(0); i++)
		{
			if (i == 0)
			{
				saveTxt = saveTxt + loadScoreArray[i];
			}
			else
			{
				saveTxt = saveTxt + "," + loadScoreArray[i];
			}
		}
	}
}