using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class PlanetCamera : MonoBehaviour {

	[Range(0.00f, 5.0f)]
	public float MoveSpeed = 0.1f;

	[Range(0.0001f, 30.0f)]
	public float zoom = 5;

	[Range(0.0f, 10.0f)]
	public float blurStregth = 1.0f;

	public GameObject mainCamera;

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

		if (mainCamera != null) {
			mainCamera.GetComponent<BlurOptimized> ().blurSize = blurStregth / zoom;
		}
	}
}
