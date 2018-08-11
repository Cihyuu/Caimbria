using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineconeManager : MonoBehaviour {

    public int numPinecones = 5;
    public float timer = 10;
    private bool hasTimerStarted = false;

    public void PineconeShot()
    {
        if (!hasTimerStarted)
        {
            StartCoroutine(PineconeTimer());
        }
        numPinecones--;

        if (numPinecones == 0 && timer > 0)
        {
            StopAllCoroutines();
            Logger.Log("pinecone effect");
        }
    }

    IEnumerator PineconeTimer()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
    }
}
