using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elk : MonoBehaviour, IEnemy {

    public Transform location1;
    public Transform location2;
    public Transform startChargingLocation;
    public GameObject portal;
    public GameObject door;
    NPCMovement movement;
    Transform player;
    int timesShotWhileCharging = 0;
    float haltTime = 2;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        movement = GetComponent<NPCMovement>();
        movement.MoveToTarget(startChargingLocation);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform == startChargingLocation && movement.GetTarget() == startChargingLocation)
        {
            Charge();
        }
        else if (hit.transform == location1 && movement.GetTarget() == location1)
        {
            movement.MoveToTarget(location2);
        }
        else if (hit.transform == location2 && movement.GetTarget() == location2)
        {
            if (timesShotWhileCharging > 1)
            {
                Invoke("Charge", haltTime);
            }
            else
            {
                Charge();
            }            
        }
    }

    void Charge()
    {
        movement.MoveToTarget(player);
    }

    public void Damaged()
    {
        if (movement.GetTarget() == player)
        {
            timesShotWhileCharging++;
            if (timesShotWhileCharging > 1)
            {
                movement.MoveToTarget(location2);
            }
            else
            {
                movement.MoveToTarget(location1);
            }
        }
    }

    public void Die()
    {
        portal.transform.position = new Vector3(transform.position.x, portal.transform.position.y, transform.position.z);
        portal.SetActive(true);
        door.SetActive(false);
        gameObject.SetActive(false);
    }
}
