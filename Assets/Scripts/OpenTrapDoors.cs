using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenTrapDoors : MonoBehaviour {
public List <GameObject> trapDoors = new List<GameObject>();

public void TriggerTraps ()
	{
		foreach (GameObject trap in trapDoors) {
			TrapDoor trapdoor = trap.GetComponent<TrapDoor>();
			Rigidbody rbL = trapdoor.leftDoor.GetComponent<Rigidbody>();
			Rigidbody rbr = trapdoor.rightDoor.GetComponent<Rigidbody>();
			rbL.useGravity = true;
			rbL.isKinematic = false;
			rbr.useGravity = true;
			rbr.isKinematic = false;
		}
	}
}
