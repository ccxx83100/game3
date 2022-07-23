using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScript : MonoBehaviour
{
	public static void SetScore(int _score)
	{
		TextMeshProUGUI TotalScore = GameObject.Find("TotalScore").GetComponent<TextMeshProUGUI>();
		int scoreText = int.Parse(TotalScore.text) + _score;
		TotalScore.text = scoreText.ToString();
	}
}
