using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    public Transform destination;
    public float teleportDelay = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("Teleport", teleportDelay);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke();
        }
    }

    void Teleport()
    {
        PlayerController.inst.transform.parent.position = destination.position;
    }
}
