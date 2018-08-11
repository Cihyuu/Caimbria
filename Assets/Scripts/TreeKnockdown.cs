using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeKnockdown : MonoBehaviour {

    BoxCollider triggerCollider;
    bool knockedDown = false;

	// Use this for initialization
	void Start () {
        triggerCollider = GetComponents<BoxCollider>()[1];
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Invoke("Knockdown", 1);
        }
    }
    

    // this is the collider at the end of the bridge
    void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player"))
        {
            if (knockedDown)
            {
                gameObject.SetActive(false);
            }
        }
	}

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Logger.Log("Press E to knock down tree");
            if (Input.GetKeyDown(KeyCode.E))
            {
                Knockdown();
            }
        }
    }

    void Knockdown()
    {
        GetComponent<Animator>().enabled = true;
        knockedDown = true;
        triggerCollider.enabled = false;
        Logger.Log("");
    }
}
