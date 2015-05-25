using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanetFactory : MonoBehaviour {
	
	
	public GameObject terrainFactory;
	public Camera terrainCamera;
	
	public bool created = false;
	public bool createdHighRes = false;

	RenderTexture nirvanaTexture;

	public RenderTexture sdTerrain;
	public RenderTexture hdTerrain;
	
	// Use this for initialization
	void Start () {
		
		nirvanaTexture = new RenderTexture (1, 1, 24);

		terrainCamera.targetTexture = nirvanaTexture;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftControl)) {
			created = false;
			createdHighRes = false;
		}
	}

	private RenderTexture buildTerrain(int x, int y, RenderTexture renderTexture) {

		//var texRenderer = plane.GetComponent<Renderer> ();

		//texRenderer.material.mainTexture = renderTexture;

		var terrainGen = terrainFactory.GetComponent<TerrainGenerator> ();
		terrainGen.x = - x * 10 * terrainGen.scale;
		terrainGen.y = - y * 10 * terrainGen.scale;

		terrainCamera.targetTexture = renderTexture;
		terrainCamera.enabled = true;

		return renderTexture;
	}

	
	void OnPostRender() {
		terrainCamera.targetTexture = nirvanaTexture;

		if (!created) {
			buildTerrain (0, 0, sdTerrain);
			created = true;
		} else if (!createdHighRes) {
			buildTerrain (0, 0, hdTerrain);
			createdHighRes = true;
		}

	}
	
	public void rebuild() {
		created = false;
		createdHighRes = false;
	}
	
}
