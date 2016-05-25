using UnityEngine;
using System.Collections;

public class ScrollTexture : MonoBehaviour {

	public float scrollspeed = 1f;
	private Renderer rend;

	// Use this for initialization
	void Start () {
		rend = gameObject.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		float offset = Time.time * scrollspeed * Globals.speedScale;
		rend.material.SetTextureOffset ("_MainTex", new Vector2(0,offset));
	}
}
