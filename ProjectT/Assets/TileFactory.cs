using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileFactory : MonoBehaviour {
	
	
	public GameObject tileFactory;
	public Camera tileCamera;
	public Camera mainCamera;

	public float tileSize = 10;
	
	public bool invalidate = false;
	
	public int preloadingLevel = 2;
	public float keepDistance = 5;
	
	RenderTexture nirvanaTexture;
	
	private Dictionary<Vector2, Tile> createdTiles;
	private List<Tile> tilePool;
	private float tileStopwatch = 0;
	
	// Use this for initialization
	void Start () {

		createdTiles = new Dictionary<Vector2, Tile> ();
		tilePool = new List<Tile> ();
		
		nirvanaTexture = new RenderTexture (1, 1, 24);
		
		buildTile (0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftControl))
			invalidate = true;
	}
	
	private void buildTile(int x, int y) {

		var tile = getTile ();

		createdTiles.Add (new Vector2 (x, y), tile);

		tile.plane.transform.position = new Vector3 (x * tileSize, y * tileSize);

		var terrainGen = tileFactory.GetComponent<TerrainGenerator> ();
		terrainGen.x = - x * 10 * terrainGen.scale;
		terrainGen.y = - y * 10 * terrainGen.scale;

		tileCamera.targetTexture = tile.renderTexture;
		tileCamera.enabled = true;
	}
	
	private Tile getTile() {
		if (tilePool.Count > 0) {
			var tile = tilePool [0];
			tilePool.Remove (tile);
			tile.plane.GetComponent<MeshRenderer> ().enabled = true;
			return tile;
		} else {
			var plane = (GameObject)Instantiate(Resources.Load("Tile"));

			plane.transform.localScale = Vector3.one * (tileSize / 10);

			var texRenderer = plane.GetComponent<Renderer> ();



			var renderTexture = new RenderTexture ((int)(256 * tileSize / 10), (int)(256 * tileSize / 10), 24);

			texRenderer.material.mainTexture = renderTexture;


			return new Tile(plane, renderTexture);
		}
	}

	private void removeTile(Tile tile) {
		tile.plane.GetComponent<MeshRenderer> ().enabled = false;
		tilePool.Add (tile);
	}
	
	private bool checkTile(Vector2 pos, int level = 0) {
		if (level > preloadingLevel) {
			return false;
		}
		
		level++;
		
		if (!isTileCreated (pos)) {
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
	
	private bool isTileCreated(Vector2 pos) {
		return createdTiles.ContainsKey (pos);
	}
	
	void OnPostRender() {
		tileCamera.targetTexture = nirvanaTexture;
		
		Vector2 curPos = new Vector2((int)(mainCamera.transform.position.x / tileSize), (int)(mainCamera.transform.position.y / tileSize));
		
		if (invalidate) {
			
			List<Vector2> keys2 = new List<Vector2> (createdTiles.Keys);
			
			foreach (var key in keys2) {
				var value = createdTiles [key];
				createdTiles.Remove (key);
				removeTile (value);
			}
			
			invalidate = false;
		}
		
		List<Vector2> keys = new List<Vector2> (createdTiles.Keys);
		
		foreach (var key in keys) {
			var value = createdTiles[key];
			
			if ((key - curPos).magnitude > keepDistance) {
				createdTiles.Remove (key);
				removeTile (value);
			}
		}

		if (tileStopwatch != 0) {
			Debug.Log ("Tile creation took " + ((Time.realtimeSinceStartup - tileStopwatch) * 1000) + "ms");
			tileStopwatch = 0;
		}

		if (checkTile (curPos)) {
			tileStopwatch = Time.realtimeSinceStartup;
		}
	}
	
	public void invalidateTiles() {
		invalidate = true;
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
