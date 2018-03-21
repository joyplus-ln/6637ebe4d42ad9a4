using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour {
	
	//variables visible in the inspector
	public GameObject cell;
	public int x;
	public int y;
	public float effectTime;
	public bool square;
	
	//not visible in the inspector
	GridLayoutGroup grid;
	RectTransform rect;

	void Start () {
		//get the grid and rect transform components
		grid = GetComponent<GridLayoutGroup>();
		rect = GetComponent<RectTransform>();
		
		//find cell size
		float X = rect.rect.width/x - ((x - 1) * grid.spacing.x/x) - (((float)grid.padding.left + (float)grid.padding.right)/x);
		float Y = rect.rect.height/y - ((y - 1) * grid.spacing.y/y) - (((float)grid.padding.bottom + (float)grid.padding.top)/y);
		
		//if the grid should be a square, use width, else use x and y
		if(square){
			grid.cellSize = new Vector2(X, X);
		}
		else{
			grid.cellSize = new Vector2(X, Y);
		}
		
		//show the actual grid
		StartCoroutine(loadGrid());
	}
	
	IEnumerator loadGrid(){
		//get the number of cells
		int cellAmount = x * y;
		
		//for each cell, add a cell object and wait a moment
		for(int i = 0; i < cellAmount; i++){
			GameObject newCell = Instantiate(cell);
			newCell.transform.SetParent(transform, false);
			
			yield return new WaitForSeconds(effectTime/(float)cellAmount);
		}
	}
}
