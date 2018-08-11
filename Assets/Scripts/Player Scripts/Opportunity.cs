using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Opportunity : MonoBehaviour {

    [Header("Tweakable Variables")]
    public float oppWindowDuration = 1;
    public float oppCooldownDuration = 5;
    public float slowmoFactor = 0.2f;
    public float slowmoDuration = 2;

    [Space(10)]
    bool isOppCoolingDown = false;
    float oppCooldownTimer = 0;
    AudioSource oppSfx;
    AudioSource oppAfterDashSfx;
    Coroutine oppWithoutAimTimer;
    
    PlayerController player;

    // Use this for initialization
    void Start () {
        player = GetComponent<PlayerController>();
        oppSfx = GetComponents<AudioSource>()[1];
        oppAfterDashSfx = GetComponents<AudioSource>()[2];
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(2))
        {
            if (!isOppCoolingDown)
            {
                ActivateOpp();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            ToggleAim(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ToggleAim(false);
        }
    }

    void ActivateOpp()
    {
        player.isOppShot = true;
        oppSfx.Play();
        if (player.hasDashJustEnded)
        {
            player.didOppAfterDash = true;
            oppAfterDashSfx.Play();
        }
        oppWithoutAimTimer = StartCoroutine(OppWithoutAimTimer());
    }

    IEnumerator OppWithoutAimTimer()
    {
        yield return new WaitForSeconds(oppWindowDuration);
        StartCoroutine(OppCooldown());
    }

    public void StartOppCooldown() // called from playerController
    {
        StartCoroutine(OppCooldown());
    }

    IEnumerator OppCooldown()
    {
        Logger.Log("Opportunity on cooldown");
        player.isOppShot = false;
        player.didOppAfterDash = false;
        Events.SendEvent.OppAimToggled(false);
        isOppCoolingDown = true;

        if (Time.timeScale == slowmoFactor)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

        while (oppCooldownTimer < oppCooldownDuration)
        {
            oppCooldownTimer += Time.deltaTime;
            yield return null;
        }
        oppCooldownTimer = 0;
        isOppCoolingDown = false;
        Logger.Log("Opportunity ready");
        yield break;
    }

    void ToggleAim(bool aim)
    {
        if (!player.isOppShot) return;

        if (aim)
        {
            StopCoroutine(oppWithoutAimTimer);
            oppSfx.Stop();
            Events.SendEvent.OppAimToggled(true);
            Time.timeScale = slowmoFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            StartCoroutine(SlowmoTimer());
        }
        else
        {
            StartCoroutine(OppCooldown());
        }
    }

    IEnumerator SlowmoTimer()
    {
        yield return new WaitForSeconds(slowmoDuration * slowmoFactor);
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
