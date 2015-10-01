using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {

	private AntBehaviour antBehaviour;

	// Use this for initialization
	void Start () {
		GameObject ant = GameObject.Find ("Ant");
		antBehaviour = ant.GetComponent<AntBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		GlobalState.GameState state = antBehaviour.CheckWinLose();
		switch (state) {
		case GlobalState.GameState.LOST:
			GUI.Box(new Rect(Screen.width/2 - 50, Screen.height/2 - 25, 100, 50), "YOU LOSE");
			if (GUI.Button (new Rect(Screen.width/2 - 40, Screen.height/2 - 5, 80, 20), "Restart")) {
				Application.LoadLevel("Menu");
			}
			break;
		case GlobalState.GameState.WON:
			GUI.Box(new Rect(Screen.width/2 - 50, Screen.height/2 - 25, 100, 50), "YOU WIN");
			if (GUI.Button (new Rect(Screen.width/2 - 40, Screen.height/2 - 5, 80, 20), "Restart")) {
				Application.LoadLevel("Menu");
			}
			break;
		}
	}
}
