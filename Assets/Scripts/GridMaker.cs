using UnityEngine;
using System.Collections;

public class GridMaker : MonoBehaviour {

	public int gridSizeHorizontal = 5;
	public int gridSizeVertical = 5;
	public float gridCellWidth = 64.0f;
	public float gridCellHeight = 64.0f;

	public GameObject cell;
	
	void Start () {
		GameObject inst = null;
		for (int x = 0; x < gridSizeHorizontal; x++) {
			for (int y = 0; y < gridSizeVertical; y++) {
				//place the line prefab twice to form the bottom and left side of a grid cell
				Vector3 pos = transform.position + new Vector3(x*gridCellWidth,y*gridCellHeight,-0.1f);
				inst = (GameObject)Instantiate (cell, pos, Quaternion.identity);
				inst.transform.localScale = new Vector3(1.0f, gridCellHeight, 1.0f);
				inst = (GameObject)Instantiate (cell, pos, Quaternion.FromToRotation(transform.up,transform.right));
				inst.transform.localScale = new Vector3(1.0f, gridCellWidth, 1.0f);
				//if at the end of a row, also place the right side
				if (x == gridSizeHorizontal - 1) {
					inst = (GameObject)Instantiate (cell, transform.position + new Vector3(gridSizeHorizontal*gridCellWidth,y*gridCellHeight,-0.1f), Quaternion.identity);
					inst.transform.localScale = new Vector3(1.0f, gridCellHeight, 1.0f);
				}
			}
			//if at the end of a column, also place the top
			inst = (GameObject)Instantiate(cell, transform.position + new Vector3(x*gridCellWidth, gridSizeVertical*gridCellHeight,-0.1f), Quaternion.FromToRotation(transform.up,transform.right));
			inst.transform.localScale = new Vector3(1.0f, gridCellWidth, 1.0f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//nothing to do, the grid already exists
	}
}
