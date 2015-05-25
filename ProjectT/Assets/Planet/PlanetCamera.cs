using UnityEngine;
using System.Collections;

public class PlanetCamera : MonoBehaviour {

	public float MoveSpeed = 0.1f;
	public float zoom = 5;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var movement = new Vector3();

		zoom *= 1.0f + (Input.mouseScrollDelta.y * 0.1f);

		if(Input.GetKey(KeyCode.UpArrow)) {
			movement += Vector3.up * MoveSpeed;
		}
		if(Input.GetKey(KeyCode.LeftArrow)) {
			movement += Vector3.left * MoveSpeed;
		}
		if(Input.GetKey(KeyCode.RightArrow)) {
			movement += Vector3.right * MoveSpeed;
		}
		if(Input.GetKey(KeyCode.DownArrow)) {
			movement += Vector3.down * MoveSpeed;
		}

		// android
		movement.x += Input.acceleration.x;
		movement.y += Input.acceleration.y;

		transform.position = transform.position + movement * zoom * Time.deltaTime * 30f;

		GetComponent<Camera> ().orthographicSize = zoom;
	}
}
