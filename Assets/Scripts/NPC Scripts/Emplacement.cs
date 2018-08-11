using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emplacement : MonoBehaviour, IEnemy {

    [Header("Tweakable Variables")]
    public float maxVisionAngle = 45;
    public float maxVisionRange = 100;
    public float idleRotSpeed = 30;
    public float playerSeenRotSpeed = 50;
    public float fireRate = 1;
    public int damage = 5;

    [Space(10)]
    Transform player;
    public Transform lookpoint;
    Vector3 playerDir;
    public LayerMask playerLayer;
    BulletPool bulletPool;
    public Transform bulletSpawnpoint;
    bool isFiring = false;
    float fireTimer = 0;
    Coroutine fireRoutine = null;
    public Transform barrel;
    bool isDead = false;

    public GameObject approachTestCube;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bulletPool = GameObject.Find("BulletPool").GetComponent<BulletPool>();
        if (GameManager.inst.shotBall)
        {
            approachTestCube.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (isDead) return;

        if (CheckIfPlayerSeen())
        {
            print("player seen");
            RotateToPlayer();
            if (!isFiring)
            {
                isFiring = true;
                fireRoutine = StartCoroutine(Fire());
            }
        }
        else
        {
            if (isFiring)
            {
                isFiring = false;
                fireTimer = 0;
                StopCoroutine(fireRoutine);
            }
            transform.Rotate(Vector3.up, idleRotSpeed * Time.deltaTime);
        }
	}

    bool CheckIfPlayerSeen()
    {
        // calculate the angle at which the player is from my look direction
        // Vector3.up is added here to make the player position 1m higher, since the player object's centre position
        // is at his feet
        playerDir = player.position + Vector3.up - lookpoint.position;
        float angle = Vector3.Angle(transform.forward, playerDir);
        
        if (angle > maxVisionAngle) return false;
        // check if there is cover between me and the player
        RaycastHit hit;
        if (Physics.Raycast(lookpoint.position, playerDir, out hit, maxVisionRange))
        {
            if (playerLayer == (playerLayer | (1 << hit.transform.gameObject.layer)))
                return true;
        }
        return false;
    }

    void RotateToPlayer()
    {
        // rotate base toward player slowly
        Vector3 newDir = Vector3.RotateTowards(transform.forward, playerDir, playerSeenRotSpeed * Time.deltaTime, 0.0F);
        newDir.y = 0;
        transform.rotation = Quaternion.LookRotation(newDir);

        // rotate barrel fast
        Vector3 playerDirFromBarrel = player.position - barrel.position;
        Vector3 barrelDir = Vector3.RotateTowards(barrel.forward, playerDirFromBarrel, 50 * Time.deltaTime, 0.0F);
        barrel.localRotation = Quaternion.LookRotation(barrelDir);
        barrel.localEulerAngles = new Vector3(barrel.localEulerAngles.x, 0, 0);
    }

    IEnumerator Fire()
    {
        while (true)
        {
            if (fireTimer == 0)
            {
                LaunchBullet();
            }
            fireTimer += Time.deltaTime;
            if (fireTimer >= fireRate)
            {
                fireTimer = 0;
            }
            yield return null;
        }
    }

    void LaunchBullet()
    {
        Bullet bullet = bulletPool.GetBullet();
        bullet.Initialize(bulletSpawnpoint.position, damage, barrel.forward);
    }

    public void Damaged()
    {
        // TODO: behavior when damaged
    }

    public void Die()
    {
        isDead = true;
    }
}
