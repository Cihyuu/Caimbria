using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OldGameManager : MonoBehaviour {

    public GameObject mainSceneCanvas;
    public GameObject mainSceneCamera;
    public bool isTesting;
    public bool isSeamlessSceneChange;
    // Approach flags
    public bool shotBall = false;


    public static OldGameManager inst = null;
    void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else if (inst != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        if (isTesting) return;
    }

    void Start()
    {
        Events.ShotBallEvent += PlayerShotBall;
    }

    void OnDisable()
    {
        Events.ShotBallEvent -= PlayerShotBall;
    }

    void PlayerShotBall()
    {
        shotBall = true;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SceneManager.LoadScene("Player", LoadSceneMode.Additive);
    }

    public void LoadSceneAdditively(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }

    public void LoadSceneGroup(string sceneGroup)
    {
        if (sceneGroup == "EmplacementApproach")
        {
            LoadSceneAdditively("EmplacementApproach");
        }
        else if (sceneGroup == "Clearing")
        {
            LoadSceneAdditively("Clearing");
        }
        else if (sceneGroup == "Path")
        {
            LoadSceneAdditively("Path");
        }
        LoadSceneAdditively("Player");
        mainSceneCanvas.SetActive(false);
        mainSceneCamera.SetActive(false);
    }
}
