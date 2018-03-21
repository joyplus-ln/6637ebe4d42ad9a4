using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class puzzle{
	public Texture2D image;
	public int size;
}

[System.Serializable]
public class tile{
	public GameObject originalBaseTile;
	public GameObject mainTile;
	public int number;
}

public class SlidingPuzzle : MonoBehaviour {
	
	[HideInInspector]
	public List<puzzle> puzzles;
	
	//visible in the inspector
	public Vector3 dragTileScale;
	public float spawnEffectTime;
	public Color borderEffectColor;
	public bool displayNumbers;
	public bool slide;
	public bool centerVertically;
	public bool mouseInput;
	
	[Space(5)]
	public GameObject baseTile;
	public GameObject mainTile;
	public Transform tilesPanel;
	public Text levelLabel;
	public Animator animator;
	
	//not visible in the inspector
	List<GameObject> baseTiles = new List<GameObject>();
	List<tile> tiles = new List<tile>();
	
	int size;
	int totalLength;
	float cellWidth;
	Texture2D sourceImage;
	bool canMove;
	int emptyBaseTileIndex;
	Vector2 clickedMainTileStartPosition;
	RectTransform mainTileRect;
	
	GridLayoutGroup grid;
	RectTransform rect;
	
	GameObject clickedMainTile;
	GameObject closestBaseTile;
	Vector2 startDragPosition;

	void Start () {
		//get the grid and transform components
		grid = GetComponent<GridLayoutGroup>();
		rect = GetComponent<RectTransform>();
		
		//get the level and show the level label
		int level = PlayerPrefs.GetInt("Sliding Puzzle Level");
		levelLabel.text = "- Level " + (level + 1) + " -";
		
		//get the puzzle size and image
		size = puzzles[level].size;
		sourceImage = puzzles[level].image;
		totalLength = size * size;
		
		//get the width of one cell and assign it
		cellWidth = rect.rect.width/size - ((size - 1) * grid.spacing.x/size) - (((float)grid.padding.left + (float)grid.padding.right)/size);
		grid.cellSize = new Vector2(cellWidth, cellWidth);
		
		//use the top padding to center the grid
		if(centerVertically)
			grid.padding.top = (int)((rect.rect.height/2f) - ((size * (cellWidth + grid.spacing.y))/2f));
		
		//create the base tiles (the tiles that don't move)
		StartCoroutine(addBaseTiles());
	}
	
