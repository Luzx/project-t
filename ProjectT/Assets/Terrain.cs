using UnityEngine;
using System.Collections;

public class Terrain : MonoBehaviour {
	
	public float moveSpeed;
	
	public Color waterColor;
	public Color sandColor;
	public Color grassColor;
	public Color rockColor;
	
	public float waterThreshold;
	public float sandThreshold;
	public float grassThreshold;
	public float rockThreshold;
	
	public float elevation;
	
	public float heightVariance = 1.0f;
	
	public float perlinShadowBias;
	
	public float mapCoefficient;
	

	
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

	}
	
	void Update()
	{

		HandleInput ();
		
		SetShaderArguments ();
	}
	
}
