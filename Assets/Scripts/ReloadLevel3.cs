using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadLevel3 : MonoBehaviour {
	public bool makeObjectAppear;
	public ObjectAppear2 objectAppear2;
//	public AudioClip appearingSound;
//	private AudioSource soundSource;

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			objectAppear2.MakeObjAppear();
		}
		
	}
}
