using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour {

    [Header("Info")]
    public GameObject modelPrefab;
    public bool inGame;
    public bool isPlayer;

    [Header("Stats")]
    public float groundDistance = 0.6f;
    public float groundOffset = 0;
    public float distanceToCheckForward = 1.3f;
    public float runSpeed = 5;
    public float walkSpeed = 2;
    public float jumpForce = 4;
    public float airTimeThreshold = 0.8f;

    [Header("Inputs")]
    public float horizontal;
    public float vertical;
    public bool jumpInput;

    [Header("States")]
    public bool obstacleForward;
    public bool groundForward;
    public float groundAngle;

    [Header("State Requests")]
    public CharStates curState;
    public bool onGround;
    public bool run;
    public bool onLocomotion;
    public bool inAngle_MoveDir;
    public bool jumping;
    public bool canJump;

    GameObject activeModel;
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rBody;

    [HideInInspector]
    public Vector3 moveDirection;
    [HideInInspector]
    public Vector3 aimPosition;
    Transform aimHelper;
    float currentY;
    float currentZ;
    public float airTime;
    [HideInInspector]
    public bool prevGround;

    LayerMask ignoreLayers;

    public enum CharStates
    {
        idle,
        moving,
        inAir,
        hold
    }

    public void Init()
    {
        inGame = true;
        CreateModel();
        SetupAnimator();
        AddControllerReferences();
        canJump = true;

        gameObject.layer = 8;
        ignoreLayers = ~(1 << 3 | 1 << 8);
    }

    void CreateModel()
    {
        activeModel = Instantiate(modelPrefab) as GameObject;
        activeModel.transform.parent = this.transform;
        activeModel.transform.localPosition = Vector3.zero;
        activeModel.transform.localEulerAngles = Vector3.zero;
        activeModel.transform.localScale = Vector3.one;
    }

    void SetupAnimator()
    {
        anim = GetComponent<Animator>();
        Animator childAnim = activeModel.GetComponent<Animator>();
        anim.avatar = childAnim.avatar;
        Destroy(childAnim);
    }

    void AddControllerReferences()
    {
        gameObject.AddComponent<Rigidbody>();
        rBody = GetComponent<Rigidbody>();
        rBody = GetComponent<Rigidbody>();
        rBody.angularDrag = 999;
        rBody.drag = 4;
        rBody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
    }

    public void FixedTick()
    {
        obstacleForward = false;
        groundForward = false;
        onGround = OnGround();

        if (onGround)
        {
            Vector3 origin = transform.position;
            //Clear forward
            origin += Vector3.up * 0.75f;
            IsClear(origin, transform.forward, distanceToCheckForward, ref obstacleForward);
            if (!obstacleForward)
            {
                // is ground forward?
                origin += transform.forward * 0.6f;
                IsClear(origin, -Vector3.up, groundDistance * 3, ref groundForward);
            }
            else
            {
                // if we're facing a wall but want to move in a different direction, let us do that
                if (Vector3.Angle(transform.forward, moveDirection) > 30)
                {
                    obstacleForward = false;
                }
            }
        }

        UpdateState();
        MonitorAirTime();
    }

    public void RegularTick()
    {
        onGround = OnGround();
    }

    void UpdateState()
    {
        if (curState == CharStates.hold) return;

        if (horizontal != 0 || vertical != 0)
        {
            curState = CharStates.moving;
        }
        else
        {
            if (onGround)
            {
                curState = CharStates.idle;
            }
            else
            {
                curState = CharStates.inAir;
            }
        }

    }

    public bool OnGround()
    {
        if (curState == CharStates.hold)
        {
            return false;
        }

        Vector3 origin = transform.position + (Vector3.up * 0.55f);

        RaycastHit hit = new RaycastHit();
        bool isHit = false;
        FindGround(origin, ref hit, ref isHit);

        if (!isHit)
        {
            for (int i=0; i < 4; i++)
            {
                Vector3 newOrigin = origin;
                
                switch(i)
                {
                    case 0: //forward
                        newOrigin += Vector3.forward / 3;
                        break;
                    case 1: //backwards
                        newOrigin -= Vector3.forward / 3;
                        break;
                    case 2: // left
                        newOrigin -= Vector3.right / 3;
                        break;
                    case 3: // right
                        newOrigin += Vector3.right / 3;
                        break;
                }

                FindGround(newOrigin, ref hit, ref isHit);

                if (isHit)
                {
                    break;
                }
            }
        }

        if (!isHit)
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + groundOffset, transform.position.z);
        }

        return isHit;
    }

    void FindGround(Vector3 origin, ref RaycastHit hit, ref bool isHit)
    {
        Debug.DrawRay(origin, -Vector3.up * 0.55f, Color.red);
        if (Physics.Raycast(origin, -Vector3.up, out hit, groundDistance, ignoreLayers))
        {
            isHit = true;
        }
    }

    void IsClear(Vector3 origin, Vector3 direction, float distance, ref bool isHit)
    {
        RaycastHit hit;
        Debug.DrawRay(origin, direction * distance, Color.green);
        if (Physics.Raycast(origin, direction, out hit, distance, ignoreLayers))
        {
            isHit = true;
        }
        else
        {
            isHit = false;
        }

        if (obstacleForward)
        {
            Vector3 incomingVec = hit.point - origin;
            Vector3 reflectVet = Vector3.Reflect(incomingVec, hit.normal);
            float angle = Vector3.Angle(incomingVec, reflectVet);

            // if this obstacle is actually a hill we can walk up
            if (angle < 70)
            {
                obstacleForward = false;
            }
        }

        // to determine if we're on an incline
        if (groundForward)
        {
            if (horizontal != 0 || vertical != 0)
            {
                Vector3 p1 = transform.position;
                Vector3 p2 = hit.point;
                float diffY = p1.y - p2.y;
                groundAngle = diffY;
            }

            float targetIncline = 0;

            if (Mathf.Abs(groundAngle) > 0.3f)
            {
                if (groundAngle < 0)
                {
                    targetIncline = 1;
                }
                else
                {
                    targetIncline = -1;
                }
            }

            if (groundAngle == 0)
            {
                targetIncline = 0;
            }

            //anim.SetFloat(Statics.incline, targetIncline, 0.3f, Time.deltaTime);
        }
    }

    void MonitorAirTime()
    {
        // jump stuff
    }
}
