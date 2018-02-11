using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

public static GameManager instance;

Animator doorAnim;
float animTime;
string animName = "door_open";
public List<GameObject> correctOrder = new List<GameObject>();
public List<GameObject> selectedOrder = new List<GameObject>();
bool statusChecked = false;
bool rightOrder = true;


    // to keep only one instance
    void Awake ()
	{
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
		}
	}
	// Use this for initialization
	void Start () {
		doorAnim = GameObject.FindWithTag("exit").GetComponent<Animator>();
	}

	public void OpenDoor ()
	{
		animTime = Mathf.Clamp01(doorAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //names for animations and animations time
        doorAnim.Play(animName, 0, animTime);
        //set float direction to 1
        doorAnim.SetFloat("direction", 1.0f);
	}

	public void CloseDoor ()
	{
		animTime = Mathf.Clamp01(doorAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        //names for animations and animations time
        doorAnim.Play(animName, 0, animTime);
        //set float direction to -1
        doorAnim.SetFloat("direction", -1.0f);
	}

	public IEnumerator DelayDoor (float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		OpenDoor();
	}

	public void AddLever (GameObject newLever)
	{
		if (selectedOrder.Contains (newLever)) {
			selectedOrder.Remove (newLever);
		} else {
			selectedOrder.Add (newLever);
		}
	}

	void CheckStatus ()
	{
		statusChecked = true;
		Debug.Log ("Lists are the same length");
		for (int i = 0; i < correctOrder.Count; i++) {
			if (correctOrder [i] != selectedOrder [i]) {
				rightOrder = false;
				StartCoroutine("WrongOrderSelection");
				return;
			}
		}

		if (rightOrder) {
			StartCoroutine("RightOrderSelection");
		}
	}

	IEnumerator WrongOrderSelection ()
	{
		yield return new WaitForSeconds (1f);

		foreach (GameObject lever in selectedOrder) {
		Animator anim = lever.GetComponent<Animator>();
		anim.SetTrigger("throw");
		}
		rightOrder = true;
		selectedOrder.Clear();
		statusChecked = false;
	}

	IEnumerator RightOrderSelection ()
	{
		yield return new WaitForSeconds (1f);
		OpenDoor ();
		foreach (GameObject lever in selectedOrder) {
		Destroy(lever.GetComponent<Collider>());
		Lever leverTrigger = lever.GetComponent<Lever>();
		leverTrigger.inTrigger = false;
		}
	}

	
	// Update is called once per frame
	void Update ()
	{
		if (correctOrder.Count > 0) {
			if (!statusChecked) {
				if (correctOrder.Count == selectedOrder.Count) {
					CheckStatus ();
				}
			}
		}
	}
}
