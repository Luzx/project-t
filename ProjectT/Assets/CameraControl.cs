using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var movement = new Vector3();

		if(Input.GetKey(KeyCode.UpArrow)) {
			movement += Vector3.up;
		}
		if(Input.GetKey(KeyCode.LeftArrow)) {
			movement += Vector3.left;
		}
		if(Input.GetKey(KeyCode.RightArrow)) {
			movement += Vector3.right;
		}
		if(Input.GetKey(KeyCode.DownArrow)) {
			movement += Vector3.down;
		}

		transform.position = transform.position + movement * Time.deltaTime * 30f;
	}
}
