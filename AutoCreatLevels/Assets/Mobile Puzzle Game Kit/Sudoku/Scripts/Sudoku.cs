using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

[System.Serializable]
public class cell{
	public Text label;
	public int solution;
	public bool clue;
	public int gridIndex;
}

public class Sudoku : MonoBehaviour {
	
	public enum clueGenerator{
        simple,
        efficient,
		automatic,
    }
	
	//hide these variables in the inspector (we're using a custom inspector)
	[HideInInspector]
	public clueGenerator clueGeneratorType;
	[HideInInspector]
	public float effectTime;
	[HideInInspector]
	public Color emptyColor;
	[HideInInspector]
	public Color labelColor;
	[HideInInspector]
	public Color clueColor;
	[HideInInspector]
	public Color clueLabelColor;
	[HideInInspector]
	public Color correctColor;
	[HideInInspector]
	public Color wrongColor;
	[HideInInspector]
	public float fadeSpeed;
	[HideInInspector]
	public int simpleClueGeneratorMinClues;
	
	//visible in the inspector
	public GameObject cell;
	public GameObject cellGrid;
	public Text loadingText;
	public GameObject numbers;
	public Image newButton;
	public Sprite cellBackground;
	public Text maxCluesText;
	public Slider maxCluesSlider;
	public Image maxCluesHandle;
	
	//not visible in the inspector
	int maxClues;
	int clueAttempts;
	
	GridLayoutGroup grid;
	RectTransform rect;
	
	bool fade;
	int solutionCount;
	
	[HideInInspector]
	public List<cell> cells = new List<cell>();
	List<int> notClues = new List<int>();

	void Start () {
		//don't yet show the numbers
		numbers.SetActive(false);
		
		//give the 'new' button a transparent color until the puzzle has been generated
		newButton.color = new Color(newButton.color.r, newButton.color.g, newButton.color.b, 0.3f);
		
		//get the grid and transform components
		grid = GetComponent<GridLayoutGroup>();
		rect = GetComponent<RectTransform>();
		
		//find the cell size and assign it to the grid
		float cellWidth = rect.rect.width/3 - (2 * grid.spacing.x/3) - (((float)grid.padding.left + (float)grid.padding.right)/3);
		grid.cellSize = new Vector2(cellWidth, cellWidth);
		
		//get the last clues slider value if it wasn't 0 (0 would mean the player prefs were deleted)
		if(PlayerPrefs.GetInt("max clues") != 0){
			maxCluesSlider.value = PlayerPrefs.GetInt("max clues");
			maxCluesText.text = PlayerPrefs.GetInt("max clues") + "";
			maxClues = PlayerPrefs.GetInt("max clues");
		}
		else{
			//if the player prefs were deleted, reset the clues slider to 50
			maxCluesSlider.value = 50;
			maxCluesText.text = 50 + "";
			maxClues = 50;
		}
		//get the start slider color based on the slider value
		maxCluesHandle.color = new Color((100f - (float)maxClues)/100f, (float)maxClues/130f, 0.2f, 1);
		
		//if we're using the automatic generator, use the max amount of clues to find out whether we should use the simple or the efficient way of generating clues
		if(clueGeneratorType == clueGenerator.automatic){
			if(maxClues < simpleClueGeneratorMinClues){
				clueGeneratorType = clueGenerator.efficient;
			}
			else{
				clueGeneratorType = clueGenerator.simple;
			}
		}
		
		//start generating the puzzle
		generate();
	}
	
	void Update(){
		//if the puzzle has been generated and we're fading in the grid
		if(fade){
			//change the button alpha and hide the loading text
			newButton.color = new Color(newButton.color.r, newButton.color.g, newButton.color.b, 1f);
			loadingText.color = new Color(1, 1, 1, 0);
			
			//show the numbers we can choose from
			if(!numbers.activeSelf)
				StartCoroutine(showNumbers());
			
			//if the cells are not fully visible yet
			if(cells[0].label.transform.parent.GetChild(1).GetComponent<Image>().color.a > 0){
				//fade out all cell cover objects
				for(int i = 0; i < 81; i++){
					Image cellCover = cells[i].label.transform.parent.GetChild(1).GetComponent<Image>();
					cellCover.color = new Color(cellCover.color.r, cellCover.color.g, cellCover.color.b, cellCover.color.a - (Time.deltaTime * fadeSpeed));
				}
			}
			else{
				//stop fading
				fade = false;
			}
		}
	}
	
