using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour {
GameObject[] arrowSpawns;
public GameObject arrow;
int arrowNum;
public bool timerBased;
public float fireTimer = 3f;
private float timer = 0f;


	// Use this for initialization
	void Start () {
		arrowSpawns = GameObject.FindGameObjectsWithTag("arrowSpawn");
	}

	void OnTriggerEnter (Collider other)
	{
		if (!timerBased) {
			if (other.tag == "Player") {
				FireArrows ();
				}
			}
		}

	public void FireArrows ()
	{
		arrowNum = Random.Range (1, arrowSpawns.Length);
		for (int i = 0; i < arrowNum; i++) {
				GameObject arrowPos = arrowSpawns[Random.Range(0, arrowSpawns.Length)];
				Instantiate (arrow, arrowPos.transform.position, arrowPos.transform.rotation);
				}

	}


	// Update is called once per frame
	void Update ()
	{
		if (timerBased) {
			if (timer <= fireTimer) {
				timer += Time.deltaTime;
			} else {
				FireArrows();
				timer = 0f;
			}
		}
	}
}