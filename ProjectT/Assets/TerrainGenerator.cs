using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

	ImprovedPerlinNoise m_perlin;

	public int m_seed = 0;
	public float m_frequency = 1.0f;
	public float m_lacunarity = 2.0f;
	public float m_gain = 0.5f;

	private Renderer renderer;

	void Start () 
	{
		m_perlin = new ImprovedPerlinNoise(m_seed);

		m_perlin.LoadResourcesFor2DNoise();

		renderer = GetComponent<Renderer> ();

		renderer.material.SetTexture("_PermTable1D", m_perlin.GetPermutationTable1D());
		renderer.material.SetTexture("_Gradient2D", m_perlin.GetGradient2D());
	}

	void Update()
	{
		renderer.material.SetFloat("_Frequency", m_frequency);
		renderer.material.SetFloat("_Lacunarity", m_lacunarity);
		renderer.material.SetFloat("_Gain", m_gain);
	}

}
