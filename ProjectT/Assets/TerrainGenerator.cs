using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

	ImprovedPerlinNoise m_perlin;

	public int m_seed = 0;
	public float m_frequency = 3.0f;
	public float m_lacunarity = 2.5f;
	public float m_gain = 0.5f;
	//public float m_xOffset = 0f;
	//public float m_zOffset = 0f;
	public float scale;
	public float moveSpeed;

	private float previousX = 0;
	private float previousY = 0;
	public float x = 0f;
	public float y = 0f;


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
		if (Input.GetKey ("o"))
			m_frequency += moveSpeed / 20;
		if (Input.GetKey ("l"))
			m_frequency -= moveSpeed / 20;
		if (Input.GetKey ("i"))
			m_lacunarity += moveSpeed / 20;
		if (Input.GetKey ("k"))
			m_lacunarity -= moveSpeed / 20;

		if (Input.GetKey ("t"))
			scale += moveSpeed / 20 * scale;
		if (Input.GetKey ("g"))
			scale -= moveSpeed / 20 * scale;

	
	}

	void SetShaderArguments ()
	{
		renderer.material.SetFloat ("_Frequency", m_frequency);
		renderer.material.SetFloat ("_Lacunarity", m_lacunarity);
		renderer.material.SetFloat ("_Gain", m_gain);
		renderer.material.SetFloat ("_x", x);
		renderer.material.SetFloat ("_y", y);
		renderer.material.SetFloat ("_scale", scale);
	}

	void Update()
	{
		previousX = x;
		previousY = y;

		HandleInput ();

		SetShaderArguments ();
	}

}
