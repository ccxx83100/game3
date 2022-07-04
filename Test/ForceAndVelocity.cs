using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceAndVelocity : MonoBehaviour
{

	Rigidbody rb_Fo, rb_Vo;
	private float upForce;
	private float jumpSpeed;


	void Start()
	{
		GameObject Fo = GameObject.Find("_Force");
		GameObject Vo = GameObject.Find("_Velocity");
		rb_Fo = Fo.GetComponent<Rigidbody>();
		rb_Vo = Vo.GetComponent<Rigidbody>();

		upForce = 500;
		jumpSpeed = 10;

	}


	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			rb_Fo.AddForce(new Vector3(0, upForce, 0));
			rb_Vo.velocity = new Vector3(rb_Vo.velocity.x, jumpSpeed, rb_Vo.velocity.z);
		}
	}
}


