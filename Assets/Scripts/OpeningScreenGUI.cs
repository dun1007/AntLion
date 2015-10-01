using UnityEngine;
using System.Collections;

public class OpeningScreenGUI : MonoBehaviour {

	public GUIStyle buttonStyle;
	public GUIStyle boxStyle;
	public Vector2 boxSize;
	public Vector2 buttonSize;
	private GlobalState gameState;

	// Use this for initialization
	void Start () {
		gameState = GameObject.Find ("GlobalState").GetComponent<GlobalState>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		Vector2 centre = new Vector2(Screen.width/2, Screen.height/2);
		GUI.Box (new Rect(centre.x-boxSize.x/2.0f, centre.y-boxSize.y/2, boxSize.x, boxSize.y), "", boxStyle);
		// Make a button. We pass in the GUIStyle defined above as the style to use
		if (GUI.Button (new Rect(centre.x-buttonSize.x/2.0f, centre.y-buttonSize.y/2.0f + 50, buttonSize.x, buttonSize.y), "Easy", buttonStyle)) {
			gameState.difficulty = GlobalState.Difficulty.EASY;
			Application.LoadLevel("Game");
		}
		if (GUI.Button (new Rect(centre.x-buttonSize.x/2.0f, centre.y-buttonSize.y/2.0f, buttonSize.x, buttonSize.y), "Medium", buttonStyle)) {
			gameState.difficulty = GlobalState.Difficulty.MEDIUM;
			Application.LoadLevel("Game");
		}
		if (GUI.Button (new Rect(centre.x-buttonSize.x/2.0f, centre.y-buttonSize.y/2.0f - 50, buttonSize.x, buttonSize.y), "Hard", buttonStyle)) {
			gameState.difficulty = GlobalState.Difficulty.HARD;
			Application.LoadLevel("Game");
		}
	}
}
