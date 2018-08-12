using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour {

    [Header("Dash Variables")]
    public float DashDuration = 1.0f; // Time (in seconds) it takes to complete the dash movement
    public float DashCooldown = 1.0f; //Time (in seconds) it takes to cool down after dash movement
    public float DashMaxSpeed = 1.0f; //Maximum speed of the dashing player where DashCurve's y = 1.
    public float DashEndWindow = 1.0f; //Time after dashing player is in standstill.

    public bool IsDashing = false; // Is the player dashing?
    public bool CanDash = true; // Is the player able to dash? (Not on a cooldown)


    public AnimationCurve DashCurve = new AnimationCurve(); // DashCurve is the speed (y axis) over time (x axis) of the player while dashing. The bouds of the x axis are [0, 1] and for y are (0, 1]. y = 1 means dash is at maximum speed.

    public PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Dash"))
        {
            if (CanDash && player.inputDir != Vector2.zero)
            {
                StartCoroutine(PerformDash());
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (IsDashing)
            {
                StopCoroutine(PerformDash());
                StartCoroutine(PerformCooldown());
                player.isDashing = false;
                IsDashing = false;
            }
        }
    }

    public IEnumerator PerformDash()
    {

        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        player.isDashing = true;
        IsDashing = true;
        CanDash = false;

        var elapsedTime = 0f;

        while (elapsedTime <= DashDuration)
        {

            elapsedTime += Time.deltaTime;

            var currentSpeed = DashCurve.Evaluate(elapsedTime / DashDuration);

            var worldCoordDir = new Vector3(player.inputDir.x, 0, player.inputDir.y);

            var dir = player.thirdPersonCamera.transform.TransformDirection(worldCoordDir);
            dir.y = 0;

            player.charController.Move(dir * currentSpeed * DashMaxSpeed * Time.deltaTime);

            yield return null;
        }

        IsDashing = false;
        player.isDashing = false;
        player.hasDashJustEnded = true;
        
        StartCoroutine(PerformCooldown());
    }

    public IEnumerator EndDash()
    {
        yield return new WaitForSeconds(DashEndWindow);
        player.hasDashJustEnded = false;
    }

    public IEnumerator PerformCooldown()
    {
        if (IsDashing)
            yield break;

        yield return new WaitForSeconds(DashCooldown);

        CanDash = true;
    }
}