	IEnumerator showNumbers(){
		//disable all numbers
		foreach(Transform child in numbers.transform){
			child.gameObject.SetActive(false);
		}
		
		//enable the parent object
		numbers.SetActive(true);
		
		//for each number, enable it and wait a moment
		for(int i = 1; i <= 10; i++){
			numbers.transform.GetChild(i).gameObject.SetActive(true);
			yield return new WaitForSeconds(effectTime/15);
		}
		
		//wait 0.5 seconds and enable the first child object
		yield return new WaitForSeconds(0.5f);
		numbers.transform.GetChild(0).gameObject.SetActive(true);
	}
	
	public void generate(){
		//first, add all empty cells
		for(int i = 0; i < 81; i++){
			cells.Add(new cell {solution = 0});
		}
		
		//if we need to regenerate the puzzle, start again
		if(regenerate()){
			cells.Clear();
			generate();
			return;
		}
		else{
			//if we don't need to generate again, load the grid
			StartCoroutine(loadGrid());
		}
	}
	
	public bool regenerate(){
		//for all rows and columns in the grid
		for(int i = 0; i < 9; i++){
			for(int I = i * 9; I < i * 9 + 9; I++){
				//store the grid index so we know the section of the grid that contains this cell
				cells[I].gridIndex = i;
				//by default, each cell is a clue
				cells[I].clue = true;
				
				//get the number for this cell
				int number = getNumber(I);
				
				//10 is not possible, so if the number is not 10, use the number as the solution for this cell
				if(number != 10){
					cells[I].solution = number;
				}
				else{
					//if the number returned 10, we know it's not possible so we need to regenerate the grid
					return true;
				}
			}
		}
		
		//if all numbers fit, we don't have to generate the main grid again
		return false;
	}
	
	IEnumerator loadGrid(){
		//for each sub-grid (there's 9 small grids with 9 cells each, part of one big grid)
		for(int i = 0; i < 9; i++){
			//create the sub-grid and parent it to this main grid
			GameObject newGrid = Instantiate(cellGrid);
			newGrid.transform.SetParent(transform, false);
			
			//get the grid and transform components
			GridLayoutGroup newGridLayout = newGrid.GetComponent<GridLayoutGroup>();
			RectTransform newRect = newGrid.GetComponent<RectTransform>();
			
			//for all cells in this sub-grid
			for(int I = i * 9; I < i * 9 + 9; I++){
				//wait a short moment and create the cell
				yield return new WaitForSeconds(effectTime/81);
				GameObject newCell = Instantiate(cell);
				
				//get the cell label
				cells[I].label = newCell.transform.GetChild(0).GetComponent<Text>(); 
				
				//parent the cell to the sub-grid and name it after its index
				newCell.transform.SetParent(newGrid.transform, false);
				newCell.name = I + "";
				
				//get the cell size and assign it to the grid
				float cellWidth = newRect.rect.width/3 - (2 * newGridLayout.spacing.x/3) - (((float)newGridLayout.padding.left + (float)newGridLayout.padding.right)/3);
				newGridLayout.cellSize = new Vector2(cellWidth, cellWidth);
			}
		}
		
		//wait a moment and get the clues
		yield return new WaitForSeconds(0.2f);
		getClues();
	}
	
	public int getNumber(int i){
		//get a random number from 1 to 9
		int number = Random.Range(1, 10);
		
		//use the random number as the start number and continue to 9
		for(int n = number; n < 10; n++){
			//check if the number fits and if it does, return it
			if(horizontal(n, i, false) && vertical(n, i, false) && box(n, i, false))
				return n;
		}
		//if none of the previous numbers fit, try again from 1 to the random number
		for(int N = 1; N < number; N++){
			if(horizontal(N, i, false) && vertical(N, i, false) && box(N, i, false))
				return N;
		}
		
		//if all numbers from 1 to 9 don't fit, return 10 to let it know we couldn't find a fitting number
		return 10;
	}
	
	public void getClues(){
		//if this try resulted in too much clues, generate clues again
		while(clues() > maxClues){
			//keep track of the number of attempts
			clueAttempts++;
			
			//if we're trying to generate the clues in an efficient way and we've generated the clues 30 times
			if(clueAttempts >= 30 && clueGeneratorType == clueGenerator.efficient){
				//reset the number of attempts
				clueAttempts = 0;
				
				//reset all solutions
				for(int i = 0; i < 81; i++){
					cells[i].solution = 0;
				}
				
				//regenerate the base grid
				while(regenerate()){
					for(int i = 0; i < 81; i++){
						cells[i].solution = 0;
					}
				}
			}
			
			//generate clues again
			generateClues();
		}
		
		//if the clues are alright, go through all cells
		for(int N = 0; N < 81; N++){
			//get the cell image
			Image image = cells[N].label.transform.parent.GetComponent<Image>();
			
			//if this cell is a clue
			if(cells[N].clue){
				//show the solution, change the cell colors and set sprite to null
				cells[N].label.text = cells[N].solution + "";
				cells[N].label.color = clueLabelColor;
				image.color = clueColor;
				image.sprite = null;
			}
			else{
				//if this is not a clue, don't show the solution yet and show the cell background
				cells[N].label.text = "";
				cells[N].label.color = labelColor;
				image.color = emptyColor;
				image.sprite = cellBackground;
			}
		}
		
		//start fading the grid
		fade = true;
	}
	
