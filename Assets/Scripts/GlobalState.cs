#define DEBUG

using UnityEngine;
using System.Collections;


public class GlobalState : MonoBehaviour {

	public enum Difficulty {EASY, MEDIUM, HARD};
	public Difficulty difficulty;

	public enum GameState {MENU, PLAYING, LOST, WON};
	public GameState globalState;

	// Use this for initialization
	void Start () {
		Object.DontDestroyOnLoad(this);
		difficulty = Difficulty.MEDIUM;
		globalState = GameState.MENU;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}



public class DebugUtils
{
	[System.Diagnostics.Conditional("DEBUG")]
	public static void Assert(bool condition)
	{
		if (!condition) throw new System.Exception();
	}
}

