using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShootTrigger : MonoBehaviour {

	void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Events.SendEvent.ShotBall();
        }
    }
}
