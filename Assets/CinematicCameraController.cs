using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMovementType {
	TURN, MOVE, BOTH
}

[System.Serializable]
public class CameraMovement {

	public CameraMovementType movementType; // The type of movement for the camera
	public float speed; // Time it takes to complete the movement

	[Header("For rotation")]
	public Vector3 endRotationEuler; // Amount to turn on x, y, z for euler angles
	[Header("For movement")]
	public Vector3 endPosition; // The end position for movement

	[HideInInspector]
	public bool completed = false;

	public CameraMovement()
	{

	}
}

[RequireComponent(typeof(Camera))]
public class CinematicCameraController : MonoBehaviour {

	[HideInInspector]
	public Camera m_camera;

	public bool beginOnStartup = false;

	public CameraMovement[] cameraMovements;

	private bool completed;

	void Start () {
		m_camera = GetComponent<Camera>();

		if (beginOnStartup)
		{
			if (cameraMovements.Length == 0)
			{
				Debug.LogWarning("[CinematicCamera] The camera movements are empty for camera " + m_camera.name + " !");
				completed = true;
				return;
			}
			completed = false;
			BeginMovement();
		}
	}
	
	void Update () {
		
	}


	public void BeginMovement()
	{
		StartCoroutine(BeginMovementTask());
	}

	private IEnumerator BeginMovementTask()
	{
		for (int i = 0; i < cameraMovements.Length; i++)
		{
			var instructions = cameraMovements[i];
			while (!instructions.completed)
			{
				if (instructions.movementType == CameraMovementType.MOVE || instructions.movementType == CameraMovementType.BOTH)
				{
					m_camera.transform.position = Vector3.Slerp(m_camera.transform.position, instructions.endPosition, instructions.speed * Time.deltaTime);
				}

				if (instructions.movementType == CameraMovementType.TURN || instructions.movementType == CameraMovementType.BOTH)
				{
					m_camera.transform.rotation = Quaternion.Slerp(m_camera.transform.rotation, Quaternion.Euler(instructions.endRotationEuler), instructions.speed * Time.deltaTime);
				}

				if (instructions.movementType == CameraMovementType.MOVE && Vector3.Distance(m_camera.transform.position, instructions.endPosition) <= 0.4f)
				{
					instructions.completed = true;
				} else if (instructions.movementType == CameraMovementType.TURN && m_camera.transform.rotation == Quaternion.Euler(instructions.endRotationEuler))
				{
					instructions.completed = true;
				} else if (instructions.movementType == CameraMovementType.BOTH && m_camera.transform.rotation == Quaternion.Euler(instructions.endRotationEuler) && Vector3.Distance(m_camera.transform.position, instructions.endPosition) <= 0.4f)
				{
					instructions.completed = true;
				}

				yield return null;
			}
		}
	}
}
