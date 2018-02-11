using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]

public class MovingObject : MonoBehaviour {
Rigidbody rb;
AudioSource aSource;
public AudioClip movingSound;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		aSource = GetComponent<AudioSource>();
		aSource.clip = movingSound;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (rb.velocity.magnitude >= 0.1 && !aSource.isPlaying) {
			aSource.Play();
		}

	}
}