	void Update(){
		//return if we're using touch input and the player doesn't touch the screen
		if(!mouseInput && Input.touchCount == 0)
			return;
		
		//get the mouse position if we're using mouse input and get the touch position if we're using touch input
		Vector2 input = Vector2.zero;
		
		if(mouseInput){
			input = Input.mousePosition;
		}
		else{
			input = Input.GetTouch(0).position;
		}
		
		//when the player starts touching the screen
		if((Input.GetMouseButtonDown(0) && mouseInput) || (!mouseInput && Input.GetTouch(0).phase == TouchPhase.Began)){
			//the clicked tiles
			clickedMainTile = null;
			closestBaseTile = null;
			
			//store the start touch position 
			startDragPosition = input;
			float closestDistance = Mathf.Infinity;
			
			//go through all base tiles to find the one closest to the touch position
			foreach(GameObject baseTile in baseTiles){
				float distance = Vector3.Distance(input, baseTile.transform.position);
				if(distance < closestDistance){
					closestDistance = distance;
					
					//only valid if the touch position is close enough to the tile (to make sure we can only drag a tile if we actually touch it)
					if(distance < (cellWidth/1.2f))
						closestBaseTile = baseTile;
				}
			}
			
			//find the main tile(the actual tile that contains the image) that has the same position as the closest base tile
			for(int i = 0; i < tiles.Count; i++){
				if(closestBaseTile && tiles[i].mainTile.GetComponent<RectTransform>().anchoredPosition == closestBaseTile.GetComponent<RectTransform>().anchoredPosition)
					clickedMainTile = tiles[i].mainTile;
			}
			
			//the position of the empty spot in the grid
			Vector2 currentEmptyPosition = Vector2.zero;
			
			//make sure we have both the base and the main tile and find the empty position
			if(closestBaseTile != null && clickedMainTile != null)
				currentEmptyPosition = emptyPosition();
			
			//if we didn't have a tile or the empty position doesn't exist, make sure the tile can't move
			if(clickedMainTile == null || currentEmptyPosition == Vector2.zero || !slide){
				canMove = false;
				
				//if we don't want to slide the tile and just touch, set the tile position to the empty position
				if(clickedMainTile != null && currentEmptyPosition != Vector2.zero){
					clickedMainTile.GetComponent<RectTransform>().anchoredPosition = currentEmptyPosition;
					
					//end the game if all tiles are in place
					if(done())
						StartCoroutine(endGame());
				}
			}
			else{
				//get the rect transform
				mainTileRect = clickedMainTile.GetComponent<RectTransform>();
				//disable the animator for this tile to be able to scale the tile
				clickedMainTile.GetComponent<Animator>().enabled = false;
				//set the temporary scale
				mainTileRect.localScale = dragTileScale;
				//set the start position to the current tile position
				clickedMainTileStartPosition = mainTileRect.anchoredPosition;
				//we can now move the tile
				canMove = true;
			}
		}
		
		//if we're sliding instead of just touching and the tile can move...
		if(((Input.GetMouseButton(0) && mouseInput) || (!mouseInput && Input.GetTouch(0).phase == TouchPhase.Moved)) && canMove){
			//get the x and y distance between the current touch position and the start touch position
			float xDistance = Mathf.Abs(input.x - startDragPosition.x);
			float yDistance = Mathf.Abs(input.y - startDragPosition.y);
			
			//if the empty tile index is 1 or -1 (the empty spot is left or right from the touched tile)
			if(emptyBaseTileIndex == 1 || emptyBaseTileIndex == -1){
				//make sure we slide left/right and not up/down
				if(xDistance > yDistance){
					//check if we can drag left or right
					bool canDragRight = (input.x >= startDragPosition.x && input.x <= startDragPosition.x + cellWidth + grid.spacing.x);
					bool canDragLeft = (input.x <= startDragPosition.x && input.x >= (startDragPosition.x - cellWidth) - grid.spacing.x);
					
					//if the empty spot is on the right and we can drag right or the empty spot is on the left and we can drag left
					if((emptyBaseTileIndex == 1 && canDragRight) || (emptyBaseTileIndex == -1 && canDragLeft))
						//move the touched tile
						mainTileRect.anchoredPosition = clickedMainTileStartPosition + new Vector2(input.x - startDragPosition.x, 0);
				}					
			}
			else{
				//if the empty spot is up/down and the drag direction is up/down as well...
				if(yDistance > xDistance){
					//check if we can drag up or down
					bool canDragUp = (input.y >= startDragPosition.y && input.y <= startDragPosition.y + cellWidth + grid.spacing.y);
					bool canDragDown = (input.y <= startDragPosition.y && input.y >= (startDragPosition.y - cellWidth) - grid.spacing.y);
					
					//if the empty spot is above the touched tile and we can drag up or the empty spot is beneath the touched tile and we can drag down
					if((emptyBaseTileIndex == size && canDragDown) || (emptyBaseTileIndex == -size && canDragUp))
						//move the touched tile
						mainTileRect.anchoredPosition = clickedMainTileStartPosition + new Vector2(0, input.y - startDragPosition.y);
				}	
			}
		}
		
		//if we release the screen and the tile was moved
		if(((Input.GetMouseButtonUp(0) && mouseInput) || (!mouseInput && Input.GetTouch(0).phase == TouchPhase.Ended)) && canMove){
			//enable the animator again
			clickedMainTile.GetComponent<Animator>().enabled = true;
			//get current tile position
			Vector2 tilePosition = mainTileRect.anchoredPosition;
			
			float closestDistance = Mathf.Infinity;
			Vector2 snapPosition = Vector2.zero;
			
			//find the closest base tile
			foreach(GameObject baseTile in baseTiles){
				float distance = Vector2.Distance(tilePosition, baseTile.GetComponent<RectTransform>().anchoredPosition);
				if(distance < closestDistance){
					closestDistance = distance;
					snapPosition = baseTile.GetComponent<RectTransform>().anchoredPosition;
				}
			}
			
			//snap to the closest base tile position
			mainTileRect.anchoredPosition = snapPosition;
			//reset the tile scale
			mainTileRect.localScale = Vector3.one;
			
			//end game if all tiles are in place
			if(done())
				StartCoroutine(endGame());
		}
	}
	
