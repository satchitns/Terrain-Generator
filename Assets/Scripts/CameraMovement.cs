using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	Vector3 mousePos;
	// Update is called once per frame
	public float velocity = 0.5f;
	public float turnRate = 0.5f;
	bool rotate = false;
	void Update()
	{
		transform.position += transform.forward * Input.GetAxis("Vertical") * velocity + transform.right * Input.GetAxis("Horizontal") * velocity;
		if (Input.GetKeyDown(KeyCode.F))
		{
			rotate = !rotate;
			mousePos = Input.mousePosition;
		}
		if (rotate)
		{
			Vector3 diff = mousePos - Input.mousePosition;
			mousePos = Input.mousePosition;
			transform.Rotate(diff.y * turnRate, -diff.x * turnRate, 0);
			Vector3 rot = transform.rotation.eulerAngles;
			transform.rotation = Quaternion.Euler(new Vector3(rot.x, rot.y, 0));
		}
	}
}
