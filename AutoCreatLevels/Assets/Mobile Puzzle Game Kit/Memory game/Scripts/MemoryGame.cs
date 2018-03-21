using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class level{
	public int x;
	public int y;
	public List<Sprite> images;
}

public class MemoryGame : MonoBehaviour {
	
	//hide the variables in the inspector (the custom inspector shows them)
	[HideInInspector]
	public List<level> levels;
	
	[HideInInspector]
	public bool useLevels;
	
	//variables visible in the inspector
	public GameObject cell;
	public GameObject newGameButton;
	public GameObject newGameButtonSmall;
	public GameObject startLevelUI;
	public Animator gameOverAnimator;
	public Text levelLabel;
	public Text timerLabel;
	public float effectTime;
	public bool center;
	
	//not visible in the inspector
	[HideInInspector]
	public int x;
	[HideInInspector]
	public int y;
	[HideInInspector]
	public List<Sprite> images;
	
	GridLayoutGroup grid;
	RectTransform rect;
	
	GameObject flippedTile;
	int pairsLeft;
	bool timer;
	float time;
	bool levelUpdated;

	void Start () {
		//check if there are levels
		if(useLevels){
			//get the current level
			int level = PlayerPrefs.GetInt("Memory Puzzle Level");
			
			//get size and images based on the level
			x = levels[level].x;
			y = levels[level].y;
			images = levels[level].images;
			
			//if we're using levels, change the 'new game' button into a label that displays the level
			newGameButtonSmall.transform.GetChild(0).GetComponent<Text>().text = "LEVEL " + (level + 1);
			newGameButtonSmall.GetComponent<Button>().enabled = false;
			newGameButton.transform.GetChild(0).GetComponent<Text>().text = "NEXT LEVEL";
			levelLabel.text = "LEVEL " + (level + 1);
			
			//open the start screen for this level
			openLevelUI();
		}
		
		//make sure the number of tiles is not odd to make the pairs
		if((x * y) % 2 == 1)
			Debug.LogError("Odd number of tiles");
		
		//get grid and transform components
		grid = GetComponent<GridLayoutGroup>();
		rect = GetComponent<RectTransform>();
		
		//width of each tile
		float width = rect.rect.width/x - ((x - 1) * grid.spacing.x/x) - (((float)grid.padding.left + (float)grid.padding.right)/x);
		//assign the cell/tile size
		grid.cellSize = new Vector2(width, width);
		
		//change the top padding to center the grid
		if(center)
			grid.padding.top = (int)((rect.rect.height/2f) - ((y * (width + grid.spacing.y))/2f));
		
		//set pairs left to the total amount of pairs
		pairsLeft = (x * y)/2;
		
		//load the grid of tiles
		StartCoroutine(loadGrid());
	}
	
	void Update(){
		//keep track of the time
		if(timer)
			time += Time.deltaTime;
		
		//show time
		timerLabel.text = time.ToString("f1");
	}
	
	public void tileFlipped(GameObject tile){
		//start timer when the first tile was flipped
		timer = true;
		
		//if there is another flipped tile, check if they're the same
		if(flippedTile != null){
			StartCoroutine(checkTiles(tile, flippedTile));
			flippedTile = null;
		}
		//else, this is the flipped tile
		else{
			flippedTile = tile;
		}
	}
	
	public void done(){
		//stop the timer and show the game over panel
		timer = false;
		gameOverAnimator.SetBool("game over", true);
		
		//change the level if we're using levels
		if(useLevels && !levelUpdated){
			PlayerPrefs.SetInt("Memory Puzzle Level", PlayerPrefs.GetInt("Memory Puzzle Level") + 1);
			levelUpdated = true;
		}
	}
	
	//open the start level panel
	public void openLevelUI(){
		startLevelUI.SetActive(true);
		Time.timeScale = 0;
	}
	
	//start the level
	public void startLevel(){
		startLevelUI.SetActive(false);
		Time.timeScale = 1;
	}
	
	IEnumerator checkTiles(GameObject tile1, GameObject tile2){
		//get tile scripts and animators for both tiles
		MemoryTile firstTile = tile1.GetComponent<MemoryTile>();
		MemoryTile secondTile = tile2.GetComponent<MemoryTile>();
		Animator firstAnimator = tile1.GetComponent<Animator>();
		Animator secondAnimator = tile2.GetComponent<Animator>();
		
		//if the tiles belong to the same pair
		if(firstTile.pairIndex == secondTile.pairIndex){
			//one pair less
			pairsLeft--;
			
			//remove both tiles
			firstAnimator.SetBool("remove tile", true);
			secondAnimator.SetBool("remove tile", true);
			
			//wait until the effect has ended and destroy the tiles
			yield return new WaitForSeconds(1);
			
			Destroy(tile1);
			Destroy(tile2);
		}
		else{
			//if they're not the same pair flip them back
			firstAnimator.SetBool("image visible", false);
			secondAnimator.SetBool("image visible", false);
		}
		
		//end game if there are no pairs left
		if(pairsLeft == 0)
			done();
	}
	
	IEnumerator loadGrid(){
		//get the amount of cells/tiles
		int cellAmount = x * y;
		
		//make sure there are enough images before loading the level
		if(cellAmount/2 > images.Count){
			Debug.LogError("Not enough images");
			yield break;
		}
		
		//get the images in random order
		List<Sprite> randomSprites = getRandomSprites(cellAmount/2);
		//make a new list for the cells
		List<GameObject> newCells = new List<GameObject>();
		
		//go through all pairs
		for(int i = 0; i < cellAmount/2; i++){
			//add two cells for each pair
			for(int I = 0; I < 2; I++){
				//create a new cell(that contains a tile) and find the tile
				GameObject newCell = Instantiate(cell);
				GameObject newTile = newCell.transform.GetChild(0).gameObject;
				
				//set the pair index, assign the image and add it to the new cells list
				newTile.GetComponent<MemoryTile>().pairIndex = i;
				newTile.transform.GetChild(1).GetComponent<Image>().sprite = randomSprites[i];
				newCells.Add(newCell);
				
				//set the cell inactive for now, so it doesn't play an animation (it's not at a random position yet)
				newCell.SetActive(false);
			}
		}
		
		//go through all cells
		for(int i = 0; i < cellAmount; i++){
			//get a random cell
			int random = Random.Range(0, newCells.Count);
			
			//activate the cell and parent it to the grid
			newCells[random].SetActive(true);
			newCells[random].transform.SetParent(transform, false);
			
			//remove the cell from the list so it won't be activated again and wait a moment
			newCells.RemoveAt(random);
			yield return new WaitForSeconds(effectTime/(float)cellAmount);
		}
	}
	
	public List<Sprite> getRandomSprites(int pairs){
		//make a new list for the randomly ordered sprites
		List<Sprite> sprites = new List<Sprite>();
		
		//for all pairs, add a random sprite to the list
		for(int i = 0; i < pairs; i++){
			int random = Random.Range(0, images.Count);
			sprites.Add(images[random]);
			images.RemoveAt(random);
		}
		
		//return the list of random sprites
		return sprites;
	}
	
	public void newGame(){
		//load the current scene to start a new game
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	public void quit(){
		//quit the game
		Application.Quit();
	}
}
