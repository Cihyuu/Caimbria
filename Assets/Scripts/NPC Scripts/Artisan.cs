using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artisan : MonoBehaviour {

    public Transform startPos;
    public AudioClip[] audioClips;
    NPCMovement movement;
    Transform player;
    float interactionDistance = 15;
    AudioSource audioSource;
    bool isWarningPlayer = false;
    bool hasGreetedPlayer = false;

    void Start()
    {
        movement = GetComponentInChildren<NPCMovement>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasGreetedPlayer)
        {
            movement.MoveToTarget(player);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if player moves away from me while I'm greeting them
            if (audioSource.isPlaying && audioSource.clip != audioClips[1])
            {
                CancelInvoke();
                PlayVoiceLine(1);
                Invoke("MoveToStartPos", audioClips[1].length);
            }
        }
    }

    private void Update()
    {
        if (movement.IsMoving() && movement.GetTarget() == player)
        {
            float distanceToPlayer = (player.position - transform.position).sqrMagnitude;
            if (distanceToPlayer <= interactionDistance)
            {
                movement.StopMoving();
                if (isWarningPlayer)
                {
                    isWarningPlayer = false;
                    PlayVoiceLine(2);
                    Invoke("MoveToStartPos", audioClips[2].length);
                }
                else //is greeting player
                {
                    PlayVoiceLine(0);
                    Invoke("FinishGreetingPlayer", audioClips[0].length);
                }
            }
        }
    }

    private void MoveToStartPos()
    {
        movement.MoveToTarget(startPos);
    }
    
    private void FinishGreetingPlayer()
    {
        hasGreetedPlayer = true;
        MoveToStartPos();
    }

    private void PlayVoiceLine(int clip)
    {
        audioSource.clip = audioClips[clip];
        audioSource.Play();
    }

    public void WarnPlayer()
    {
        if (!hasGreetedPlayer)
        {
            CancelInvoke();
            isWarningPlayer = true;
            movement.MoveToTarget(player);
        }
    }
}
