using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredHazard : MonoBehaviour {

    public float knockbackForce = 3;
    public LayerMask playerLayer;
    private Animator anim;
    private bool hasTriggered = false;
    private Collider body;
    private Collider trigger;
    

    private void Start()
    {
        anim = GetComponent<Animator>();
        body = GetComponents<Collider>()[0];
        trigger = GetComponents<Collider>()[1];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            if (!hasTriggered)
            {
                trigger.enabled = false;
                anim.enabled = true;
                hasTriggered = true;
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                PlayerController.inst.StartKnockback(knockbackForce);
            }
        }
    }

    // to be called by an animation event at the end of the hazard's animation
    public void AnimationEnded()
    {
        body.isTrigger = false;
    }
}
