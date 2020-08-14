using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private PlayerController camFocuser;

	private Vector3 lastFocusPosition;
	private float distanceToMove;

	// Start is called before the first frame update
	void Start()
	{
		camFocuser = PlayerController.instance;
		lastFocusPosition = camFocuser.transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		distanceToMove = camFocuser.transform.position.y - lastFocusPosition.y;

		transform.position = new Vector3(transform.position.x, transform.position.y + distanceToMove, transform.position.z);
		lastFocusPosition = camFocuser.transform.position;
	}
}
