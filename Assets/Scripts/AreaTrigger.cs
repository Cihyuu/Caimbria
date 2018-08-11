using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour {

    public string collidingObjectTag;
    public GameObject objectToCall;
    public string methodToCall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(collidingObjectTag))
        { 
            objectToCall.SendMessage(methodToCall);
        }
    }
}
