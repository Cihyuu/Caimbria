using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnpoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // only change the player position if player jumped scenes (scene didn't load additively)
        if (!OldGameManager.inst.isSeamlessSceneChange)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.position = transform.position;
            player.rotation = transform.rotation;
        }
	}
}
