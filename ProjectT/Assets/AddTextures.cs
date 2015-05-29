using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class AddTextures : MonoBehaviour {

	public Texture water;

	public Texture sand;

	public Texture grass;

	public Texture rock;

	public Texture ice;

	public Texture heightmap;

	public Shader addTextureShader = null;

	public float x, y;

	//TODO: pull from TerrainColor.cs
	/*public float waterThreshold;
	public float sandThreshold;
	public float grassThreshold;
	public float rockThreshold;*/

	private Material mat = null;

	// Use this for initialization
	void Start () {
		mat = new Material(addTextureShader);
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		mat.SetTexture ("_WaterTex", water);
		mat.SetTexture ("_SandTex", sand);
		mat.SetTexture ("_GrassTex", grass);
		mat.SetTexture ("_RockTex", rock);
		mat.SetTexture ("_IceTex", ice);

		mat.SetFloat ("_LevelWidth", 1.0f / 5.0f);

		mat.SetFloat ("_AspectRatio", (float)Screen.width / (float)Screen.height);
		Debug.Log ((float)Screen.width / (float)Screen.height);

		/*mat.SetFloat("_waterThreshold", waterThreshold);
		mat.SetFloat("_sandThreshold", sandThreshold);
		mat.SetFloat("_grassThreshold", grassThreshold);
		mat.SetFloat("_rockThreshold", rockThreshold);*/

		Graphics.Blit (source, destination, mat, 2);

	}

	
}
