using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Teleporter : MonoBehaviour {

public AudioClip teleportDepart;
public AudioClip teleportArrive;
AudioSource soundSource;
public GameObject destination;
public float teleportTime = 0.5f;
public ParticleSystem destinationParticle;

	// Use this for initialization
	void Start () {
		soundSource = GetComponent<AudioSource>();
	}

	void OnTriggerEnter (Collider other)
	{
		StartCoroutine("Teleport", other.gameObject);
		soundSource.clip = teleportDepart;
		soundSource.Play();
	}
	
	IEnumerator Teleport (GameObject teleported)
	{
		teleported.SetActive(false);
		destinationParticle.Play();
		yield return new WaitForSeconds(teleportTime);
		soundSource.clip = teleportArrive;
		soundSource.Play();
		teleported.transform.position = destination.transform.position;
		teleported.SetActive(true);
	}
}
