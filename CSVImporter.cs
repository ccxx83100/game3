using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class CSVImporter
{
	/*
		static TextAsset csvFile;
		static List<string[]> csvDataList = new List<string[]>();
	*/


	//-------------------------------------------------------------------------------------------------------------------------
	//保存 save
	//-------------------------------------------------------------------------------------------------------------------------
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

	//-------------------------------------------------------------------------------------------------------------------------
	//読み込み Load
	//-------------------------------------------------------------------------------------------------------------------------
	public int[] loadScoreArray;
	string saveTxt = "";
	public void CsvLoad()
	{
		string csvPath = Application.persistentDataPath + "/save_data.csv";
		Debug.Log($"{csvPath}");
		if (!System.IO.File.Exists(csvPath))
		{
			//ファイルがなければ生成
			File.WriteAllText(csvPath, "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0");
			Debug.Log("デフォルト保存保存しました");
		}
		string csvData = File.ReadAllText(csvPath);
		//Debug.Log("LoadData: " + csvData);
		string[] csvArrayString = csvData.Split(",");
		loadScoreArray = csvArrayString.Select(int.Parse).ToArray();

		DebugArrayLog dll = new DebugArrayLog();
		//dll.Array1DLog(loadScoreArray);


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

		//Debug.Log(saveTxt);


		/*	StringReader _stReader = new StringReader(csvData);
				while (_stReader.Peek() != -1)
				{
					string rline = _stReader.ReadLine();
					csvDataList.Add(rline.Split(','));
				}
				string _dataPreview = "";
				for (int i = 0; i < csvDataList[0].Length; i++)
				{
					if (i == 0)
					{
						_dataPreview = _dataPreview + csvDataList[0][i];
					}
					else
					{
						_dataPreview = _dataPreview + "," + csvDataList[0][i];
					}
				}
				*/


		//Debug.Log(_dataPreview);

	}


	public static void CsvReader()
	{
		//拡張子を入れない
		//csvFile = Resources.Load("save_data") as TextAsset;
		//


		/*
				StringReader _stReader = new StringReader(csvFile.text);
				while (_stReader.Peek() != -1)
				{
					string rline = _stReader.ReadLine();
					csvDataList.Add(rline.Split(','));
				}

				string _dataPreview = "";
				for (int i = 0; i < csvDataList[0].Length; i++)
				{
					if (i == 0)
					{
						_dataPreview = _dataPreview + csvDataList[0][i];
					}
					else
					{
						_dataPreview = _dataPreview + "," + csvDataList[0][i];
					}
					//Debug.Log(csvDataList[0][i]);
				}
				*/

		//Debug.Log(_dataPreview);




		//return;
		/*
		string csvData = File.ReadAllText(csvPath);



		Debug.Log(csvPath);
		Debug.Log("saveData: " + csvData);

		StringReader _stReader = new StringReader(csvData);
		while (_stReader.Peek() != -1)
		{
			string rline = _stReader.ReadLine();
			csvDataList.Add(rline.Split(','));
		}

		string[] csvDataAttay = csvData.Split(',');

		DebugArrayLog dal = new DebugArrayLog();
		dal.Array1DLog(csvDataAttay);


		string _dataPreview = "";
		for (int i = 0; i < csvDataList[0].Length; i++)
		{
			if (i == 0)
			{
				_dataPreview = _dataPreview + csvDataList[0][i];
			}
			else
			{
				_dataPreview = _dataPreview + "," + csvDataList[0][i];
			}
		}

		Debug.Log("_dataPreview:" + _dataPreview);
*/







		//		dll.List2DLog(csvDataList);

		/*
				// 書き込み
				string path = Application.persistentDataPath + "/save_data.csv";
				Debug.Log(path);
				File.WriteAllText(path, "hoge");

				// 追記
				File.AppendAllText(path, "fuga");
				Debug.Log("Save at: " + path);

				// 読み込み
				string data = File.ReadAllText(path);
				Debug.Log("Data is: " + data);
				*/



	}
}