using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGameMain : MonoBehaviour
{


	GameObject sp1, sp2, sp3, sp4, bodyDown;
	Rigidbody rbSp1, rbSp2, rbSp3, rbSp4, rbBodyDown;



	void Awake()
	{
		//--車


		sp1 = GameObject.Find("Sphere001");
		sp2 = GameObject.Find("Sphere002");
		sp3 = GameObject.Find("Sphere003");
		sp4 = GameObject.Find("Sphere004");
		rbSp1 = sp1.GetComponent<Rigidbody>();
		rbSp2 = sp2.GetComponent<Rigidbody>();
		rbSp3 = sp3.GetComponent<Rigidbody>();
		rbSp4 = sp4.GetComponent<Rigidbody>();

		bodyDown = GameObject.Find("BodyDown");



		//--
	}


	void Start()
	{

	}


	// Update is called once per frame
	void Update()
	{
		Vector3 a3 = new Vector3(1, 0, 0);
		rbSp1.AddForce(a3, ForceMode.Acceleration);
		rbSp2.AddForce(a3, ForceMode.Acceleration);
		rbSp3.AddForce(a3, ForceMode.Acceleration);
		rbSp4.AddForce(a3, ForceMode.Acceleration);
	}
}
