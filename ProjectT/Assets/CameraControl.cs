using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	public float MoveSpeed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var movement = new Vector3();

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

		movement.x += Input.acceleration.x;
		movement.y += Input.acceleration.y;

		transform.position = transform.position + movement * Time.deltaTime * 30f;
	}
}
