using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LockingPolicy = Thalmic.Myo.LockingPolicy;
using Pose = Thalmic.Myo.Pose;
using UnlockType = Thalmic.Myo.UnlockType;
using VibrationType = Thalmic.Myo.VibrationType;
using System;
using UnityEngine.SceneManagement;
using System.Globalization;

public class Lever : MonoBehaviour {

public bool inTrigger = false;
Animator leverAnim;
public bool aMultiLever = false;
public bool arrows;
public ArrowTrap arrowTrap;
public bool trap;
public OpenTrapDoors openTrap;
public GameObject myo = null;

	void Start ()
	{
		leverAnim = GetComponent<Animator>();

	}

	void OnTriggerEnter ()
	{
		inTrigger = true;
	}

	void OnTriggerExit ()
	{
		inTrigger = false;
	}

	IEnumerator TriggerArrowTrap ()
	{
		leverAnim.SetTrigger ("throw");
		yield return new WaitForSeconds(0.5f);
		arrowTrap.FireArrows();
	}

	// Update is called once per frame
	void Update ()
	{
		if (inTrigger) {
			// Access the ThalmicMyo component attached to the Myo object.
			ThalmicMyo thalmicMyo = myo.GetComponent<ThalmicMyo> ();
			if (Input.GetKeyDown (KeyCode.E)||thalmicMyo.pose == Pose.DoubleTap) {
				leverAnim.SetTrigger ("throw");
				if (aMultiLever) {
					GameManager.instance.AddLever (gameObject);
				} else if (arrows) {
					StartCoroutine ("TriggerArrowTrap");
				} else if (trap) {
					openTrap.TriggerTraps();
				}else {
					StartCoroutine (GameManager.instance.DelayDoor (1.5f));
				}
			}
		}
	}
}
