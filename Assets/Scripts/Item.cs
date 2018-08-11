using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    bool inRangeOfPlayer = false;
    bool isPickedUp = false;
    PlayerController player;
    Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!inRangeOfPlayer) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPickedUp)
            {
                GetDropped();
            }
            else
            {
                GetPickedUp();
            }
        }
	}

    void GetPickedUp()
    {
        isPickedUp = true;
        rigidBody.useGravity = false;
        player.PickUpItem(transform);
    }

    void GetDropped()
    {
        isPickedUp = false;
        rigidBody.useGravity = true;
        player.DropItem(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Logger.Log("Press E");
            inRangeOfPlayer = true;
            player = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Logger.Log("");
            inRangeOfPlayer = false;
        }
    }
}
