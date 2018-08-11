using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {

    StateManager states;
    HandleMovement_Player hMove;

    float horizontal;
    float vertical;

    public ThirdPersonCamera tpCamera;

	// Use this for initialization
	void Start () {
        tpCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        gameObject.AddComponent<HandleMovement_Player>();

        states = GetComponent<StateManager>();
        hMove = GetComponent<HandleMovement_Player>();

        states.isPlayer = true;
        states.Init();
        hMove.Init(states, this);

        FixPlayerMeshes();
	}

    void FixPlayerMeshes()
    {
        SkinnedMeshRenderer[] skinned = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i=0; i<skinned.Length;i++)
        {
            skinned[i].updateWhenOffscreen = true;
        }
    }

    private void FixedUpdate()
    {
        states.FixedTick();
        UpdateStatesFromInput();
        hMove.Tick();
    }

    private void Update()
    {
        states.RegularTick();
    }

    void UpdateStatesFromInput()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        Vector3 v = tpCamera.transform.forward * vertical;
        Vector3 h = tpCamera.transform.right * horizontal;

        v.y = 0;
        h.y = 0;

        states.horizontal = horizontal;
        states.vertical = vertical;

        Vector3 moveDir = (h + v).normalized;
        states.moveDirection = moveDir;
        states.inAngle_MoveDir = InAngle(states.moveDirection, 25);
        if (states.run && horizontal != 0 || states.run && vertical != 0)
        {
            states.inAngle_MoveDir = true;
        }

        states.onLocomotion = states.anim.GetBool(Statics.onLocomotion);
    }

    bool InAngle(Vector3 targetDir, float angleThreshold)
    {
        float angle = Vector3.Angle(transform.forward, targetDir);
        return angle < angleThreshold;
    }
}
