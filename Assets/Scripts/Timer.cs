using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    float timeRemaining = 600.0f;

    void Update () {
        timeRemaining -= Time.deltaTime;
    }
    
    void OnGUI(){
        GUI.contentColor = Color.red;
        if(timeRemaining > 0){
            GUI.Label(new Rect(100, 100, 200, 100), 
                         "Time Remaining : "+(int)timeRemaining);
        }
        else{
            GUI.Label(new Rect(100, 100, 200, 100), "Time's Up");
        }
    }
}
