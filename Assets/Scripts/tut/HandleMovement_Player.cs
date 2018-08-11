using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMovement_Player : MonoBehaviour {

    StateManager states;
    Rigidbody rb;

    public bool doAngleCheck = true;
    [SerializeField]
    float degreesRunThreshold = 8;
    [SerializeField]
    bool usedot = true;

    bool overrideForce;
    bool inAngle;

    float rotateTimer_;
    float velocityChange = 4;
    bool applyJumpForce;

    Vector3 storeDirection;
    InputHandler inputHandler;

    Vector3 curVelocity;
    Vector3 targetVelocity;
    float prevAngle;
    Vector3 prevDir;

    Vector3 overrideDirection;
    float overrideSpeed;
    float forceOverrideTimer;
    float forceOverrideLife;
    bool stopVelocity;

    public void Init(StateManager st, InputHandler ih)
    {
        inputHandler = ih;
        states = st;
        rb = st.rBody;
        states.anim.applyRootMotion = false;
    }

    public void Tick()
    {
        if (!overrideForce)
        {
            HandleDrag();
            if (states.onLocomotion)
            {
                MovementNormal();
            }
        }
        else
        {
            states.horizontal = 0;
            states.vertical = 0;
            OverrideLogic();
        }
    }

    void MovementNormal()
    {
        inAngle = states.inAngle_MoveDir;

        Vector3 v = inputHandler.tpCamera.transform.forward * states.vertical;
        Vector3 h = inputHandler.tpCamera.transform.right * states.horizontal;

        v.y = 0;
        h.y = 0;

        if (states.onGround)
        {
            if (states.onLocomotion)
            {
                HandleRotation_Normal(h, v);
            }

            float targetSpeed = states.runSpeed;

            if (inAngle)
            {
                HandleVelocity_Normal(h, v, targetSpeed);
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }
    }

    void HandleVelocity_Normal(Vector3 h, Vector3 v, float speed)
    {
        Vector3 currVelocity = rb.velocity;

        if (states.horizontal != 0 || states.vertical != 0)
        {
            targetVelocity = (h + v).normalized * speed;
            velocityChange = 3;
        }
        else
        {
            velocityChange = 2;
            targetVelocity = Vector3.zero;
        }

        Vector3 vel = Vector3.Lerp(currVelocity, targetVelocity, velocityChange * Time.deltaTime);
        rb.velocity = vel;

        if (states.obstacleForward)
        {
            rb.velocity = Vector3.zero;
        }
    }

    void HandleRotation_Normal(Vector3 h, Vector3 v)
    {
        if (states.horizontal != 0 || states.vertical != 0)
        {
            storeDirection = (v + h).normalized;

            float targetAngle = Mathf.Atan2(storeDirection.x, storeDirection.z) * Mathf.Rad2Deg;

            if (states.run && doAngleCheck)
            {
                if (!usedot)
                {
                    if ((Mathf.Abs(prevAngle - targetAngle)) > degreesRunThreshold)
                    {
                        prevAngle = targetAngle;
                        PlayAnimSpecial(AnimSpecials.runToStop, false);
                        return;
                    }
                }
                else
                {
                    float dot = Vector3.Dot(prevDir, states.moveDirection);
                    if (dot < 0)
                    {
                        prevDir = states.moveDirection;
                        PlayAnimSpecial(AnimSpecials.runToStop, false);
                        return;
                    }
                }
            }

            prevDir = states.moveDirection;
            prevAngle = targetAngle;

            storeDirection += transform.position; // is this necessary??
            Vector3 targetDir = (storeDirection - transform.position).normalized;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
            {
                targetDir = transform.forward;
            }
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, velocityChange * Time.deltaTime);
        }
    }

    void HandleAnimations_Normal()
    {
        Vector3 relativeDirection = transform.InverseTransformDirection(states.moveDirection);

        float h = relativeDirection.x;
        float v = relativeDirection.z;

        if (states.obstacleForward)
        {
            v = 0;
        }

        states.anim.SetFloat(Statics.vertical, v, 0.2f, Time.deltaTime);
        states.anim.SetFloat(Statics.horizontal, h, 0.2f, Time.deltaTime);
    }

    void HandleDrag()
    {
        if (states.horizontal != 0 || states.vertical != 0 || states.onGround == false)
        {
            rb.drag = 0;
        }
        else
        {
            rb.drag = 4; // so you don't slide around
        }
    }

    public void PlayAnimSpecial(AnimSpecials t, bool sptrue = true)
    {
        int n = Statics.GetAnimSpecialType(t);
        states.anim.SetBool(Statics.special, sptrue);
        states.anim.SetInteger(Statics.specialType, n);
        StartCoroutine(CloseSpecialOnAnim(0.4f));
    }

    IEnumerator CloseSpecialOnAnim(float t)
    {
        yield return new WaitForSeconds(t);
        states.anim.SetBool(Statics.special, false);
    }

    // if you want to forcibly apply velocity
    public void AddVelocity(Vector3 direction, float t, float force, bool clamp)
    {
        forceOverrideLife = t;
        overrideSpeed = force;
        overrideForce = true;
        forceOverrideTimer = 0;
        overrideDirection = direction;
        rb.velocity = Vector3.zero;
        stopVelocity = clamp;
    }

    void OverrideLogic()
    {
        rb.drag = 0;
        rb.velocity = overrideDirection * overrideSpeed;

        forceOverrideTimer += Time.deltaTime;
        if (forceOverrideTimer > forceOverrideLife)
        {
            if (stopVelocity)
            {
                if (stopVelocity)
                {
                    rb.velocity = Vector3.zero;
                }

                stopVelocity = false;
                overrideForce = false;
            }
        }
    }


}