	public int clues(){
		int clueCount = 0;
		
		//add one to the number of clues for each clue
		for(int i = 0; i < 81; i++){
			if(cells[i].clue)
				clueCount++;
		}
		
		//return the total number of clues
		return clueCount;
	}
	
	public void generateClues(){
		//first make all cells clues
		for(int N = 0; N < 81; N++){
			cells[N].clue = true;

			Image image = cells[N].label.transform.parent.GetComponent<Image>();
				
			cells[N].label.text = cells[N].solution + "";
			cells[N].label.color = clueLabelColor;
			image.color = clueColor;
			image.sprite = null;
		}
		
		//create a list for the cell positions
		List<int> cellPositions = new List<int>();
		//add a new position for each cell
		for(int i = 0; i < 81; i++){
			cellPositions.Add(i);
		}
		
		//if we're using the simple clue generator
		if(clueGeneratorType == clueGenerator.simple){
			//create a list of shuffled cell positions
			List<int> shuffledCellPositions = new List<int>();
			
			//for each cell, add a random position (a random number between 0 & 80), to shuffle them
			for(int I = 0; I < 81; I++){
				int cell = Random.Range(0, cellPositions.Count);
				shuffledCellPositions.Add(cellPositions[cell]);
				cellPositions.RemoveAt(cell);
			}
			
			//for each position in the grid, check if it should be a clue or not
			for(int n = 0; n < 81; n++){
				checkForClue(shuffledCellPositions[n]);
			}
		}
		//if we're using the efficient clue generator
		else{
			//for all cells
			for(int I = 0; I < 81; I++){
				//create a list with the best positions
				List<int> bestPositions = new List<int>();
				//the most clues starts with 0
				int mostClues = 0;
				
				//for all 81 numbers
				for(int n = 0; n < cellPositions.Count; n++){
					//get the number of surrounding clues for the position at this index
					int clueCount = surroundingClueCount(cellPositions[n]);
					
					//if the clue count is the same as the most clues, add this position to the best positions
					if(clueCount == mostClues){
						bestPositions.Add(cellPositions[n]);
					}
					//if the clue count is bigger than most clues, set the clue count to the most clues number and reset the best positions to this position
					else if(clueCount > mostClues){
						mostClues = clueCount;
						bestPositions.Clear();
						bestPositions.Add(cellPositions[n]);
					}
				}
				
				//get a random cell from the best positions, remove it from the cell positions and check if the random cell should be a clue or not
				int randomCell = bestPositions[Random.Range(0, bestPositions.Count)];
				cellPositions.Remove(randomCell);
				checkForClue(randomCell);
			}
		}
	}
	
