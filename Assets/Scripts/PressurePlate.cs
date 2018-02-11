using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour {

Animator plateAnim;
public string pressureParam;
public float waitTime;
public bool needsWeight; 
public bool makeObjectAppear;
public ObjectAppear objectAppear;

	// Use this for initialization
	void Start () {
		plateAnim = GetComponent<Animator>();
	}
	
	void OnTriggerEnter (Collider other)
	{
		plateAnim.SetTrigger (pressureParam);
		if (!makeObjectAppear) {
			StartCoroutine (GameManager.instance.DelayDoor (waitTime));
		} else {
			objectAppear.MakeObjAppear();
		}
		
	}

	void OnTriggerExit ()
	{
		if (needsWeight) {
			plateAnim.SetTrigger (pressureParam);
			if (!makeObjectAppear) {
				GameManager.instance.CloseDoor ();
			} else {
				objectAppear.MakeObjectDisappear();
			}
		}
	}
}
