using UnityEngine;
using System.Collections;

public class TerrainColor : MonoBehaviour {
	
	public float moveSpeed;
	
	public Color waterColor;
	public Color sandColor;
	public Color grassColor;
	public Color rockColor;
	public Color iceColor;
	
	public float waterThreshold;
	public float sandThreshold;
	public float grassThreshold;
	public float rockThreshold;
	
	public float elevation;
	
	public float heightVariance = 1.0f;
	
	public float perlinShadowBias;
	
	public float mapCoefficient;
	public float mountainHeight = 10;
	

	
	private Renderer renderer;
	
	void Start () 
	{
		renderer = GetComponent<Renderer> ();
		renderer.material.SetTexture("_MainTex", renderer.material.mainTexture);
	}
	
	void HandleInput ()
	{

		if (Input.GetKey ("u"))
			elevation += moveSpeed / 20;
		if (Input.GetKey ("j"))
			elevation -= moveSpeed / 20;
		if (Input.GetKey ("y"))
			heightVariance += moveSpeed / 20;
		if (Input.GetKey ("h"))
			heightVariance -= moveSpeed / 20;

		if (Input.GetKey ("r"))
			mapCoefficient += moveSpeed;
		if (Input.GetKey ("f"))
			mapCoefficient -= moveSpeed;

		if (Input.GetKey ("n"))
			mountainHeight += moveSpeed;
		if (Input.GetKey ("m"))
			mountainHeight -= moveSpeed;
	}
	
	void SetShaderArguments ()
	{
		renderer.material.SetFloat ("_waterThreshold", waterThreshold);
		renderer.material.SetFloat ("_sandThreshold", sandThreshold);
		renderer.material.SetFloat ("_grassThreshold", grassThreshold);
		renderer.material.SetFloat ("_rockThreshold", rockThreshold);
		renderer.material.SetFloat ("_elevation", elevation);
		renderer.material.SetFloat ("_heightVariance", heightVariance);
		renderer.material.SetFloat ("_perlinShadowBias", perlinShadowBias);
		renderer.material.SetFloat ("_perlinShadowBias", perlinShadowBias);
		renderer.material.SetFloat ("_mapCoefficient", mapCoefficient);
		renderer.material.SetFloat ("_mountainHeight", mountainHeight);

		renderer.material.SetFloat ("_LevelWidth", 1.0f / 5.0f);

		renderer.material.SetColor ("_waterColor", waterColor);
		renderer.material.SetColor ("_sandColor", sandColor);
		renderer.material.SetColor ("_grassColor", grassColor);
		renderer.material.SetColor ("_rockColor", rockColor);
		renderer.material.SetColor ("_iceColor", iceColor);

	}
	
	void Update()
	{

		HandleInput ();
		
		SetShaderArguments ();
	}
	
}
