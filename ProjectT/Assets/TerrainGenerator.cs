using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

	ImprovedPerlinNoise m_perlin;

	public int m_seed = 0;
	public float m_frequency = 1.0f;
	public float m_lacunarity = 2.0f;
	public float m_gain = 0.5f;
	public float m_xOffset = 0f;
	public float m_zOffset = 0f;
	public float scale;
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


	private Renderer renderer;

	void Start () 
	{
		m_perlin = new ImprovedPerlinNoise(m_seed);

		m_perlin.LoadResourcesFor2DNoise();

		renderer = GetComponent<Renderer> ();

		renderer.material.SetTexture("_PermTable1D", m_perlin.GetPermutationTable1D());
		renderer.material.SetTexture("_Gradient2D", m_perlin.GetGradient2D());
	}

	void HandleInput ()
	{
		if (Input.GetKey ("w"))
			m_zOffset += moveSpeed;
		if (Input.GetKey ("a"))
			m_xOffset -= moveSpeed;
		if (Input.GetKey ("s"))
			m_zOffset -= moveSpeed;
		if (Input.GetKey ("d"))
			m_xOffset += moveSpeed;
		if (Input.GetKey ("o"))
			m_frequency += moveSpeed / 20;
		if (Input.GetKey ("l"))
			m_frequency -= moveSpeed / 20;
		if (Input.GetKey ("i"))
			m_lacunarity += moveSpeed / 20;
		if (Input.GetKey ("k"))
			m_lacunarity -= moveSpeed / 20;
		if (Input.GetKey ("u"))
			elevation += moveSpeed / 20;
		if (Input.GetKey ("j"))
			elevation -= moveSpeed / 20;
		if (Input.GetKey ("y"))
			heightVariance += moveSpeed / 20;
		if (Input.GetKey ("h"))
			heightVariance -= moveSpeed / 20;
		if (Input.GetKey ("t"))
			scale += moveSpeed / 20;
		if (Input.GetKey ("g"))
			scale -= moveSpeed / 20;
	}

	void SetShaderArguments ()
	{
		renderer.material.SetFloat ("_Frequency", m_frequency);
		renderer.material.SetFloat ("_Lacunarity", m_lacunarity);
		renderer.material.SetFloat ("_Gain", m_gain);
		renderer.material.SetFloat ("_xOffset", m_xOffset);
		renderer.material.SetFloat ("_zOffset", m_zOffset);
		renderer.material.SetFloat ("_scale", scale);
		renderer.material.SetFloat ("_waterThreshold", waterThreshold);
		renderer.material.SetFloat ("_sandThreshold", sandThreshold);
		renderer.material.SetFloat ("_grassThreshold", grassThreshold);
		renderer.material.SetFloat ("_rockThreshold", rockThreshold);
		renderer.material.SetFloat ("_elevation", elevation);
		renderer.material.SetFloat ("_heightVariance", heightVariance);
		renderer.material.SetFloat ("_perlinShadowBias", perlinShadowBias);
		renderer.material.SetFloat ("_perlinShadowBias", perlinShadowBias);
	}

	void Update()
	{
		HandleInput ();

		SetShaderArguments ();
	}

}
