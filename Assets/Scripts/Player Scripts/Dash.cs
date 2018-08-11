using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    [Header("Tweakable Variables")]
    public float dashCooldownDuration = 5;
    public float dashWindupDuration = 0.3f;
    public float dashSpeed = 100f;
    public float dashDuration = 0.5f;
    public float dashEndWindow = 1;

    float dashWindupTimer = 0;
    float dashTimer = 0;
    float dashCooldownTimer = 0;
    bool isDashCoolingDown = false;
    Coroutine dashCoroutine;

    PlayerController player;

    // Use this for initialization
    void Start () {
        player = GetComponent<PlayerController>();

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // can't dash if not moving
            if (!isDashCoolingDown && player.inputDir != Vector2.zero)
                StartCoroutine(DashWindup());
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (dashTimer > 0)
            {
                StopCoroutine(dashCoroutine);
                player.isDashing = false;
                StartCoroutine("DashCooldown");
                dashTimer = 0;
            }
        }
    }

    IEnumerator DashWindup()
    {
        player.isDashing = true;
        while (dashWindupTimer <= dashWindupDuration)
        {
            transform.Rotate(Vector3.right, -20 * Time.deltaTime);
            dashWindupTimer += Time.deltaTime;
            yield return null;
        }
        dashWindupTimer = 0;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        dashCoroutine = StartCoroutine(DoDash());
        yield break;
    }

    IEnumerator DoDash()
    {
        while (dashTimer <= dashDuration)
        {
            Vector3 worldCoordDir = new Vector3(player.inputDir.x, 0, player.inputDir.y);
            Vector3 dir = player.thirdPersonCamera.transform.TransformDirection(worldCoordDir);
            dir.y = 0;
            player.charController.Move(dir * dashSpeed * Time.deltaTime);
            dashTimer += Time.deltaTime;
            yield return null;
        }
        player.isDashing = false;
        player.hasDashJustEnded = true;
        Invoke("EndDash", dashEndWindow);
        StartCoroutine("DashCooldown");
        dashTimer = 0;
        yield break;
    }

    void EndDash()
    {
        player.hasDashJustEnded = false;
    }

    IEnumerator DashCooldown()
    {
        Logger.Log("Dash on cooldown");
        isDashCoolingDown = true;
        while (dashCooldownTimer < dashCooldownDuration)
        {
            dashCooldownTimer += Time.deltaTime;
            yield return null;
        }
        dashCooldownTimer = 0;
        isDashCoolingDown = false;
        Logger.Log("Dash ready");
        yield break;
    }

}
