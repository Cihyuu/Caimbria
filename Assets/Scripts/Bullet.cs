using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // TODO: make this a subclass for infantry bullet or something
    public GameObject impactParticles;
    public LayerMask enemyLayer;
    public LayerMask playerLayer;
    private float bulletSpeed = 800;
    private float lifetime = 2;
    private int weakpointDamage;
    private Rigidbody rigidBody;
    private int damage;

    public void SetWeakpointDamage(int damage)
    {
        weakpointDamage = damage;
    }

    private void OnEnable()
    {
        Invoke("Recycle", lifetime);
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Recycle()
    {
        gameObject.SetActive(false);
    }

    public void Initialize(Vector3 position, int damage, Vector3 direction)
    {
        transform.position = position;
        this.damage = damage;
        rigidBody.velocity = direction.normalized * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(impactParticles, transform.position, Quaternion.identity);
        // if this hit the enemy layer
        if (enemyLayer == (enemyLayer | (1 << collision.collider.gameObject.layer)))
        {
            if (collision.collider.CompareTag("Weakpoint"))
            {
                damage += weakpointDamage;
                // a weakpoint is a grandchild of the enemy object
                collision.collider.transform.parent.parent.GetComponent<EnemyHealth>().TakeDamage(damage);
            }
            else
            {
                EnemyHealth enemyHealth = FindEnemyHealthComponent(collision.transform);
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(damage);
            }
        }
        if (playerLayer == (playerLayer | (1 << collision.collider.gameObject.layer)))
        {
            PlayerHealth playerHealth = FindPlayerHealthComponent(collision.transform);
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
        gameObject.SetActive(false);
    }

    EnemyHealth FindEnemyHealthComponent(Transform t)
    {
        if (t.GetComponent<EnemyHealth>() != null)
        {
            return t.GetComponent<EnemyHealth>();
        }
        else if (t.parent != null) {
            return FindEnemyHealthComponent(t.parent);
        }
        else
        {
            print("the target has the enemy layer but no enemy component found");
            return null;
        }
    }
    // TODO: refactor - redundant
    PlayerHealth FindPlayerHealthComponent(Transform t)
    {
        if (t.GetComponent<PlayerHealth>() != null)
        {
            return t.GetComponent<PlayerHealth>();
        }
        else if (t.parent != null)
        {
            return FindPlayerHealthComponent(t.parent);
        }
        else
        {
            print("the target has the player layer but no player component found");
            return null;
        }
    }



}
