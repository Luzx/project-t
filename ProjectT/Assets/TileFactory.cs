using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class TileFactory : MonoBehaviour {


	public GameObject tileFactory;
	public Camera tileCamera;
	public Camera mainCamera;
	private int curX = 0;
	private int curY = 0;

	RenderTexture nirvanaTexture;

	private List<Vector2> createdTiles;

	// Use this for initialization
	void Start () {

		createdTiles = new List<Vector2> ();

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

		createdTiles.Add (new Vector2 (x, y));

		tileCamera.enabled = true;
	}

	void OnPreRender() {


	}

	void OnPostRender() {
		tileCamera.enabled = false;	
		tileCamera.targetTexture = nirvanaTexture;
		tileCamera.enabled = true;


		Vector2 curTile = new Vector2((int)(mainCamera.transform.position.x / 10), (int)(mainCamera.transform.position.y / 10));
		if (!createdTiles.Contains (curTile)) {
			buildTile ((int)curTile.x, (int)curTile.y);
		} 
	}

}
