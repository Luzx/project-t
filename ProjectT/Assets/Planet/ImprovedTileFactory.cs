using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

public class ImprovedTileFactory : MonoBehaviour {

	public Camera mainCamera;

	public float tileSize = 10;

	public bool invalidate = false;

	public int preloadingLevel = 2;
	public float keepDistance = 5;

	RenderTexture nirvanaTexture;

	private Dictionary<Vector2, Tile> createdTiles;
	private List<Tile> tilePool;

	public GameObject tileEngine;

	// Use this for initialization
	void Start () {
		createdTiles = new Dictionary<Vector2, Tile> ();
		tilePool = new List<Tile> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftControl))
			invalidate = true;


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

		checkTile (curPos);

		checkForTextureLoading ();
	}

	private void checkForTextureLoading() {
		List<Vector2> keys2 = new List<Vector2> (createdTiles.Keys);

		foreach (var key in keys2) {
			var value = createdTiles [key];

			if (value.texLoader != null) {
				if (value.texLoader.isDone) {
					value.texLoader.LoadImageIntoTexture(value.texture);
					value.texLoader = null;
				}
			}
		}

	}

	private void buildTile(int x, int y) {

		var tileEngineComponent = tileEngine.GetComponent<TileEngine> ();
		var texLoader = tileEngineComponent.getTileLoader(x, y);

		if (texLoader != null) {

			var tile = getTile ();

			tile.plane.transform.position = new Vector3 (x * tileSize, y * tileSize);


			tile.texLoader = texLoader;

			var tex = new Texture2D (tileEngineComponent.size, tileEngineComponent.size, TextureFormat.ARGB32, false);
			tex.filterMode = FilterMode.Point;
			tile.texture = tex;

			tile.plane.GetComponent<Renderer> ().material.mainTexture = tex;

			createdTiles.Add (new Vector2 (x, y), tile);

		}

	}

	private Tile getTile() {
		if (tilePool.Count > 0) {
			var tile = tilePool [0];
			tilePool.Remove (tile);
			tile.plane.GetComponent<MeshRenderer> ().enabled = true;
			tile.texLoader = null;
			tile.texture = null;
			return tile;
		} else {
			var plane = (GameObject)Instantiate(Resources.Load("Tile"));

			plane.transform.localScale = Vector3.one * (tileSize / 10);

			return new Tile(plane);
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

	/*
	private void checkTile(Vector2 pos, int level = 0) {
		if (level > preloadingLevel) {
			return;
		}

		level++;

		if (!isTileCreated (pos)) {
			buildTile ((int)pos.x, (int)pos.y);
		}

		checkTile (pos + Vector2.up, level);

		checkTile (pos - Vector2.up, level);

		checkTile (pos + Vector2.right, level);

		checkTile (pos - Vector2.right, level);

		checkTile (pos + Vector2.up + Vector2.right, level);

		checkTile (pos - Vector2.up + Vector2.right, level);

		checkTile (pos + Vector2.up - Vector2.right, level);

		checkTile (pos - Vector2.up - Vector2.right, level);

	}*/

	private bool isTileCreated(Vector2 pos) {
		return createdTiles.ContainsKey (pos);
	}
		

	public void invalidateTiles() {
		invalidate = true;
	}


	public class Tile {
		public GameObject plane;
		public Texture2D texture;
		public WWW texLoader;

		public Tile(GameObject plane = null, Texture2D texture = null, WWW texLoader = null) {
			this.plane = plane;
			this.texture = texture;
			this.texLoader = texLoader;
		}
	}
}