	//find the empty position
	public Vector2 emptyPosition(){
		//get touched tile index and surrounding indexes
		int baseTileIndex = baseTiles.IndexOf(closestBaseTile);
		int[] surroundingBaseTileIndexes = {1, -1, size, -size};
		
		//for all tiles around the touched tile
		for(int i = 0; i < 4; i++){
			//get the tile position and the index of the other tile
			GameObject baseTileToCheck = null;
			int index = baseTileIndex + surroundingBaseTileIndexes[i];
			Vector2 closestBaseTilePosition = closestBaseTile.GetComponent<RectTransform>().anchoredPosition;
			
			//if the other tile exists and touches the clicked tile
			if(index < baseTiles.Count && index >= 0 && !(i < 2 && closestBaseTilePosition.y != baseTiles[index].GetComponent<RectTransform>().anchoredPosition.y))
				//check this tile
				baseTileToCheck = baseTiles[index];
			
			//if the spot is empty (it doesn't have a tile with an image), return its position
			if(baseTileToCheck && !hasMainTile(baseTileToCheck)){
				emptyBaseTileIndex = surroundingBaseTileIndexes[i];
				return baseTileToCheck.GetComponent<RectTransform>().anchoredPosition;
			}
		}
		
		//if there is no empty spot, return zero
		return Vector2.zero;
	}
	
	//check if the base tile has a moveable tile (to find out if the spot is empty)
	public bool hasMainTile(GameObject baseTile){
		//go through all tiles to see if one of them has the same position as the base tile
		for(int i = 0; i < tiles.Count; i++){
			if(tiles[i].mainTile.GetComponent<RectTransform>().anchoredPosition == baseTile.GetComponent<RectTransform>().anchoredPosition)
				return true;
		}
		
		//return false if we didn't find a corresponding tile
		return false;
	}
	
	//add the base tiles
	IEnumerator addBaseTiles(){		
	    //create all base tiles and add them to the list of tiles
		for(int i = 0; i < totalLength; i++){
			GameObject newBaseTile = Instantiate(baseTile);
			newBaseTile.transform.SetParent(transform, false);
			
			baseTiles.Add(newBaseTile);
		}
		
		//wait before adding the actual tiles
		yield return 0;
		//try adding the tiles
		addTiles();
		
		//if the current version is not solvable, try adding the tiles again
		while(!solvable()){
			addTiles();
		}
		
		//for all tiles - 1... the last tile should be the empty spot
		for(int I = 0; I < totalLength - 1; I++){
			//wait a moment for the spawn effect
			yield return new WaitForSeconds(spawnEffectTime/(float)totalLength);
			
			//disable the number label if we don't want to give away the index
			if(!displayNumbers)
				tiles[I].mainTile.transform.GetChild(0).gameObject.SetActive(false);
			
			//set border effect colors and enable the main tile
			tiles[I].mainTile.transform.GetChild(1).GetComponent<Image>().color = borderEffectColor;
			tiles[I].mainTile.transform.GetChild(2).GetComponent<Image>().color = borderEffectColor;
			tiles[I].mainTile.SetActive(true);
		}
	}
	
	public void addTiles(){
		//clear the current tiles before continuing
		if(tiles.Count > 0)
			clearAll();
		
		//tile position(index in the grid)
		int position = 0;
		
		//list of tile positions 
		List<int> positions = new List<int>();
		for(int j = 0; j < totalLength - 1; j++){
			positions.Add(j);
		}
		
		//create a list of random positions by shuffling the list of positions
		List<int> randomPositions = new List<int>();
		for(int J = 0; J < totalLength - 1; J++){
			int random = Random.Range(0, positions.Count);
			randomPositions.Add(positions[random]);
			positions.RemoveAt(random);
		}
		
		//go throught all positions in the grid
		for(int i = 0; i < size; i++){
			for(int I = 0; I < size; I++){
				//return if this is the last tile
				if(i == size - 1 && I == size - 1)
					return;
				
				//create the tile, assign the text for the label and change the tile name
				GameObject newTile = Instantiate(mainTile);
				newTile.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = (position + 1) + "";
				newTile.name = position + "";
				
				//get the transform and set it's size to the cell size
				RectTransform rect = newTile.GetComponent<RectTransform>();
				rect.sizeDelta = new Vector2(cellWidth, cellWidth);
				
				//set position to the base tile position and parent the tile to the tile panel
				rect.anchoredPosition = baseTiles[randomPositions[position]].GetComponent<RectTransform>().anchoredPosition;
				newTile.transform.SetParent(tilesPanel, false);
				
				//get the image component
				Image tileImage = newTile.GetComponent<Image>();
				//get the part of the original image that should be displayed on this tile
				Rect spriteRect = new Rect(I * (sourceImage.width/size), (size - i - 1) * (sourceImage.height/size), sourceImage.width/size, sourceImage.height/size);
				//create the new image for this tile
				Sprite tileSprite = Sprite.Create(sourceImage, spriteRect, new Vector2(0, 0), 100.0f);
				//assign the new image
				tileImage.sprite = tileSprite;
				//disable the tile for now
				newTile.SetActive(false);
				//add new tile to the tile list
				tiles.Add(new tile{originalBaseTile = baseTiles[position], mainTile = newTile, number = (position + 1)});
				
				position++;
			}
		}
	}
	
