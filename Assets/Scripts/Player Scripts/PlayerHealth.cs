using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    int health = 100;
    int stage = 4;
    public float regenSpeed = 1;
    float floatTimer = 0;
    bool isDead;

    public Text healthText;

	void Update()
    {
        if (isDead) return;

        if (health % 25 != 0)
        {
            RegenHealth();
        }
    }

    void RegenHealth()
    {
        floatTimer += regenSpeed * Time.deltaTime;
        if (floatTimer >= 1)
        {
            health++;
            healthText.text = health.ToString();
            floatTimer = 0;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthText.text = health.ToString();
        floatTimer = 0;
        if (health > 75) stage = 4;
        else if (health > 50) stage = 3;
        else if (health > 25) stage = 2;
        else if (health > 0) stage = 1;
        else Die();
    }

    void Die()
    {
        isDead = true;
        print("YOU DIED");
    }
}
