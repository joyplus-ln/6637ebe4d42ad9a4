using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public class category{
	public string name;
	
	//only when using the text file
	public TextAsset words;
	
	public List<wordPuzzle> wordPuzzles;
}

[System.Serializable]
public class wordPuzzle{
	public int size;
	public List<string> words;
	
	//only when using the text file
	public int numberOfWords;
}

public class characterCell{
	public Text label;
	public string character;
	public int x, y;
}

public class wordData{
	public string word;
	public bool found;
	public int startPosition, endPosition;
	public GameObject labelCheckmark;
}

public class WordSearch : MonoBehaviour {
	
	[HideInInspector]
	public List<category> categories;
	
	//variables visible in the inspector
	public GameObject characterCell;
	public GameObject categoryButton;
	public GameObject wordLabel;
	public Transform categoryList;
	public Transform wordLabelList;
	public CanvasGroup startLevelUI;
	public float effectTime;
	public bool center;
	public float fadespeed;
	public Text levelLabel;
	public int triesBeforeResetting;
	public bool wordEffects;
	public Text wordEffectLabel;
	public float newWordEffectTime;
	public float wordEffectSpeed;
	public Animator endPanelAnimator;
	
	//not visible in the inspector
	//only when using the text file
	[HideInInspector]
	public bool textFileWords;
	[Tooltip("When selecting words from the text file, should it use words with the same length as the grid size? Including these words might result in 0 valid puzzles")]
	public bool includeWordsWithSameLength;
	
	[HideInInspector]
	public bool[] enabledDirections = new bool[8] ; 
	
	GridLayoutGroup grid;
	RectTransform rect;
	