	public int surroundingClueCount(int cell){
		//number of clues surrouding this cell
		int clueCount = 0;
		
		//find the horizontal start position
		int startPositionHor = 0;
		for(int i = cell; i >= 0; i--){
			if(i % 9 == 0 || i % 9 == 3 || i % 9 == 6){
				startPositionHor = i;
				break;
			}
		}
		
		//find the sub-grid that contains the start position
		int gridIndexHor = cells[startPositionHor].gridIndex;
		
		//move the horizontal start position to the most left sub-grid
		if(gridIndexHor == 1 || gridIndexHor == 4 || gridIndexHor == 7){
			startPositionHor -= 9;
		}
		else if(gridIndexHor == 2 || gridIndexHor == 5 || gridIndexHor == 8){
			startPositionHor -= 18;
		}
		
		//get all cell positions in this horizontal row
		int[] horizontalCells = {startPositionHor, startPositionHor + 1, startPositionHor + 2, startPositionHor + 9, startPositionHor + 10, startPositionHor + 11, startPositionHor + 18, startPositionHor + 19, startPositionHor + 20};
		
		//for all cells in the horizontal row, if it's a clue, increase the clue count
		for(int I = 0; I < 9; I++){
			if(cells[horizontalCells[I]].clue)
				clueCount++;
		}
		
		//find the vertical start position
		int startPositionVer = 0;
		for(int n = cell; n >= 0; n -= 3){
			if(n % 9 == 0 || n % 9 == 1 || n % 9 == 2){
				startPositionVer = n;
				break;
			}
		}
		
		//find the sub-grid that contains this start position
		int gridIndexVer = cells[startPositionVer].gridIndex;
		
		//move the vertical start position to the first sub-grid
		if(gridIndexVer == 3 || gridIndexVer == 4 || gridIndexVer == 5){
			startPositionVer -= 27;
		}
		else if(gridIndexVer == 6 || gridIndexVer == 7 || gridIndexVer == 8){
			startPositionVer -= 54;
		}
		
		//get all cell positions in the vertical column
		int[] verticalCells = {startPositionVer, startPositionVer + 3, startPositionVer + 6, startPositionVer + 27, startPositionVer + 30, startPositionVer + 33, startPositionVer + 54, startPositionVer + 57, startPositionVer + 60};
		
		//for all vertical cells, if it's a clue, increase the cell count
		for(int N = 0; N < 9; N++){
			if(cells[verticalCells[N]].clue)
				clueCount++;
		}
		
		//start position in the box (the sub-grid)
		int startPositionBox = 0;
		
		//find the start position in the box
		for(int j = cell; j >= 0; j--){
			if(j % 9 == 0){
				startPositionBox = j;
				break;
			}
		}
		
		//for all cells in the box, if it's a clue increase the clue count
		for(int J = startPositionBox; J < startPositionBox + 9; J++){
			if(cells[J].clue)
				clueCount++;
		}
		
		//return the total amount of clues surrouding this cell
		return clueCount;
	}
	
	public void checkForClue(int cell){
		//total number of solutions
		solutionCount = 0;
		
		//disable the clue at this cell position
		cells[cell].clue = false;
		cells[cell].label.text = "";
		
		//clear all cell positions that or not clues
		notClues.Clear();
		
		//for all cells, add the cells that are not clues
		for(int i = 0; i < 81; i++){
			if(!cells[i].clue)
				notClues.Add(i);
		}
		
		//check the number of solutions
		checkSolutions();
		
		//if the number of solutions for the current grid is not 1, we need to enable this clue to make sure the puzzle has 1 solution
		if(solutionCount != 1){
			cells[cell].clue = true;
			cells[cell].label.text = cells[cell].solution + "";
		}
		
		return;
	}
	
	public void checkSolutions(){
		int i = 0;
		
		//check for new solutions
		while(newSolution(i)){
			solutionCount++;
			i = notClues.Count - 1;
			
			//two solutions already means we need to enable the clue so we can return
			if(solutionCount > 1)
				return;
		}
	}
	
	public bool newSolution(int i){
		int I = i;
		
		//go through all cell positions that are not clues
		while(I < notClues.Count){
			
			//if it can backtrack this position, go to the next cell that is not a clue
			if(backtrack(notClues[I])){
				I++;
			}
			//if it can't, while the not-clue cell index is bigger than 0, go back to the last cell that is not a clue
			else{
				if(I > 0){
					I--;
				}
				else{
					//if this is the first one again, there's no solution left
					return false;
				}
			}
		}
		
		//if we went through all cells, there's a new solution
		return true;
	}
	
	public bool backtrack(int i){
		//start with the number 1
		int startValue = 1;
		
		//if the cell has a number, set the start value to that number + 1
		if(cells[i].label.text != "")
			startValue = int.Parse(cells[i].label.text) + 1;
		
		//if the text had the number 9 (9 + 1 = 10) we can't check another number so this cell can not be filled and we need to return
		if(startValue == 10){
			cells[i].label.text = "";
			return false;
		}
		
		//until 9, for all values check if they are valid and if they are, return true since we found a succesfull value
		for(int I = startValue; I < 10; I++){
			if(horizontal(I, i, true) && vertical(I, i, true) && box(I, i, true)){
				cells[i].label.text = "" + I;
				return true;
			}
		}
		
		//after checking all values, return false since we didn't find a valid value
		cells[i].label.text = "";
		return false;
	}
		
	public void showSolution(){
		//show the solution for this puzzle
		StartCoroutine(solution());
	}
	
	IEnumerator solution(){
		//for all cells in the grid
		for(int i = 0; i < 81; i++){
			//if this cell is not a clue
			if(!cells[i].clue){
				//get the cell image component
				Image image = cells[i].label.transform.parent.GetComponent<Image>();
				
				//if the player filled in the right number, show the green color
				if(cells[i].label.text == cells[i].solution + ""){
					image.color = correctColor;
				}
				else{
					//if the player had a wrong number, show the red color
					image.color = wrongColor;
				}
				
				//show the solution and wait a moment for a nice effect
				cells[i].label.text = cells[i].solution + "";
				yield return new WaitForSeconds(effectTime/81);
			}
		}
	}
	
