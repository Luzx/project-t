using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using System;

public class PlanetCamera : MonoBehaviour {
	
	[Range(0.00f, 5.0f)]
	public float MoveSpeed = 0.1f;
	
	[Range(0.0001f, 30.0f)]
	public float zoom = 5;
	
	[Range(0.0f, 10.0f)]
	public float blurStregth = 1.0f;
	
	[Range(0.0f, 5.0f)]
	public float zeroBlurThreshold = 0.2f;
	
	[Range(1.0f, 30.0f)]
	public float iterationThreshold2 = 0.2f;
	[Range(1.0f, 30.0f)]
	public float iterationThreshold3 = 0.2f;
	[Range(1.0f, 30.0f)]
	public float iterationThreshold4 = 0.2f;
	
	public GameObject mainCamera;
	
	/*public float Direction {
		get {
			return Direction;
		}
		set {
			//Rotate camera accordningly
			transform.Rotate(new Vector3(0, 0, direction - value));

			direction = value;
		}
	}
	private float direction = 0;*/
	private float direction = 0;
	
	void rotateCamera(float deltaDir)
	{
		transform.Rotate(new Vector3(0, 0, deltaDir), Space.Self);
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var movement = new Vector3();
		
		zoom *= 1.0f + (Input.mouseScrollDelta.y * 0.1f);
		zoom = Math.Max (0.01f, zoom);
		
		//Cartesian movement
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
		
		
		
		//Polar movement
		if(Input.GetKey("w")) {
			float cosA = Mathf.Cos(-transform.eulerAngles.z);
			float sinA = Mathf.Sin(-transform.eulerAngles.z);
			movement = new Vector3(cosA - sinA, 0, 0) * MoveSpeed;
		}
		if(Input.GetKey("s")) {
			movement -= transform.forward * MoveSpeed;
		}
		if(Input.GetKey("a")) {
			//direction += 0.2f * MoveSpeed;
			rotateCamera(4.0f * MoveSpeed);
			//transform.eulerAngles = new Vector3(0, 0, direction);
		}
		if(Input.GetKey("d")) {
			//direction -= 0.2f * MoveSpeed;
			rotateCamera(-4.0f * MoveSpeed);
			//transform.eulerAngles = new Vector3(0, 0, direction);
		}
		
		// android
		movement.x += Input.acceleration.x;
		movement.y += Input.acceleration.y;
		
		transform.position = transform.position + movement * zoom * Time.deltaTime * 30f;
		
		
		GetComponent<Camera> ().orthographicSize = zoom;
		
		
		//Adjust gaussian blur applied to heightmap, so it corresponds to zoom level. 
		//This is necessary because the perlin noise generator produces jagged edges at small scales.
		if (mainCamera != null) {
			
			float blur = Math.Max(0, blurStregth / zoom - zeroBlurThreshold * blurStregth);
			
			var blurrer = mainCamera.GetComponent<BlurOptimized> ();
			blurrer.blurIterations = (blur > iterationThreshold2) ? 2 : 1;
			blurrer.blurIterations = (blur > iterationThreshold3) ? 3 : blurrer.blurIterations;
			blurrer.blurIterations = (blur > iterationThreshold4) ? 4 : blurrer.blurIterations;
			blurrer.blurSize = blur / blurrer.blurIterations;
		}
	}
}
