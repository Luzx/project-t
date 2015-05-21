using UnityEngine;
using System.Collections;

public class NoiseGenerator : MonoBehaviour {

	public int pixWidth;
	public int pixHeight;
	public float xOrg;
	public float yOrg;
	public float scale = 1.0f;

	public Color waterColor;
	public Color sandColor;
	public Color grassColor;
	public Color rockColor;

	public float waterThreshold;
	public float sandThreshold;
	public float grassThreshold;
	public float rockThreshold;

	public float perlinShadowBias;

	public float elevation = 1;
	public float heightVariance = 1;

	public float moveSpeed = 1;

	private Renderer renderer;
	private Texture2D noiseTex;
	private Color[] pix;


	void Start () {
		renderer = GetComponent<Renderer> ();
		noiseTex = new Texture2D (pixWidth, pixHeight);
		pix = new Color[noiseTex.width * noiseTex.height];
		renderer.material.mainTexture = noiseTex;
	}

	void Render () {



		for(float y = 0; y < noiseTex.height; y++) {
			for(float x = 0; x < noiseTex.width; x++) {
				float xCoord = xOrg + x / noiseTex.width * scale;
				float yCoord = yOrg + y / noiseTex.height * scale;

				//Generate heightmap
				float sample  = 0;
				sample += Mathf.PerlinNoise (xCoord + 5, yCoord) * 0.3f * 0.3f;
				sample += Mathf.PerlinNoise (xCoord / 7, yCoord / 7) * heightVariance + elevation;
				sample += Mathf.PerlinNoise (xCoord / 7, (yCoord + scale) / 7) * heightVariance + elevation;
				sample += Mathf.PerlinNoise (xCoord + 0, yCoord) * 0.3f;
				sample += Mathf.PerlinNoise (xCoord + 10, yCoord + 5) * 0.3f * 0.3f;
				sample += Mathf.PerlinNoise (xCoord + 5, yCoord - 5) * 0.3f * 0.3f;

				sample += Mathf.PerlinNoise (xCoord * 1.5f + 5, yCoord * 1.5f) * 0.3f * 0.3f;

				sample += Mathf.PerlinNoise (xCoord * 40 + 5, yCoord * 40) * 0.025f;
				sample += Mathf.PerlinNoise (xCoord * 20 + 70, yCoord * 20) * 0.023f;
				sample += Mathf.PerlinNoise (xCoord * 10 + 5, yCoord * 10) * 0.024f;
				//sample += Mathf.PerlinNoise (xCoord * 5 * x + 5, yCoord * 5 * y) * 0.04f;

				sample /= 4;

				sample = sample * sample * sample;



				//Assign color zones
				if (sample < waterThreshold) pix [(int)(y * noiseTex.width + x)] = new Color (sample + perlinShadowBias * waterColor.r, sample + perlinShadowBias * waterColor.g, sample + perlinShadowBias * waterColor.b);

				else if (sample < sandThreshold) pix [(int)(y * noiseTex.width + x)] = new Color (sample * perlinShadowBias * sandColor.r, sample * perlinShadowBias * sandColor.g, sample * perlinShadowBias * sandColor.b);

				else if (sample < grassThreshold) pix [(int)(y * noiseTex.width + x)] = new Color (sample * perlinShadowBias * grassColor.r, sample * perlinShadowBias * grassColor.g, sample * perlinShadowBias * grassColor.b);

				else if (sample < rockThreshold) pix [(int)(y * noiseTex.width + x)] = new Color (sample * perlinShadowBias * rockColor.r, sample * perlinShadowBias * rockColor.g, sample * perlinShadowBias * rockColor.b);


				else pix [(int)(y * noiseTex.width + x)] = new Color (sample, sample, sample);


			}
		}

		noiseTex.SetPixels (pix);
		noiseTex.Apply ();

	}


	void Update () {
		if (Input.GetKey ("w"))
			yOrg -= moveSpeed;
		if (Input.GetKey ("a"))
			xOrg += moveSpeed;
		if (Input.GetKey ("s"))
			yOrg += moveSpeed;
		if (Input.GetKey ("d"))
			xOrg -= moveSpeed;

		Render ();
	}
}
