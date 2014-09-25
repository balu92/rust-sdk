using UnityEngine;
using System.Collections;

public class DevCamera : MonoBehaviour
{
	public float movementScale = 1.0f;

	Vector3 view = new Vector3();
	Vector3 velocity = new Vector3();
	float zoom = 0.5f;

	void Start ()
	{
		transform.position = Camera.main.transform.position;
		view = Camera.main.transform.rotation.eulerAngles;
	}

	void Update ()
	{
		if ( Input.GetButton( "FIRE_SECONDARY" ) )
		{
			DoFreeMovement();

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}

	void DoFreeMovement()
	{
		if ( Input.GetButton( "FIRE_PRIMARY" ) )
		{
			zoom = Mathf.Clamp( zoom + Input.GetAxis( "Mouse Y" ) * -0.033f, 0.01f, 1.0f );
		}
		else if ( Input.GetButton( "DUCK" ) )
		{
			view.z += Input.GetAxis( "Mouse X" ) * 1.5f;
		}
		else
		{
			view.y += Input.GetAxis( "Mouse X" ) * 3.0f * zoom;
			view.x -= Input.GetAxis( "Mouse Y" ) * 3.0f * zoom;
		}

		transform.rotation = Quaternion.Euler( view );

		Vector3 vMove = Vector3.zero;
		if ( Input.GetButton( "FORWARD" ) ) vMove += Vector3.forward;
		if ( Input.GetButton( "BACKWARD" ) ) vMove += Vector3.back;
		if ( Input.GetButton( "LEFT" ) ) vMove += Vector3.left;
		if ( Input.GetButton( "RIGHT" ) ) vMove += Vector3.right;

		vMove.Normalize();

		if ( Input.GetButton( "SPRINT" ) )
			vMove *= 3.0f;

		velocity += transform.rotation * vMove * 0.05f;
		velocity = Vector3.Lerp( velocity, Vector3.zero, 0.2f );

		transform.position += velocity * movementScale;

		Camera.main.transform.position = transform.position;
		Camera.main.transform.rotation = transform.rotation;

		Camera.main.fieldOfView = Mathf.Lerp( 1.0f, 120.0f, zoom );
	}

}
