using UnityEngine;
using System.Collections;

public class NoiseGenerator : MonoBehaviour {

	public int pixWidth;
	public int pixHeight;
	public float xOrg;
	public float yOrg;
	public float scale = 1.0f;


	void Start () {
		Renderer renderer = GetComponent<Renderer> ();
		Texture2D noiseTex = new Texture2D (pixWidth, pixHeight);
		Color[] pix = new Color[noiseTex.width * noiseTex.height];
		renderer.material.mainTexture = noiseTex;


		for(float y = 0; y < noiseTex.height; y++) {
			for(float x = 0; x < noiseTex.width; x++) {
				float xCoord = xOrg + x / noiseTex.width * scale;
				float yCoord = yOrg + y / noiseTex.height * scale;

				float sample = Mathf.PerlinNoise (xCoord, yCoord);

				pix [(int)(y * noiseTex.width + x)] = new Color (sample, sample, sample);
			}
		}

		noiseTex.SetPixels (pix);
		noiseTex.Apply ();

	}


	void Update () {

	}
}
