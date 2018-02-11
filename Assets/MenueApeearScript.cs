using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenueApeearScript : MonoBehaviour {


	public GameObject menu; // Assign in inspector
	private bool isShowing = false;

	void Update() {
		if (Input.GetKey(KeyCode.Q))
		{
			this.gameObject.SetActive(false);
		}
		else if (Input.GetKey(KeyCode.E))
		{
			this.gameObject.SetActive(true);
		}
	}
}
