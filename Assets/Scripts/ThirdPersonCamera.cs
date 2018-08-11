using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    public bool usingGamepad;
    public Transform lookTarget;
    public bool lockCursor = true;
    public Vector2 pitchMinMax = new Vector2(-30, 70);
    public float dstFromTarget = 6;
    public float mouseSensitivity = 6;
    public float gamepadSensitivity = 10;
    public Vector3 normalOffset = new Vector3(0, 1, -10);
    //public float rotSmoothTime = 0.12f;
    //Vector3 rotSmoothVelocity;
    float yaw;
    float pitch;
    Transform player;

    public GameObject crosshair;
    public Vector3 aimingOffset = new Vector3(2, 1, -4);
    //public float aimSmoothTime = 0.2f;
    //Vector3 aimSmoothVelocity;
    public bool isAiming = false;
    Vector3 currentRot;
    Vector3 targetPos;
    

    private void Start()
    {
        if (lockCursor && !usingGamepad)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        MoveCamera();
    }

    public void ToggleAim(bool aim)
    {
        if (aim)
        {
            isAiming = true;
            crosshair.SetActive(true);
        }
        else
        {
            if (isAiming)
            {
                // revert player's x-rotation back to 0
                player.eulerAngles = Vector3.Lerp(player.eulerAngles, new Vector3(0, player.eulerAngles.y, player.eulerAngles.z), 1f);
            }
            crosshair.SetActive(false);
            isAiming = false;
        }
    }

    void MoveCamera()
    {
        if (usingGamepad)
        {
            yaw += Input.GetAxis("Joystick X") * gamepadSensitivity;
            pitch -= Input.GetAxis("Joystick Y") * gamepadSensitivity;
        }
        else
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y); // so that the camera doesn't go upside down
        //currentRot = Vector3.SmoothDamp(currentRot, new Vector2(pitch, yaw), ref rotSmoothVelocity, rotSmoothTime);
        transform.eulerAngles = new Vector2(pitch, yaw);
        
        if (isAiming)
        {
            targetPos = lookTarget.position
                + transform.forward * aimingOffset.z
                + transform.right * aimingOffset.x
                + transform.up * aimingOffset.y;

            player.rotation = Quaternion.Lerp(player.rotation, transform.rotation, 1f);
            
        }
        else
        {
            targetPos = lookTarget.position + transform.forward * normalOffset.z + transform.up * normalOffset.y;
        }
        //currentPos = Vector3.SmoothDamp(currentPos, targetPos, ref aimSmoothVelocity, aimSmoothTime);
        transform.position = targetPos;
    }
}
