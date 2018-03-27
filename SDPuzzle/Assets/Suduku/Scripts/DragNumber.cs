using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DragNumber : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	
	//visible in the inspector
	public float maxDropDistance;
	public int resetSpeed;
	
	//not visible in the inspector
	bool returnToStart;
    float startTime;
    float journeyLength;
	Vector2 lerpStart;
	
	Sudoku sudoku;
	Vector2 startPosition;
	Animator animator;
	
	void Start(){
		//find the main sudoku script, get the animator component and save the start position
		sudoku = GameObject.FindObjectOfType<Sudoku>();
		animator = GetComponent<Animator>();
		startPosition = transform.position;
	}
	
	void Update(){
		//if the number should jump back to its original position
		if(returnToStart){
			//use lerp to move the object back over time
			float distCovered = (Time.time - startTime) * resetSpeed * 100;
			float fracJourney = distCovered / journeyLength;
			transform.position = Vector2.Lerp(lerpStart, startPosition, fracJourney);
			
			//stop moving if the object has reached its start position
			if(new Vector2(transform.position.x, transform.position.y) == startPosition)
				returnToStart = false;
		}
	}
	
	//stop moving if the player starts dragging this number
	public void OnBeginDrag(PointerEventData eventData){
		returnToStart = false;
	}

    public void OnDrag(PointerEventData eventData){
		//move the number to the drag position
        transform.position = Input.mousePosition;
		
		//get the cell closest to the number and find the distance between that cell and the number label
		GameObject closestCell = getClosestCell();
		float smallestDistance = Vector3.Distance(transform.position, closestCell.transform.position);
		
		//get the index of the closest cell
		int cellIndex = int.Parse(closestCell.name);
		
		//show the green outline for the closest cell
		if(smallestDistance <= maxDropDistance && !sudoku.cells[cellIndex].clue)
			closestCell.GetComponent<Outline>().enabled = true;
    }

    public void OnEndDrag(PointerEventData eventData){
		//get the closest cell and again find the distance
		GameObject closestCell = getClosestCell();
		float smallestDistance = Vector3.Distance(transform.position, closestCell.transform.position);
		
		//get the index of the closest cell
		int cellIndex = int.Parse(closestCell.name);
		
		//if the cell is close enough, drop the number there and reset the number label position
		if(smallestDistance <= maxDropDistance && !sudoku.cells[cellIndex].clue){
			sudoku.cells[cellIndex].label.text = transform.GetChild(0).GetComponent<Text>().text;
			StartCoroutine(reset(closestCell.transform.position));
		}
		else{
			//if it is not close enough, return the number label to its original position
			startTime = Time.time;
			lerpStart = transform.position;
			journeyLength = Vector3.Distance(lerpStart, startPosition);
			returnToStart = true;
		}
    }
	
	public GameObject getClosestCell(){
		//closest distance and cell
		float smallestDistance = Mathf.Infinity;
		GameObject closestCell = null;
		
		//check all cells in the grid
		foreach(GameObject cell in GameObject.FindGameObjectsWithTag("cell")){
			//reset outline
			cell.GetComponent<Outline>().enabled = false;
			
			//if this cell is closer than all previous cells, this is the closest cell
			if(Vector2.Distance(transform.position, cell.transform.position) < smallestDistance){
				smallestDistance = Vector2.Distance(transform.position, cell.transform.position);
				closestCell = cell;
			}
		}
		
		//return the closest cell
		return closestCell;
	}
	
	IEnumerator reset(Vector2 cellPosition){
		//snap to the closest cell position
		transform.position = cellPosition;
		//play the apply animation
		animator.SetBool("end", true);
		//wait a moment and reset the number label position
		yield return new WaitForSeconds(0.3f);
		transform.position = startPosition;
		animator.SetBool("end", false);
	}
}
