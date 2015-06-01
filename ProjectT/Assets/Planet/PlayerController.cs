using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float maxSpeed = 5.0f;

	Animator anim;

	Vector3 lastSpeed;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		lastSpeed = transform.position;
	}

	void Update () {
	
		float moveX = 0.0f;
		float moveY = 0.0f;

		moveY += (Input.GetKey(KeyCode.W)) ? 1 : 0;
		moveY -= (Input.GetKey(KeyCode.S)) ? 1 : 0;

		moveX += (Input.GetKey(KeyCode.D)) ? 1 : 0;
		moveX -= (Input.GetKey(KeyCode.A)) ? 1 : 0;

		var moveSpeed = new Vector3 (moveX, moveY) * maxSpeed;

		var moveDelta = Vector3.Lerp (lastSpeed, moveSpeed, Time.deltaTime * 8);

		lastSpeed = moveDelta;

		anim.SetFloat ("speed", moveDelta.magnitude);

		transform.position = transform.position + (moveDelta * Time.deltaTime);

		if (moveDelta.magnitude > 0.1f) {
			transform.eulerAngles = new Vector3 (
				transform.eulerAngles.x,
				transform.eulerAngles.y,
				(moveDelta.x > 0) ? -Vector3.Angle(moveDelta, Vector3.up) : Vector3.Angle(moveDelta, Vector3.up)
			);
		}
	}
}
