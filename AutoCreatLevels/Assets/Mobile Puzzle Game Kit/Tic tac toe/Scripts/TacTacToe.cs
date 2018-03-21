using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TacTacToe : MonoBehaviour {
	
	//variables visible in the inspector
	public GameObject cell;
	public float effectTime;
	public Sprite player1;
	public Sprite player2;
	public Color spriteColor;
	public Color markColor;
	public float markEffectTime;
	
	//not visible in the inspector
	GridLayoutGroup grid;
	RectTransform rect;
	
	GameObject playerTurnLabel;
	Text winLabel;
	Text timeLabel;
	
	bool player1Turn = true;
	bool turn;
	bool gaming;
	
	float time;
	
	int[] cellStates = new int[9];

	void Start () {
		//get the grid and transform components
		grid = GetComponent<GridLayoutGroup>();
		rect = GetComponent<RectTransform>();
		
		//find some text objects
		playerTurnLabel = GameObject.Find("player turn text");
		winLabel = GameObject.Find("win text").GetComponent<Text>();
		timeLabel = GameObject.Find("game time label").GetComponent<Text>();
		
		//get the cell width and apply it to the grid component
		float cellWidth = rect.rect.width/3 - (2 * grid.spacing.x/3) - (((float)grid.padding.left + (float)grid.padding.right)/3f);
		grid.cellSize = new Vector2(cellWidth, cellWidth);
		
		//load the grid
		StartCoroutine(loadGrid());
	}
	
	void Update(){
		//if the label that displays the player should be turned, rotate it
		if(turn)
			playerTurnLabel.transform.Rotate(Vector3.right * Time.deltaTime * 720);
		
		//keep track of the time
		if(gaming)
			time += Time.deltaTime;
	}
	
	IEnumerator loadGrid(){
		//for all cells... (3 x 3)
		for(int i = 0; i < 9; i++){
			//create a new cell and parent it to this object
			GameObject newCell = Instantiate(cell);
			newCell.transform.SetParent(transform, false);
			
			//name the cell and make sure the cell action starts when we click it
			newCell.name = "" + i;
			newCell.GetComponent<Button>().onClick.AddListener(() => { this.cellAction(newCell, int.Parse(newCell.name)); });
			
			//get the cell image and disable it
			GameObject image = newCell.transform.GetChild(0).gameObject;
			image.SetActive(false);
			
			//wait a moment for a nice effect
			yield return new WaitForSeconds(effectTime/9f);
		}
	}
	
	//when the player touches one of the cells
	public void cellAction(GameObject cell, int cellIndex){
		//check if this cell is empty
		if(cellStates[cellIndex] != 0)
			return;
		
		//start counting
		gaming = true;
		
		//get the image object and component
		GameObject image = cell.transform.GetChild(0).gameObject;
		Image imageComponent = image.GetComponent<Image>();
		
		//set the image color and enable the image
		imageComponent.color = spriteColor;
		image.SetActive(true);
		
		//check who pressed the cell, change cell index, sprite and label text
		if(player1Turn){
			cellStates[cellIndex] = 1;
			imageComponent.sprite = player1;
			playerTurnLabel.GetComponent<Text>().text = "Player two's turn";
		}
		else{
			cellStates[cellIndex] = 2;
			imageComponent.sprite = player2;
			playerTurnLabel.GetComponent<Text>().text = "Player one's turn";
		}
		
		//change the turn
		player1Turn = !player1Turn;
		//rotate the label
		StartCoroutine(turnlabelEffect());
		//check if there's a winner
		checkCells();
	}
	
	public void checkCells(){
		//check the horizontal and vertical rows
		for(int i = 0; i < 3; i++){
			if(cellStates[i] != 0 && cellStates[i] == cellStates[i + 3] && cellStates[i] == cellStates[i + 6]){
				StartCoroutine(markSprites(i, i + 3, i + 6));
				gameWon(cellStates[i]);
				return;
				
			}
			else if(cellStates[i * 3] != 0 && cellStates[i * 3] == cellStates[i * 3 + 1] && cellStates[i * 3] == cellStates[i * 3 + 2]){
				StartCoroutine(markSprites(i * 3, i * 3 + 1, i * 3 + 2));
				gameWon(cellStates[i * 3]);
				return;
			}
		}
		
		//check the diagonal rows
		if(cellStates[0] != 0 && cellStates[0] == cellStates[4] && cellStates[0] == cellStates[8]){
			StartCoroutine(markSprites(0, 4, 8));
			gameWon(cellStates[0]);
			return;
		}
		else if(cellStates[2] != 0 && cellStates[2] == cellStates[4] && cellStates[2] == cellStates[6]){
			StartCoroutine(markSprites(2, 4, 6));
			gameWon(cellStates[2]);
			return;
		}
		
		//check the entire board to see if it's a draw
		for(int i = 0; i < 9; i++){
			if(cellStates[i] == 0)
				return;
		}
		
		//if all cells are filled and nobody won, it's a draw
		gameWon(0);
	}
	
	public void gameWon(int player){
		//stop counting
		gaming = false;
		
		//change the main label so it shows who won
		if(player != 0){
			winLabel.text = "PLAYER " + player + " WINS";
		}
		else{
			winLabel.text = "DRAW";
		}
		
		//get the minutes and seconds and display them on the time label
		int minutes = Mathf.FloorToInt(time/60f);
		int seconds = Mathf.FloorToInt(time - minutes * 60);
		string timeText = string.Format("{0:0}:{1:00}", minutes, seconds);
		timeLabel.text = "Time: " + timeText;
		
		//end the game
		StartCoroutine(endGame());
	}
	
	public void restart(){
		//reload the current scene
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	//colors the sprites when somebody won the game
	IEnumerator markSprites(int sprite1, int sprite2, int sprite3){
		//get the image components of the cells that should be colored
		Image[] sprites = new Image[3];
		sprites[0] = GameObject.Find("" + sprite1).transform.GetChild(0).GetComponent<Image>();
		sprites[1] = GameObject.Find("" + sprite2).transform.GetChild(0).GetComponent<Image>();
		sprites[2] = GameObject.Find("" + sprite3).transform.GetChild(0).GetComponent<Image>();
		
		//for each of the 3 images, change color and wait a moment
		foreach(Image sprite in sprites){
			yield return new WaitForSeconds(markEffectTime/3);
			sprite.color = markColor;
		}
	}
	
	IEnumerator turnlabelEffect(){
		//turn the object for 0.5 seconds and reset rotation
		turn = true;
		yield return new WaitForSeconds(0.5f);
		turn = false;
		playerTurnLabel.transform.rotation = Quaternion.identity;
	}
	
	IEnumerator endGame(){
		//wait for the effect and play the game over animation
		yield return new WaitForSeconds(markEffectTime + 0.5f);
		transform.parent.GetComponent<Animator>().SetBool("game over", true);
	}
}