	//entire alphabet for filling the left over character cells
	string[] alphabet = new string[]{"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
	
	[HideInInspector]
	public List<characterCell> characters = new List<characterCell>();
	[HideInInspector]
	public float width;
	[HideInInspector]
	public Color lastColor;
	
	int size;
	int category;
	List<string> words;
	bool fade;
	int tries;
	int resetCount;
	List<wordData> allWordData = new List<wordData>();
	Coroutine wordEffect;
	bool levelDone;
	
	int[] possibleDirections;

	IEnumerator Start () {			
		//get the grid and transform components
		grid = GetComponent<GridLayoutGroup>();
		rect = GetComponent<RectTransform>();
		
		//don't yet show the word effect
		wordEffectLabel.gameObject.SetActive(false);
		
		//if we had selected a category already, skip the categories and open the level
		if(PlayerPrefs.GetInt("category") != 0){
			startLevel(PlayerPrefs.GetInt("category") - 1);
			yield break;
		}
		
		//for all categories
		for(int i = 0; i < categories.Count; i++){
			//create a category button, parent it to the category list and change its text
			GameObject newCategory = Instantiate(categoryButton);
			newCategory.transform.SetParent(categoryList, false);
			newCategory.transform.GetChild(0).GetComponent<Text>().text = categories[i].name + " (lvl " + (PlayerPrefs.GetInt(i + "Word Search Level") + 1) + ")";
			
			//change the button name
			newCategory.transform.name = "" + i;
			//make sure the button starts a level on click
			newCategory.GetComponent<Button>().onClick.AddListener(() => { startLevel(int.Parse(newCategory.transform.name)); });
			
			//wait a moment for a nice effect
			yield return new WaitForSeconds(effectTime/(float)categories.Count);
		}
	}
	
	void Update(){
		//if the start UI is fading, decrease its alpha
		if(fade && startLevelUI.alpha > 0){
			startLevelUI.alpha -= Time.deltaTime * fadespeed;
		}
		//else, stop fading and disable the start UI
		else if(fade){
			fade = false;
			startLevelUI.gameObject.SetActive(false);
		}
		
		//if the word effect is visible, move it up
		if(wordEffectLabel.gameObject.activeSelf)
			wordEffectLabel.gameObject.transform.Translate(Vector2.up * Time.deltaTime * wordEffectSpeed);
	}
	
	public void startLevel(int categoryIndex){
		//assign the category
		category = categoryIndex;
		
		//get the current level
		int level = PlayerPrefs.GetInt(category + "Word Search Level");	
		
		//if the level does not exist return to the category menu
		if(level == categories[category].wordPuzzles.Count){
			newGame(true);
			return;
		}
		
		//get the grid size
		size = categories[category].wordPuzzles[level].size;
		
		if(!textFileWords){
			//if we're not selecting words from a text file, just use the words that belong to this level
			words = categories[category].wordPuzzles[level].words;	
		}
		else{
			//if we do use a text file, get all words from the file
			List<string> allWordsInFile = (categories[category].words.text.Split(' ')).ToList();
			
			//list of randomly picked words
			List<string> randomWords = new List<string>();
			
			//for the amount of words we want in the puzzle...
			for(int i = 0; i < categories[category].wordPuzzles[level].numberOfWords; i++){
				//if there are 0 words in the file, show an error and return
				if(allWordsInFile.Count == 0){
					if(includeWordsWithSameLength){
						Debug.LogError("Not enough words in text file of category '" + categories[category].name + "'. Please make sure to add at least " + categories[category].wordPuzzles[level].numberOfWords + " words with a maximum of " + size + " characters.");
					}
					else{
						Debug.LogError("Not enough words in text file of category '" + categories[category].name + "'. Please make sure to add at least " + categories[category].wordPuzzles[level].numberOfWords + " words with a maximum of " + (size - 1) + " characters.");
					}
					
					return;
				}
				
				//get a random word index
				int random = Random.Range(0, allWordsInFile.Count);
				
				//if the word has the right length, add it to the random words and remove it from the list of available words
				if((allWordsInFile[random].Length <= size && includeWordsWithSameLength) || allWordsInFile[random].Length < size){
					randomWords.Add(allWordsInFile[random]);
					allWordsInFile.RemoveAt(random);
				}
				else{
					//if it doesn't have the right lenght, remove it from the list of available words
					allWordsInFile.RemoveAt(random);
					i--;
				}
			}
			
			//set the list of words to the randomly picked words
			words = randomWords;
		}
		
		//add all words to the list of word data
		for(int i = 0; i < words.Count; i++){
			allWordData.Add(new wordData{word = words[i], found = false});
		}
		//create the labels at the bottom so players can see which words have been found
		StartCoroutine(addWordLabels());
		
		//show the category and level
		levelLabel.text = categories[category].name + " - Level " + (level + 1);
		//collection of the possible word directions
		possibleDirections = new int[]{-size, -size + 1, 1, size + 1, size, size - 1, -1, -size - 1};
		
		//find the cell width and assign it to the grid
		width = rect.rect.width/size - ((size - 1) * grid.spacing.x/size) - (((float)grid.padding.left + (float)grid.padding.right)/size);
		grid.cellSize = new Vector2(width, width);
		
		//change the top padding to center the grid
		if(center)
			grid.padding.top = (int)((rect.rect.height/2f) - ((size * (width + grid.spacing.y))/2f));
		
		//reset the characters for a clear list 
		resetCharacters();
		
		//try adding the words and break the loop if all words have been placed (stop after 3000 tries to make sure it won't freeze at an impossible task)
		while(resetCount < 3000){
			if(placedCharacters())
				break;
		}
		
		//start fading in the grid if all words were added
		if(resetCount < 3000){
			fade = true;
		}
		//if it has been resetted for 3000 times, show an error 
		else{
			Debug.LogError("Could not create puzzle, please allow more directions, add shorter words, increase grid size or use less words");
		}
	}
	
	IEnumerator addWordLabels(){
		//for each word
		for(int i = 0; i < words.Count; i++){
			//create a new label, parent it and show the right word
			GameObject newLabel = Instantiate(wordLabel);
			newLabel.transform.SetParent(wordLabelList, false);
			newLabel.transform.GetChild(0).GetComponent<Text>().text = words[i];
			
			//find the checkmark and disable it
			GameObject checkmark = newLabel.transform.GetChild(1).GetChild(0).gameObject;
			checkmark.SetActive(false);
			
			//assign the checkmark to the word data so we can use it later on the enable the checkmark
			allWordData[i].labelCheckmark = checkmark;
			
			//wait a moment for a nice effect
			yield return new WaitForSeconds(effectTime/(float)words.Count);
		}
	}
	
	//returns true if it succesfully added all characters
	public bool placedCharacters(){
		//for all words
		for(int i = 0; i < words.Count; i++){
			//first try to add the word by finding a corresponding character and placing it there
			if(!addedWordUsingCorrespondingCharacter(words[i], i)){
				//if that doesn't work, try some random places
				while(tries < triesBeforeResetting){
					if(addedWordRandomly(words[i], i))
						break;
					
					//keep track of the amount of tries so we can reset the grid if the word doesn't fit
					tries++;
				}
				
				//if we've tried adding the word several times, reset the grid and start again
				if(tries == triesBeforeResetting){
					resetCharacters();
					tries = 0;
					resetCount++;
					return false;
				}
				
				//reset the number of tries
				tries = 0;
			}
		}
		
		//after adding all words, load the actual grid and return true
		loadGrid();
		return true;
	}
	
	public void resetCharacters(){
		//clear all characters
		characters.Clear();
		//add character cells for the entire grid
		for(int i = 0; i < size; i++){
			for(int I = 0; I < size; I++){
				characters.Add(new characterCell{label = null, character = "", x = I, y = i});
			}
		}
	}
	
	public bool addedWordUsingCorrespondingCharacter(string word, int wordIndex){
		//create a list of corresponding character positions and a list for the indexes in the words where the corresponding character positions are
		List<int> correspondingCharacterPositions = new List<int>();
		List<int> correspondingCharacterPositionsInWords = new List<int>();
		
		//for all characters in this word
		for(int n = 0; n < word.Length; n++){
			//for all character cells
			for(int N = 0; N < size * size; N++){
				//check if the character at index n corresponds with this character cell 
				if(word.Substring(n, 1) == characters[N].character){
					//if it does, add the indexes to the lists
					correspondingCharacterPositions.Add(N);
					correspondingCharacterPositionsInWords.Add(n);
				}
			}
		}
		
		//for each corresponding position in the word, check if the word can be added there using any of the possible directions
		for(int J = 0; J < correspondingCharacterPositions.Count; J++){
			if(validDirection(word, false, correspondingCharacterPositions, correspondingCharacterPositionsInWords, 0, J, wordIndex))
				//if it can be added return true
				return true;
		}
		
		//after checking all corresponding characters, return false and start adding the word at random positions
		return false;
	}
	
	public bool addedWordRandomly(string word, int wordIndex){
		//get a random character position in the grid
		int characterCount = size * size;
		int start = Random.Range(0, characterCount);
		
		//check if the word can be added there using any of the possible directions
		if(validDirection(word, true, null, null, start, 0, wordIndex))
			return true;
		
		//return false if it can't be added at this random position
		return false;
	}
	
	public bool validDirection(string word, bool randomPosition, List<int> correspondingCharacterPositions, List<int> correspondingCharacterPositionsInWords, int start, int J, int wordIndex){
		//create a list of directions
		List<int> directions = new List<int>();
		
		//for all possible directions, only add the ones we want to use
		for(int d = 0; d < 8; d++){
			if(enabledDirections[d])
				directions.Add(possibleDirections[d]);
		}
		
		//shuffle the directions so it starts with a different direction each time
		directions = shuffledDirections(directions);
		
		//go through all directions
		for(int i = 0; i < directions.Count; i++){
			//create a list for the used character positions and for the original characters (to reset them if the word doesn't fit)
			List<int> usedCharacterPositions = new List<int>();
			List<string> originalCharacters = new List<string>();
			
			//for all characters in the word
			for(int I = 0; I < word.Length; I++){	
				//the current position is the word start position + the direction * the character index in this word
				int currentCharacterPosition = start + (I * directions[i]);
				
				//if we're not using a random word position...
				if(!randomPosition){
					//get the corresponding character position and go back or forward to find the current position
					currentCharacterPosition = correspondingCharacterPositions[J];	
					if(I < correspondingCharacterPositionsInWords[J]){
						currentCharacterPosition -= ((correspondingCharacterPositionsInWords[J] - I) * directions[i]);
					}
					else if(I > correspondingCharacterPositionsInWords[J]){
						currentCharacterPosition += ((I - correspondingCharacterPositionsInWords[J]) * directions[i]);
					}
				}
				
				//if this is an existing character, add the character
				if(currentCharacterPosition >= 0 && currentCharacterPosition < size * size){
					originalCharacters.Add(characters[currentCharacterPosition].character);
				}
				else{
					//else, don't add any character
					originalCharacters.Add("");
				}
				
				//check if we can add the character at the current position
				if((randomPosition && (canAddCharacter(word.Substring(I, 1), directions[i], start + (I * directions[i]), start + ((I - 1) * directions[i]), start, I)))
				|| (!randomPosition && (canAddCharacter(word.Substring(I, 1), directions[i], currentCharacterPosition, currentCharacterPosition - directions[i], correspondingCharacterPositions[J] - ((correspondingCharacterPositionsInWords[J] - I) * directions[i]), I)))){
					//if the character can be added, also add it to the list of used character positions
					usedCharacterPositions.Add(currentCharacterPosition);
					
					//if this is the last character in this word
					if(I == word.Length - 1){
						//set the word start position
						if(randomPosition){
							allWordData[wordIndex].startPosition = start;
						}
						else{
							allWordData[wordIndex].startPosition = correspondingCharacterPositions[J] - (correspondingCharacterPositionsInWords[J] * directions[i]);
						}
						
						//set the word end position
						allWordData[wordIndex].endPosition = currentCharacterPosition;
						
						//return true (the word has been added)
						return true;
					}
				}
				else{
					//if we could not add the character, reset all characters in this word
					for(int j = 0; j < usedCharacterPositions.Count; j++){
						characters[usedCharacterPositions[j]].character = originalCharacters[j];
					}
					
					//clear the list of original characters and the list of used character positions
					originalCharacters.Clear();
					usedCharacterPositions.Clear();
					
					//break the loop
					break;
				}
			}
		}
		
		//return false because we could not add the word in this direction
		return false;
	}
	
	public List<int> shuffledDirections(List<int> directions){
		//create a list for the shuffled directions
		List<int> shuffled = new List<int>();
		
		//for all directions
		for(int i = 0; i < directions.Count; i++){
			//get a random directions and add it to the shuffled list of directions
			int random = Random.Range(0, directions.Count);
			shuffled.Add(directions[random]);
			directions.RemoveAt(random);
		}
		
		//return the shuffled list of directions
		return shuffled;
	}
	
	public bool canAddCharacter(string character, int direction, int characterPosition, int previousCharacterPosition, int wordStartPosition, int indexInWord){
		//we can not add the character if it is outside the grid
		if(characterPosition < 0 || characterPosition >= size * size)
			return false;
		
		//we can not add the character if the cell is not empty and not the character that we want to add
		if(characters[characterPosition].character != "" && characters[characterPosition].character != character)
			return false;
		
		//if this is not the first character in the word
		if(indexInWord != 0){
			//for all directions, check if the character is in the same row
			
			//for example, if we have a 6 x 6 grid, the start character has index 5 in the grid and we're trying to add the word horizontally,
			//than index + 1 would be the next row and the word would not be horizontal, so we return false
			if((direction == -size + 1 || direction == size - 1) && characters[characterPosition].y == characters[previousCharacterPosition].y){
				return false;
			}
			else if((direction == 1 || direction == -1) && characters[characterPosition].y != characters[previousCharacterPosition].y){
				return false;
			}
			else if(direction == size + 1 && characters[characterPosition].y != characters[previousCharacterPosition].y + 1){
				return false;
			}
			else if(direction == -size - 1 && characters[characterPosition].y != characters[previousCharacterPosition].y - 1){
				return false;
			}
		}
		
		//if the character is valid, place it and return
		characters[characterPosition].character = character;
		return true;
	}
	
	public void loadGrid(){
		//get the grid size
		int characterCount = size * size;
		
		//for all characters in the grid
		for(int i = 0; i < characterCount; i++){
			//create the character cell, parent it to this object, assign the character label and rename the character cell
			GameObject newCharacter = Instantiate(characterCell);
			newCharacter.transform.SetParent(transform, false);
			characters[i].label = newCharacter.transform.GetChild(0).GetComponent<Text>();
			newCharacter.name = i + "";
			
			//add a random character to the empty cells
			if(characters[i].character == "")
				characters[i].character = alphabet[Random.Range(0, alphabet.Length)];
			//else
				//characters[i].label.color = Color.red;
			
			//show the character
			characters[i].label.text = characters[i].character.ToUpper();
		}
	}
	
	public void drewNewLine(int startPosition, int endPosition, bool removeLine, Vector2 position){
		//return if we already completed this level
		if(levelDone)
			return;
		
		//for all words
		for(int i = 0; i < allWordData.Count; i++){
			//check if this word has the right start/end positions
			if(startPosition != endPosition && (allWordData[i].startPosition == startPosition || allWordData[i].startPosition == endPosition) && (allWordData[i].endPosition == startPosition || allWordData[i].endPosition == endPosition)){
				//if we're not going to remove the line, we found a new word
				if(!removeLine){
					allWordData[i].found = true;
					
					//if we want the word effect and the effect exists
					if(wordEffects){
						if(wordEffect != null){
							//disable the effect
							wordEffectLabel.gameObject.SetActive(false);
							StopCoroutine(wordEffect);
						}
						
						//start a new word effect
						wordEffect = StartCoroutine(newWordEffect(allWordData[i], position));
					}
				}
				else{
					//if we're removing the line, make sure we have to find the word again
					allWordData[i].found = false;
				}
			}
		}
		
		//if we found all words, end this level
		if(done()){
			endLevel();
		}
	}
	
	public bool done(){
		bool allFound = true;
		
		//for all words
		for(int i = 0; i < allWordData.Count; i++){
			//if the word was not found yet, disable the checkmark and return false
			if(!allWordData[i].found){
				allFound = false;
				allWordData[i].labelCheckmark.SetActive(false);
			}
			else{
				//if this word was found, enable the checkmark
				allWordData[i].labelCheckmark.SetActive(true);
			}
		}
		
		return allFound;
	}
	
	public void endLevel(){
		//open the next level
		PlayerPrefs.SetInt(category + "Word Search Level", PlayerPrefs.GetInt(category + "Word Search Level") + 1);
		
		//play the end animation and end the game
		endPanelAnimator.SetBool("done", true);
		levelDone = true;
	}
	
	IEnumerator newWordEffect(wordData data, Vector2 position){
		//get the word
		string word = data.word;
		
		//get the color to use for the effect
		if(lastColor != new Color(0, 0, 0, 0))
			wordEffectLabel.color = lastColor;
		
		//assign the position and text before enabling the effect
		wordEffectLabel.gameObject.GetComponent<RectTransform>().anchoredPosition = position;
		wordEffectLabel.text = word;
		wordEffectLabel.gameObject.SetActive(true);
		
		//wait a moment and disable the effect
		yield return new WaitForSeconds(newWordEffectTime);
		wordEffectLabel.gameObject.SetActive(false);
	}
	
	public void newGame(bool categories){
		//if we want to go back to the categories, save a playerprefs value that tells the game to open the category menu
		if(categories){
			PlayerPrefs.SetInt("category", 0);
		}
		else{
			PlayerPrefs.SetInt("category", category + 1);
		}
		
		//load the current scene
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	public void quit(){
		//quit the application
		Application.Quit();
	}
}
