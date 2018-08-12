using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {

    [Header("Tweakable Variables")]
    public float speed;
    public float speedSmoothTime = 0.05f;
    public float gravity = -15f;
    public int basicShotDamage = 2;
    public int counterShotDamage = 5;
    public int counterShotAfterDashDamage = 5;
    public int defaultWeakpointDamage = 2;
    public bool projectile = false;
    public float bulletSpeed = 20;

    [Space(10)]
    [Header("Component Flags")]
    public bool isDashing = false;
    public bool hasDashJustEnded = false;
    public bool isOppShot = false;
    public bool didOppAfterDash = false;

    [Space(10)]
    public bool isFalling = false;


    [Space(10)]
    public CharacterController charController;
    public Transform itemParent;

    float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public Vector2 inputDir = new Vector3(0, 0);
    
    float speedSmoothVelocity;
    float currentSpeed;
    public ThirdPersonCamera thirdPersonCamera;
    float velocityY = 0;
    protected bool isAiming = false;

    BulletPool bulletPool;
    public GameObject impactParticles;
    float bulletRange = 1000;
    AudioSource gunshotSfx;
    public GameObject bulletPrefab;
    public Transform bulletSpawnpoint;
    Opportunity oppComponent;
    
    float snapDistance = 1.5f;
    RaycastHit groundRaycastHit = new RaycastHit();
    Ray groundRay;
    bool isSliding = false;
    bool isKnockedBack = false;

    Animator anim;
    Vector3 prevVelocity = Vector3.zero;
    bool isStoppingRun = false;

    public static PlayerController inst = null;

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
    }

    // Use this for initialization
    private void Start () {
        thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        charController = GetComponent<CharacterController>();
        gunshotSfx = GetComponents<AudioSource>()[0];
        bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();
        oppComponent = GetComponent<Opportunity>(); // may be null if no Opportunity component attached
        charController.Move(Vector3.zero);
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	private void Update () {
        HandleMovement();
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            ToggleAim();
        }
        else if (Input.GetButtonUp("Fire2"))
        {
            ToggleAim();
        }
    }

    void HandleMovement()
    {
        if (isDashing || isSliding || isKnockedBack || isStoppingRun) return;

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;

        float targetSpeed = speed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        anim.SetFloat("Speed", currentSpeed);

        // gravity
        if (charController.isGrounded)
        {
            velocityY = 0;
        }
        else
        {
            velocityY += Time.deltaTime * gravity;
        }

        Vector3 velocity;

        //bool isTurning = false;

        if (isAiming)
        {
            // player should strafe left and right instead of rotating
            Vector3 dir = thirdPersonCamera.transform.TransformDirection(new Vector3(inputDir.x, 0, inputDir.y));
            dir.y = 0;
            velocity = dir * currentSpeed + Vector3.up * velocityY;

            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, 0f, transform.rotation.z));
        }
        else
        {
            if (inputDir != Vector2.zero)
            {
                // input rotates the player instead of strafing
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + thirdPersonCamera.transform.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);


                //if (targetRotation > transform.eulerAngles.y + 10 || targetRotation > transform.eulerAngles.y + 10)
                //{
                //    isTurning = true;
                //}
            }
            //if (isTurning)
            //{
            //    velocity = Vector3.zero;
            //}
            //else
            //{
            //    velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
            //}

            velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
            prevVelocity = velocity;
        }

        charController.Move(velocity * Time.deltaTime);

        if (!charController.isGrounded)
        {
            if (!isFalling)
                StartCoroutine(Fall());
        }
        else
        {
            if (!isFalling)
                isFalling = !isFalling;
        }
    }

    private IEnumerator Fall()
    {
        while (!charController.isGrounded)
        {
            groundRay = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(groundRay, out groundRaycastHit, snapDistance))
            {
                charController.Move(groundRaycastHit.point - transform.position);
            }
            yield return null;
        }

        yield break;
    }

    void ToggleAim()
    {
        if (!isAiming)
        {
            isAiming = true;
            thirdPersonCamera.ToggleAim(true);
        }
        else
        {
            isAiming = false;
            thirdPersonCamera.ToggleAim(false);
        }
    }

    void Shoot()
    {
        gunshotSfx.Play();
        if (projectile)
        {
            Bullet bullet = bulletPool.GetBullet();
            if (bullet == null) return;// should never happen after we implement ammo
            List<int> damages = CalculateDamage();
            int normalDamage = damages[0];
            int weakpointDamage = damages[1];
            bullet.SetWeakpointDamage(weakpointDamage);
            
            RaycastHit hit;
            if (Physics.Raycast(thirdPersonCamera.transform.position, thirdPersonCamera.transform.forward, out hit, bulletRange))
            {
                bullet.Initialize(bulletSpawnpoint.position, normalDamage, (hit.point - bulletSpawnpoint.position));
            }
            else // if there is no target, just shoot the bullet to maximum range in the direction of the centre of screen
            {
                // get position of bulletrange distance away from camera
                Vector3 farthestPoint = thirdPersonCamera.transform.position + thirdPersonCamera.transform.forward * bulletRange;
                bullet.Initialize(bulletSpawnpoint.position, normalDamage, (farthestPoint - bulletSpawnpoint.position));
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(thirdPersonCamera.transform.position, thirdPersonCamera.transform.forward, out hit, bulletRange))
            {
                Instantiate(impactParticles, hit.point, Quaternion.LookRotation(hit.normal));
                if (hit.transform.GetComponent<EnemyHealth>() != null)
                {
                    DamageEnemy(hit);
                }
            }
        }

    }

    // Use this for hitscan only
    void DamageEnemy(RaycastHit hit)
    {
        int damage = basicShotDamage; // basic shot
        int weakpointDamage = defaultWeakpointDamage;
        if (isOppShot) // if counter shot (aimed during opportunity)
        {
            damage += counterShotDamage;
            if (didOppAfterDash) // if counter shot within one second of dash end
            {
                damage += counterShotAfterDashDamage;
            }
            else
            {
                weakpointDamage /= 2;
            }
            if (oppComponent != null)
                oppComponent.StartOppCooldown();
        }
        if (hit.collider.CompareTag("WeakPoint"))
        {
            damage += weakpointDamage;
        }
        hit.transform.GetComponent<EnemyHealth>().TakeDamage(damage);
        print(damage + " damage done");
    }

    // use this for projectiles
    protected List<int> CalculateDamage()
    {
        int damage = basicShotDamage; // basic shot
        int weakpointDamage = defaultWeakpointDamage;

        if (isOppShot) // if counter shot (aimed during opportunity)
        {
            damage += counterShotDamage;
            if (didOppAfterDash) // if counter shot within one second of dash end
            {
                damage += counterShotAfterDashDamage;
            }
            else // bunt shot
            {
                weakpointDamage = defaultWeakpointDamage / 2;
            }
            if (oppComponent != null)
                oppComponent.StartOppCooldown();
        }
        List<int> result = new List<int>();
        result.Add(damage);
        result.Add(weakpointDamage);
        return result;
    }

    public void PickUpItem(Transform item)
    {
        item.parent = itemParent;
        item.localPosition = Vector3.zero;
    }

    public void DropItem(Transform item)
    {
        item.parent = null;
    }

    public void StartSlide(Vector3 direction, float slideSpeed)
    {
        isSliding = true;
        StartCoroutine(Slide(direction, slideSpeed));
    }

    public void EndSlide()
    {
        isSliding = false;
    }

    public void StopRun()
    {
        isStoppingRun = true;
    }

    public void ContinueRun()
    {
        isStoppingRun = false;
    }

    IEnumerator Slide(Vector3 direction, float slideSpeed)
    {
        while (isSliding)
        {
            print("blah");
            charController.Move(direction * slideSpeed * Time.deltaTime);

            if (!charController.isGrounded)
            {
                groundRay = new Ray(transform.position, Vector3.down);
                if (Physics.Raycast(groundRay, out groundRaycastHit, 10))
                {
                    charController.Move(groundRaycastHit.point - transform.position);
                }
            }

            yield return null;
        }
    }

    // direction of knockback will always be just backwards for now
    public void StartKnockback(float force)
    {
        isKnockedBack = true;
        StartCoroutine(Knockback(force));
    }

    IEnumerator Knockback(float force)
    {
        while (force > 0.5)
        {
            charController.Move(-transform.forward * force);
            force = Mathf.Lerp(force, 0, 5 * Time.deltaTime);
            yield return null;
        }
        isKnockedBack = false;
    }
}
