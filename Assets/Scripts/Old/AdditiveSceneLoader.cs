using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditiveSceneLoader : MonoBehaviour {

    public List<string> scenesToLoad;

	// Use this for initialization
	void Awake () {
		foreach (string name in scenesToLoad)
        {
            OldGameManager.inst.LoadSceneAdditively(name);
        }
	}
}
