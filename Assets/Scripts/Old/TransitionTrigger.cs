using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionTrigger : MonoBehaviour {

    public string sceneToUnload;
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        OldGameManager.inst.isSeamlessSceneChange = true;
        if (SceneManager.GetSceneByName(sceneToUnload).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
        if (!SceneManager.GetSceneByName(sceneToLoad).isLoaded)
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
        }
    }
}
