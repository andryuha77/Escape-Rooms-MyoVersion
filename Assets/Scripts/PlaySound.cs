using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//if script added to object it will look for audio file
//or add if it not exist
[RequireComponent(typeof(AudioSource))]
public class PlaySound : MonoBehaviour {

//make possible to add sound from inspector window 
public void PlaySoundEffect (AudioClip assetSound)
	{
    //declare  asset sound
	AudioSource assetSource = GetComponent<AudioSource>();
    //load audio clip
	assetSource.clip = assetSound;
    //plays sound
	assetSource.Play();
	}

}
