using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawLinesTouchInput : MonoBehaviour {
	
	//variables visible in the inspector
	public GameObject line;
	public float lineWidth;
	public bool lineWithCellWidth;
	public float lineEndLength;
	public float lineTransparency;
	public bool randomColors;
	
	//variables not visible in the inspector
	WordSearch wordSearch;
	characterCell startCharacterCell;
	characterCell endCharacterCell;
	bool drawing;
	RectTransform currentLine;
	
	List<GameObject> lines = new List<GameObject>();
	
	void Start(){
		//find the main word search script
		wordSearch = FindObjectOfType<WordSearch>();
	}

	void Update () {
		//return if we're not touching the screen
		if(Input.touchCount == 0)
			return;
		
		if(Input.GetTouch(0).phase == TouchPhase.Began){
			//get the closest character (closest to the touch position)
			startCharacterCell = closestCharacter();
			
			//if there is a character cell
			if(startCharacterCell != null){
				//create a new line and parent it to this object
				currentLine = Instantiate(line).GetComponent<RectTransform>();
				currentLine.gameObject.transform.SetParent(transform, false);
				
				//if we're using random colors, apply a nice random color
				if(randomColors){
					Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), lineTransparency);
					currentLine.gameObject.GetComponent<Image>().color = color;
					
					//keep the last color (for the effect when the player finds a new word)
					wordSearch.lastColor = color;
				}
				
				//start drawing the line
				drawing = true;
			}
		}
		//if we're swiping and we're drawing a line
		else if(Input.GetTouch(0).phase == TouchPhase.Moved && drawing){
			//get the character cell closest to the current drag position
			endCharacterCell = closestCharacter();
			
			//if there is a character cell
			if(endCharacterCell != null){
				
				//get the start and the current line position
				Vector2 start = startCharacterCell.label.transform.parent.GetComponent<RectTransform>().anchoredPosition;
				Vector2 current = endCharacterCell.label.transform.parent.GetComponent<RectTransform>().anchoredPosition;
				//find the distance between the start and the current character cell
				float distance = Vector2.Distance(start, current);
				
				if(!lineWithCellWidth){
					currentLine.sizeDelta = new Vector2(distance + (wordSearch.width * lineEndLength), lineWidth);
				}
				else{
					currentLine.sizeDelta = new Vector2(distance + (wordSearch.width * lineEndLength), wordSearch.width);
				}
				
				//set the line position(middle of the line) to the point in between the start and current position
				currentLine.anchoredPosition = new Vector2((start.x + current.x)/2f, (start.y + current.y)/2f);
				
				//change the rotation of the line 
				Vector2 v2 = current - start;
				currentLine.localEulerAngles = new Vector3(currentLine.localEulerAngles.x, currentLine.localEulerAngles.y, Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg);
			}
		}
		else if(Input.GetTouch(0).phase == TouchPhase.Ended){
			//stop drawing if we release the screen
			drawing = false;
			
			//if there are both a start character cell and an end character cell and they're not the same object
			if(startCharacterCell != null && endCharacterCell != null && startCharacterCell != endCharacterCell){
				//tell the word search script we drew a new line
				int startPosition = int.Parse(startCharacterCell.label.gameObject.transform.parent.name);
				int endPosition = int.Parse(endCharacterCell.label.gameObject.transform.parent.name);
				wordSearch.drewNewLine(startPosition, endPosition, false, endCharacterCell.label.transform.parent.GetComponent<RectTransform>().anchoredPosition);
				
				//set the name of this line to the start and end position in case we need it later
				currentLine.gameObject.name = startPosition + "-" + endPosition;
				
				//add the line to the list of lines
				lines.Add(currentLine.gameObject);
			}
			else if(currentLine != null){
				//remove the line
				Destroy(currentLine.gameObject);
			}
			
			//reset the current line
			currentLine = null;
		}
	}
	
	characterCell closestCharacter(){
		//smallest distance and closest character
		float smallestDistance = Mathf.Infinity;
		characterCell closestCharacter = null;
		
		//for all characters in the grid
		for(int i = 0; i < wordSearch.characters.Count; i++){
			if(Vector3.Distance(Input.GetTouch(0).position, wordSearch.characters[i].label.transform.position) < smallestDistance){
				//if this character is closer than all previous characters, this is the closest character
				smallestDistance = Vector3.Distance(Input.GetTouch(0).position, wordSearch.characters[i].label.transform.position);
				closestCharacter = wordSearch.characters[i];
			}
		}
		
		//make sure the mouse actually hits the closest character
		if(smallestDistance > wordSearch.width/1.2f)
			return null;
		
		//return the closest character
		return closestCharacter;
	}
	
	public void undo(){
		//if there aren't any lines to undo, don't undo anything
		if(lines.Count == 0)
			return;
		
		//get the last line and remove it from the list of lines
		GameObject lastDrawnLine = lines[lines.Count - 1];
		lines.RemoveAt(lines.Count - 1);
		
		//get the line start & end position using its name
		string lineName = lastDrawnLine.gameObject.name;
		int middlePartIndex = lineName.IndexOf("-");
		int start = int.Parse(lineName.Substring(0, lineName.Length - (lineName.Length - middlePartIndex)));
		int end = int.Parse(lineName.Substring(middlePartIndex + 1));
		
		//tell the word search script we drew a new line but we want to remove it
		wordSearch.drewNewLine(start, end, true, Vector2.zero);
		
		//actually remove the line
		Destroy(lastDrawnLine);
	}
}