	//clear all tiles in the tile list
	public void clearAll(){		
		for(int i = 0; i < totalLength - 1; i++){
			Destroy(tiles[i].mainTile);
		}
		
		tiles.Clear();
	}
	
	public bool solvable(){
		//create a new list for the numbers
		List<int> numbers = new List<int>();
		
		//for each tile...
		for(int i = 0; i < totalLength; i++){
			for(int I = 0; I < totalLength - 1; I++){
				if(tiles[I].mainTile.GetComponent<RectTransform>().anchoredPosition == baseTiles[i].GetComponent<RectTransform>().anchoredPosition){
					//add this tile number
					numbers.Add(tiles[I].number);
				}
			}
		}
		
		int inversions = 0;
		
		//count the number of inversions
		for(int j = 0; j < numbers.Count; j++){
			for(int J = j + 1; J < numbers.Count; J++){
				if(numbers[J] < numbers[j])
					inversions++;
			}
		}
		
		//if the number of inversions is odd, we don't have a solvable puzzle
		if(inversions % 2 == 1){
			return false;
		}
		
		//otherwise the puzzle is solvable
		return true;
	}
	
	public bool done(){
		//for all tiles
		for(int i = 0; i < tiles.Count; i++){
			//get the recttransform components for both tiles
			RectTransform baseTileRect = tiles[i].originalBaseTile.GetComponent<RectTransform>();
			RectTransform mainTileRect = tiles[i].mainTile.GetComponent<RectTransform>();
			
			//if the main tile is not at the same position as the corresponding base tile, we're not done yet
			if(baseTileRect.anchoredPosition != mainTileRect.anchoredPosition)
				return false;
		}
		
		//open the next level and return true
		PlayerPrefs.SetInt("Sliding Puzzle Level", PlayerPrefs.GetInt("Sliding Puzzle Level") + 1);
		return true;
	}
	
	public void reload(){
		//load the current scene
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	IEnumerator endGame(){
		//show the game end animation
		animator.SetBool("end game", true);
		
		//wait for the animation
		yield return new WaitForSeconds(0.2f);
		//get the spacing
		Vector2 originalSpacing = grid.spacing;
		
		//set the spacing to 0 to join the image parts
		grid.spacing = Vector2.zero;
		//get the new size for the cells (without spacing)
		float cellWidthWithoutSpacing = rect.rect.width/size - (((float)grid.padding.left + (float)grid.padding.right)/size);
		//assign the new width
		grid.cellSize = new Vector2(cellWidthWithoutSpacing, cellWidthWithoutSpacing);
		
		//for all tiles except the last one
		for(int i = 0; i < totalLength - 1; i++){
			//get the rect transform component for the main tile and adjust position and size
			RectTransform mainRect = tiles[i].mainTile.GetComponent<RectTransform>();
			mainRect.anchoredPosition = tiles[i].originalBaseTile.GetComponent<RectTransform>().anchoredPosition;
			mainRect.sizeDelta += originalSpacing;
			
			//disable all labels on the image
			for(int I = 0; I < 3; I++){
				tiles[i].mainTile.transform.GetChild(I).gameObject.SetActive(false);
			}
			//show the final effect 
			tiles[i].mainTile.GetComponent<Animator>().SetBool("effect", true);
		}
		
		//load the last tile to complete the image
		loadLastTile(originalSpacing);
	}
	
	public void loadLastTile(Vector2 originalSpacing){
		//create the last tile, assign the right size and set its position
		GameObject newTile = Instantiate(mainTile);
		RectTransform rect = newTile.GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(cellWidth, cellWidth);
		rect.sizeDelta += originalSpacing;
		rect.anchoredPosition = baseTiles[totalLength - 1].GetComponent<RectTransform>().anchoredPosition;
		newTile.transform.SetParent(tilesPanel, false);
		
		//get the image component, calculate the part of the image that should be visible and assign the new sprite
		Image tileImage = newTile.GetComponent<Image>();
		Rect spriteRect = new Rect((size - 1) * (sourceImage.width/size), 0, sourceImage.width/size, sourceImage.height/size);
		Sprite tileSprite = Sprite.Create(sourceImage, spriteRect, new Vector2(0, 0), 100.0f);
		tileImage.sprite = tileSprite;
		
		//disable the labels
		for(int i = 0; i < 3; i++){
			newTile.transform.GetChild(i).gameObject.SetActive(false);
		}
		
		//show the effect
		newTile.GetComponent<Animator>().SetBool("effect", true);
	}
}
