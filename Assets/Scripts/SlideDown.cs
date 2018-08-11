using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideDown : MonoBehaviour {

    public float slideSpeed = 10;
    public LayerMask playerLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            // the hill's normal is Vector3.left
            PlayerController.inst.StartSlide(transform.up, slideSpeed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            PlayerController.inst.EndSlide();
        }
    }
}
