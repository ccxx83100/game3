using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberImage : MonoBehaviour
{
	public Sprite Number0, Number1, Number2, Number3, Number4, Number5, Number6, Number7, Number8, Number9, Minus, Dot, NoN;

	public void SetSprite(char num)
	{
		Dictionary<char, Sprite> dic = new Dictionary<char, Sprite>()
		{
			{'0', Number0},
			{'1', Number1},
			{'2', Number2},
			{'3', Number3},
			{'4', Number4},
			{'5', Number5},
			{'6', Number6},
			{'7', Number7},
			{'8', Number8},
			{'9', Number9},
			{'-', Minus},
			{'.', Dot},
			{'n', NoN}
		};

		SpriteRenderer img = GetComponent<SpriteRenderer>();
		img.sprite = dic[num];
	}
}

