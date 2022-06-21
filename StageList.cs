using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///-------------------------------------------------------------------------------
/// <summary>
/// ステージリスト
/// </summary>
///-------------------------------------------------------------------------------
public class StageList
{
	private int[] startPos;
	private int[,] stageArray, hintArray;
	private int breakCount;
	private float stageScale;

	/// <summary>
	/// 総ステージ数 [重要※！]ステージ数を増やしたら増やす
	/// </summary>
	public int switchStage = 10;

	///-------------------------------------------------------------------------------
	/// <summary>
	/// ステージ情報 (引数:ステージ番号)
	/// </summary>
	/// <param name="stageNo">ステージ番号</param>
	/// <returns>ボールの初期位置 int[], ステージ配列 int[,], ヒント配列 int[,], パネルのサイズ float, Break数</returns>
	///-------------------------------------------------------------------------------
	public (int[], int[,], int[,], float, int) StageSetUP(int stageNo)
	{
		switch (stageNo)
		{
			//-------------------------------------------------------------------------------
			//Stage: 1 OK
			//-------------------------------------------------------------------------------
			case 1:
				stageScale = 0.09f;
				startPos = new int[] { 1, 1 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,0,0,0,0,0,0}
				};
				hintArray = new int[,] { };
				breakCount = 0;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 2 OK
			//-------------------------------------------------------------------------------
			case 2:
				stageScale = 0.09f;
				startPos = new int[] { 1, 3 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0,0},
						{0,1,1,1,1,1,0,0},
						{0,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,0},
						{0,0,0,0,0,0,0,0}
				};
				hintArray = new int[,] { { 3, 5 } };
				breakCount = 2;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 3 OK
			//-------------------------------------------------------------------------------
			case 3:
				stageScale = 0.08f;
				startPos = new int[] { 4, 2 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0},
						{0,1,1,1,1,0},
						{0,1,1,1,1,0},
						{0,1,1,1,0,0},
						{0,1,1,1,0,0},
						{0,1,1,1,1,0},
						{0,1,1,1,1,0},
						{0,1,1,1,1,0},
						{0,0,0,0,0,0}
				};
				hintArray = new int[,] { { 6, 3 } };
				breakCount = 1;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 4 OK
			//-------------------------------------------------------------------------------
			case 4:
				stageScale = 0.09f;
				startPos = new int[] { 2, 2 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0,0},
						{0,1,1,1,1,1,0,0},
						{0,1,1,1,1,1,0,0},
						{0,1,1,1,1,1,0,0},
						{0,0,1,0,0,1,1,0},
						{0,0,1,1,1,1,0,0},
						{0,0,0,0,0,0,0,0},
				};
				hintArray = new int[,] { { 2, 4 } };
				breakCount = 2;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 5 OK
			//-------------------------------------------------------------------------------
			case 5:
				stageScale = 0.065f;
				startPos = new int[] { 2, 2 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0,0,0,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,0,0,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,0,0,0,0,1,1,0},
						{0,1,1,0,0,0,0,1,1,0},
						{0,1,1,0,0,0,0,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,0,0,0,0,0,0,0,0,0},
				};
				hintArray = new int[,] { { 3, 3 }, { 3, 4 } };
				breakCount = 2;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 6 OK
			//-------------------------------------------------------------------------------
			case 6:
				stageScale = 0.06f;
				startPos = new int[] { 1, 1 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0,0,0,0},
						{0,1,1,1,1,1,0,1,1,0},
						{0,1,1,1,1,1,0,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,0,0,1,1,1,0},
						{0,1,1,1,0,0,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,0,1,0,1,1,1,0},
						{0,1,1,0,1,1,1,1,1,0},
						{0,0,0,0,0,0,0,0,0,0},
				};
				hintArray = new int[,] { { 1, 4 }, { 6, 5 } };
				breakCount = 3;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 7 OK
			//-------------------------------------------------------------------------------
			case 7:
				stageScale = 0.068f;
				startPos = new int[] { 2, 2 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0,0,0,0},
						{0,1,1,1,1,1,1,1,0,0},
						{0,1,1,1,1,1,0,1,1,0},
						{0,1,1,1,0,1,0,1,1,0},
						{0,0,1,1,0,1,1,1,1,0},
						{0,0,1,1,0,1,0,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,0,0,0,0},
						{0,0,0,0,0,0,0,0,0,0},
				};
				hintArray = new int[,] { { 6, 1 }, { 4, 2 }, { 3, 7 } };
				breakCount = 3;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 8 OK
			//-------------------------------------------------------------------------------
			case 8:
				stageScale = 0.068f;
				startPos = new int[] { 2, 4 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0,0,0,0},
						{0,0,0,1,1,1,1,1,1,0},
						{0,0,0,1,1,1,1,1,1,0},
						{0,0,1,1,0,1,1,1,1,0},
						{0,0,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,0,0,1,1,1,1,1,1,0},
						{0,0,0,1,1,1,1,1,1,0},
						{0,0,0,0,0,0,0,0,0,0},
				};
				hintArray = new int[,] { { 5, 3 }, { 4, 5 } };
				breakCount = 2;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 9 OK
			//-------------------------------------------------------------------------------
			case 9:
				stageScale = 0.068f;
				startPos = new int[] { 5, 6 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0,0,0,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,1,1,1,1,1,1,1,1,0},
						{0,0,0,0,0,0,0,0,0,0},
				};
				hintArray = new int[,] { { 5, 5 } };
				breakCount = 1;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 10 NG
			//-------------------------------------------------------------------------------
			case 10:
				stageScale = 0.08f;
				startPos = new int[] { 1, 1 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,0,0,0,0,0,0},
				};
				hintArray = new int[,] { };
				breakCount = 9;
				break;
			//-------------------------------------------------------------------------------
			//Stage: 11 NG
			//-------------------------------------------------------------------------------
			case 11:
				stageScale = 0.08f;
				startPos = new int[] { 1, 1 };
				stageArray = new int[,]
				{
						{0,0,0,0,0,0,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,1,1,1,1,1,0},
						{0,0,0,0,0,0,0},
				};
				hintArray = new int[,] { };
				breakCount = 9;
				break;
			default:
				stageScale = 0.25f;
				startPos = new int[] { 0, 0 };
				stageArray = new int[,]
				{
						{1,1},
						{1,1}
				};
				hintArray = new int[,] { };
				breakCount = 2;
				break;
		}
		//戻り値-------------------------------------------------------------------------------
		return (startPos, stageArray, hintArray, stageScale, breakCount);
	}
}
