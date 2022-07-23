using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

///-------------------------------------------------------------------------------
/// <summary>
/// マウス処理 UIはインターフェイスの継承をする
/// </summary>
///-------------------------------------------------------------------------------
public class MouseDragRotation : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
	private Vector2 startPosition;
	GolfGameMain mainScript;

	void Awake()
	{
		mainScript = GameObject.Find("GameMain").GetComponent<GolfGameMain>();
	}

	///-------------------------------------------------------------------------------
	/// <summary>
	/// クリック処理
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnPointerClick(PointerEventData eventData)
	{
		//Debug.Log("OnPointerClick:" + eventData);
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		//Debug.Log("OnPointerDown");
		startPosition = eventData.position;
		Debug.Log("OnPointerDown:" + startPosition);
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		//Debug.Log("OnPointerUp");
	}

	private float minDrag = 2;
	///-------------------------------------------------------------------------------
	/// <summary>
	/// ドラッグ処理
	/// </summary>
	///-------------------------------------------------------------------------------
	public void OnDrag(PointerEventData eventData)
	{
		//Debug.Log("OnDrag:" + eventData);
		float xMove = eventData.position.x - startPosition.x;
		float yMove = eventData.position.y - startPosition.y;
		float absX = Mathf.Abs(xMove);
		float absY = Mathf.Abs(yMove);
		float signX = Mathf.Sign(xMove);
		float signY = Mathf.Sign(yMove);

		if (absX > absY && absX >= minDrag)
		{
			if (signX == 1)
			{
				mainScript.MapRotation(1);
				startPosition = eventData.position;
			}
			else if (signX == -1)
			{
				mainScript.MapRotation(3);
				startPosition = eventData.position;
			}
		}
		else if (absX < absY && absY >= minDrag)
		{
			if (signY == 1)
			{
				mainScript.MapRotation(0);
				startPosition = eventData.position;
			}
			else if (signY == -1)
			{
				mainScript.MapRotation(2);
				startPosition = eventData.position;
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
	}

	public void OnEndDrag(PointerEventData eventData)
	{
	}
}
