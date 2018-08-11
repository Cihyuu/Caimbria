using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour {

    public int health = 50;
    [Space(10)]
    public Slider healthSlider;
    public Transform weakpoints;
    IEnemy enemyAI;

    void Start()
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
        enemyAI = GetComponent<IEnemy>();
    }

    void OnEnable()
    {
        Events.OppAimToggledEvent += OppAimToggled;
    }

    void OnDisable()
    {
        Events.OppAimToggledEvent -= OppAimToggled;
    }

    public void TakeDamage(int damage)
    {
        enemyAI.Damaged();
        health -= damage;
        healthSlider.value = health;
        print("Damage done: " + damage);

        if (health <= 0)
        {
            enemyAI.Die();
        }
    }

    private void OppAimToggled(bool on)
    {
        if (on)
        {
            DisplayWeakpoints();
        }
        else
        {
            HideWeakpoints();
        }
    }

    void DisplayWeakpoints()
    {
        if (weakpoints == null) return;
        foreach (Transform t in weakpoints)
        {
            t.GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void HideWeakpoints()
    {
        if (weakpoints == null) return;
        foreach (Transform t in weakpoints)
        {
            t.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void ToggleWeakpoints(bool makeVisible)
    {
        if (weakpoints == null) return;
        foreach (Transform t in weakpoints)
        {
            t.GetComponent<MeshRenderer>().enabled = makeVisible;
        }
    }
}
