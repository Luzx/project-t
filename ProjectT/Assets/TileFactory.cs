using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class TileFactory : MonoBehaviour {


	public GameObject tileFactory;
	public Camera tileCamera;
	public Camera mainCamera;

	public int preloadingLevel = 2;
	public float keepDistance = 5;

	RenderTexture nirvanaTexture;

	private Dictionary<Vector2, Tile> createdTiles;

	// Use this for initialization
	void Start () {

		createdTiles = new Dictionary<Vector2, Tile> ();

		nirvanaTexture = new RenderTexture (1, 1, 24);

		buildTile (0, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void buildTile(int x, int y) {
		
		var plane = (GameObject)Instantiate(Resources.Load("Tile"));

		plane.transform.position = new Vector3 (x * 10, y * 10);
		var texRenderer = plane.GetComponent<Renderer> ();

		var terrainGen = tileFactory.GetComponent<TerrainGenerator> ();
		terrainGen.x = - x * 10;
		terrainGen.y = - y * 10;

		var renderTexture = new RenderTexture (1024, 1024, 24);

		texRenderer.material.mainTexture = renderTexture;
		tileCamera.targetTexture = renderTexture;

		createdTiles.Add (new Vector2 (x, y), new TileFactory.Tile(plane, renderTexture));

		tileCamera.enabled = true;
	}

	void OnPreRender() {


	}

	private bool checkTile(Vector2 pos, int level = 0) {
		if (level > preloadingLevel) {
			return false;
		}

		level++;

		if (!tileCreated (pos)) {
			buildTile ((int)pos.x, (int)pos.y);
			return true;
		}
			
		if (checkTile (pos + Vector2.up, level))
			return true;

		if (checkTile (pos - Vector2.up, level))
			return true;

		if (checkTile (pos + Vector2.right, level))
			return true;

		if (checkTile (pos - Vector2.right, level))
			return true;

		if (checkTile (pos + Vector2.up + Vector2.right, level))
			return true;

		if (checkTile (pos - Vector2.up + Vector2.right, level))
			return true;

		if (checkTile (pos + Vector2.up - Vector2.right, level))
			return true;

		if (checkTile (pos - Vector2.up - Vector2.right, level))
			return true;

		return false;

	}
		
	private bool tileCreated(Vector2 pos) {
		return createdTiles.ContainsKey (pos);
	}

	void OnPostRender() {
		tileCamera.targetTexture = nirvanaTexture;

		Vector2 curPos = new Vector2((int)(mainCamera.transform.position.x / 10), (int)(mainCamera.transform.position.y / 10));

		checkTile (curPos);

		List<Vector2> keys = new List<Vector2> (createdTiles.Keys);

		foreach (var key in keys) {
			var value = createdTiles[key];

			if ((key - curPos).magnitude > keepDistance) {
				Destroy (value.renderTexture, 0.1f);
				Destroy (value.plane, 0.2f);
				createdTiles.Remove (key);
			}
		}
	}

	public class Tile {
		public GameObject plane;
		public RenderTexture renderTexture;

		public Tile(GameObject plane, RenderTexture renderTexture) {
			this.plane = plane;
			this.renderTexture = renderTexture;
		}
	}

}