	public void updateMaxClues(){
		//when we slide the clues slider, save the new clues value
		PlayerPrefs.SetInt("max clues", (int)maxCluesSlider.value);
		//change the text next to the slider
		maxCluesText.text = PlayerPrefs.GetInt("max clues") + "";
		//set the actual max clues value
		maxClues = PlayerPrefs.GetInt("max clues");
		//update the color of the slider accordingly
		maxCluesHandle.color = new Color((100f - (float)maxClues)/100f, (float)maxClues/130f, 0.2f, 1);
	}
	
	public bool horizontal(int number, int n, bool checkingClues){
		//horizontal start position
		int startPosition = 0;
		
		//find the horizontal start position
		for(int i = n; i >= 0; i--){
			if(i % 9 == 0 || i % 9 == 3 || i % 9 == 6){
				startPosition = i;
				break;
			}
		}
		
		//get the sub-grid that contains this cell
		int gridIndex = cells[startPosition].gridIndex;
		
		//move the start position to the first sub-grid
		if(gridIndex == 1 || gridIndex == 4 || gridIndex == 7){
			startPosition -= 9;
		}
		else if(gridIndex == 2 || gridIndex == 5 || gridIndex == 8){
			startPosition -= 18;
		}
		
		//get all cell positions in the horizontal row
		int[] checkCells = {startPosition, startPosition + 1, startPosition + 2, startPosition + 9, startPosition + 10, startPosition + 11, startPosition + 18, startPosition + 19, startPosition + 20};
		
		//for all horizontal positions
		for(int I = 0; I < 9; I++){
			//if we're not checking for clues
			if(!checkingClues){
				//return false if this is not the cell that we were checking and the solution of this cell is the number
				if(checkCells[I] != n && cells[checkCells[I]].solution == number)
					return false;
			}
			else{
				//if we are checking for clues, do the same but now check the actual label
				if(checkCells[I] != n && cells[checkCells[I]].label.text == number + "")
					return false;
			}
		}
		
		//if we've checked all horizontal cells return true
		return true;
	}
	
	public bool vertical(int number, int n, bool checkingClues){
		//vertical start position
		int startPosition = 0;
		
		//get the vertical start position
		for(int i = n; i >= 0; i -= 3){
			if(i % 9 == 0 || i % 9 == 1 || i % 9 == 2){
				startPosition = i;
				break;
			}
		}
		
		//get the sub-grid that contains this cell
		int gridIndex = cells[startPosition].gridIndex;
		
		//move the start position to the first sub-grid
		if(gridIndex == 3 || gridIndex == 4 || gridIndex == 5){
			startPosition -= 27;
		}
		else if(gridIndex == 6 || gridIndex == 7 || gridIndex == 8){
			startPosition -= 54;
		}
		
		//get all cell positions in the vertical column
		int[] checkCells = {startPosition, startPosition + 3, startPosition + 6, startPosition + 27, startPosition + 30, startPosition + 33, startPosition + 54, startPosition + 57, startPosition + 60};
		
		//for all vertical cells
		for(int I = 0; I < 9; I++){
			//if we're not checking for clues
			if(!checkingClues){
				//return false if this is not the cell that we were checking and the solution of this cell is the number
				if(checkCells[I] != n && cells[checkCells[I]].solution == number)
					return false;
			}
			else{
				//if we are checking for clues, do the same but now check the actual label
				if(checkCells[I] != n && cells[checkCells[I]].label.text == number + "")
					return false;
			}
		}
		
		//if we've checked all vertical cells return true
		return true;
	}
	
	public bool box(int number, int n, bool checkingClues){
		//box start position
		int startPosition = 0;
		
		//find the start position in this sub-grid (which automatically is the start position since we're checking the box)
		for(int i = n; i >= 0; i--){
			if(i % 9 == 0){
				startPosition = i;
				break;
			}
		}
		
		//for all cells in the sub-grid
		for(int I = startPosition; I < startPosition + 9; I++){
			//if we're not checking clues
			if(!checkingClues){
				//return false if this is not the cell that we were checking and the solution of this cell is the number
				if(I != n && cells[I].solution == number)
					return false;
			}
			else{
				//if we are checking for clues, do the same but now check the actual label
				if(I != n && cells[I].label.text == number + "")
					return false;
			}
		}
		
		//if we've checked all cells in this sub-grid, return true
		return true;
	} 
	
	public void restart(){
		//if we can load a new puzzle, restart the current scene
		if(newButton.color.a == 1)
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}