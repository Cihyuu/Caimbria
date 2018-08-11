using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logger : MonoBehaviour {

    static Text logText;

	// Use this for initialization
	void Start () {
        logText = GetComponent<Text>();
	}
	
	public static void Log(string text)
    {
        logText.text = text;
    }
}
