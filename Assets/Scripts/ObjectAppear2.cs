using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAppear2 : MonoBehaviour {
public GameObject appearingObj;
public AudioClip appearingSound;
private AudioSource soundSource;

	// Use this for initialization
	void Start () {
		appearingObj.SetActive(true);
		soundSource = appearingObj.GetComponent<AudioSource>();
		soundSource.clip = appearingSound;
	}

	public void MakeObjAppear ()
	{
		appearingObj.SetActive(true);
		soundSource.Play();
	}

	public void MakeObjectDisappear ()
	{
		appearingObj.SetActive(false);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
