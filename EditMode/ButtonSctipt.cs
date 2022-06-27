using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSctipt : MonoBehaviour
{
	public void OnClickGnerate()
	{
		EditModeMain em = GameObject.Find("EditModeMain").GetComponent<EditModeMain>();
		em.EnterAndClick();
	}

	public void ConsoleSclole()
	{

	}
}
