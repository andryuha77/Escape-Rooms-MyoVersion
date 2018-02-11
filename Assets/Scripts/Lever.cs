using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

public bool inTrigger = false;
Animator leverAnim;
public bool aMultiLever = false;
public bool arrows;
public ArrowTrap arrowTrap;
public bool trap;
public OpenTrapDoors openTrap;


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
			if (Input.GetKeyDown (KeyCode.E)) {
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
