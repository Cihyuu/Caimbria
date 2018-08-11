using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour {

    public float speed = 10;
    public float speedSmoothTime = 0.05f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -15f;
    bool isMoving = false;
    Transform target;
    float speedSmoothVelocity;
    float turnSmoothVelocity;
    float currentSpeed;
    CharacterController charController;
    float velocityY = 0;

    // Use this for initialization
    void Start () {
        charController = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {

        if (!isMoving) return;
        if ((transform.position - target.position).sqrMagnitude < 1)
        {
            isMoving = false;
        }
        
        currentSpeed = Mathf.SmoothDamp(currentSpeed, speed, ref speedSmoothVelocity, speedSmoothTime);

        // gravity
        if (charController.isGrounded)
        {
            velocityY = 0;
        }
        else
        {
            velocityY += Time.deltaTime * gravity;
        }
        Vector3 direction = (target.position - transform.position).normalized;
        //direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
        charController.Move(velocity * Time.deltaTime);
    }

    public Transform GetTarget()
    {
        return target;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void MoveToTarget(Transform t)
    {
        isMoving = true;
        target = t;
    }

    public void StopMoving()
    {
        isMoving = false;
    }
}
