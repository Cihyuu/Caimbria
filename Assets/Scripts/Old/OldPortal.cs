using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPortal : MonoBehaviour {

    public string sceneName;
    float sceneChangeDelay = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("ChangeScene", sceneChangeDelay);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke();
        }
    }

    void ChangeScene()
    {
        OldGameManager.inst.isSeamlessSceneChange = false;
        OldGameManager.inst.LoadScene(sceneName);
    }
}
