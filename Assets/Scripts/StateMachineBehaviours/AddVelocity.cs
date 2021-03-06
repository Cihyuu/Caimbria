﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddVelocity : StateMachineBehaviour {

    public float life = 0.4f;
    public float force = 6;
    public Vector3 direction;
    [Space]
    [Header("This will override the direction")]
    public bool useTransformForward;
    public bool additive;
    public bool onEnter;
    public bool onExit;
    [Header("when Ending Applying velocity! Not anim state")]
    public bool onEndClampVelocity;

    StateManager states;
    HandleMovement_Player ply;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onEnter)
        {
            if (useTransformForward && !additive)
                direction = animator.transform.forward;

            if (useTransformForward && additive)
                direction += animator.transform.forward;

            if (states == null)
                states = animator.transform.GetComponent<StateManager>();

            if (!states.isPlayer)
                return;

            if (ply == null)
                ply = animator.transform.GetComponent<HandleMovement_Player>();

            ply.AddVelocity(direction, life, force, onEndClampVelocity);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (onExit)
        {
            if (useTransformForward && !additive)
                direction = animator.transform.forward;

            if (useTransformForward && additive)
                direction += animator.transform.forward;

            if (states == null)
                states = animator.transform.GetComponent<StateManager>();

            if (!states.isPlayer)
                return;

            if (ply == null)
                ply = animator.transform.GetComponent<HandleMovement_Player>();

            ply.AddVelocity(direction, life, force, onEndClampVelocity);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
