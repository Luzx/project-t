using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

public class TileEngine : MonoBehaviour {

	Dictionary<Vector2, string> tileFiles;

	Texture nirvanaTexture;

	ImprovedPerlinNoise m_perlin;

	private Material material;


	public bool Loaded {
		get {
			return loaded;
		}
	}

	private bool loaded = false;


	public int m_seed = 0;
	public float m_frequency = 3.0f;
	public float m_lacunarity = 2.5f;
	public float m_gain = 0.5f;
	public float scale = 10;

	public int size = 512;

	private string path;

	//int x = 0;

	void Start () {

		m_perlin = new ImprovedPerlinNoise(m_seed);
		m_perlin.LoadResourcesFor2DNoise();

		material = new Material (Shader.Find ("Custom/Perlin"));
		material.SetTexture("_PermTable1D", m_perlin.GetPermutationTable1D());
		material.SetTexture("_Gradient2D", m_perlin.GetGradient2D());

		material.SetFloat ("_left", 0);
		material.SetFloat ("_top", 0);
		material.SetFloat ("_right", 0.25f);
		material.SetFloat ("_bottom", 0.75f);

		nirvanaTexture = new RenderTexture (size, size, 24, RenderTextureFormat.ARGB32);
		material.mainTexture = nirvanaTexture;


		path = Application.dataPath + "/tiles/";
		Directory.CreateDirectory(path);

		tileFiles = new Dictionary<Vector2, string>();


		for(int x = -10; x <= 10; x++) {
			for(int y = -10; y <= 10; y++) {
				var timer = new Stopwatch ();

				timer.Start ();
				tileFiles.Add(new Vector2(x, y), genTile (x, y));

				UnityEngine.Debug.Log ("generating a tile took " + timer.ElapsedMilliseconds + "ms");
			}
		}
		loaded = true;

	}
	
	// Update is called once per frame
	void Update () {

	}

	private string genTile(int x, int y) {
		
		var texture = new RenderTexture (size, size, 24, RenderTextureFormat.ARGB32);

		buildTerrain (x, y, texture);

		var tex2d = toTex2d(texture);

		var bytes = tex2d.EncodeToPNG ();

		DestroyImmediate (tex2d);
		DestroyImmediate (texture);


		string filename = "tile_" + x + "_" + y + ".png";
		File.WriteAllBytes(path + filename, bytes);

		return filename;
	}


	private Texture2D toTex2d(RenderTexture renderTexture) {


		RenderTexture.active = renderTexture;

		Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
		tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
		tex.Apply ();

		RenderTexture.active = null;

		return tex;
	}


	private RenderTexture buildTerrain(float x, float y, RenderTexture renderTexture) {

		material.SetFloat ("_Frequency", m_frequency);
		material.SetFloat ("_Lacunarity", m_lacunarity);
		material.SetFloat ("_Gain", m_gain);
		material.SetFloat ("_x", -x * scale);
		material.SetFloat ("_y", -y * scale);
		material.SetFloat ("_scale", scale);

		Graphics.Blit (nirvanaTexture, renderTexture, material);

		return renderTexture;
	}


	public string getTilePath(int x, int y) {
		Vector2 pos = new Vector2 (x, y);

		if (tileFiles.ContainsKey (pos)) {
			return path + tileFiles [pos];
		}

		return null;
	}

	public WWW getTileLoader(int x, int y) {
		var tilefile = getTilePath (x, y);

		if (tilefile != null) {

			var www = new WWW ("file://" + tilefile);

			return www;
		}

		return null;
	}
		
}
