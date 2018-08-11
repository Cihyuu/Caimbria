using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    // Approach flags
    public bool shotBall = false;


    public static GameManager inst = null;
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
}
