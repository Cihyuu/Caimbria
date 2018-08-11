using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour {

    public GameObject objectToSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            objectToSpawn.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}
