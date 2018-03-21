using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryTile : MonoBehaviour {
	
	//visible in the inspector
	public int pairIndex;
	
	//not visible
	Animator animator;
	MemoryGame memoryGame;
	
	bool canFlip = true;
	
	void Start(){
		//get the animator component and memorygame script
		animator = GetComponent<Animator>();
		memoryGame = FindObjectOfType<MemoryGame>();
	}

	public void flip(){
		//if the tile is not already flipping, flip the tile
		if(canFlip)
			StartCoroutine(flipTile());
	}
	
	IEnumerator flipTile(){
		//play the flip animation and wait a moment
		canFlip = false;
		animator.SetBool("image visible", true);
		yield return new WaitForSeconds(0.7f);
		canFlip = true;
		
		//let the main script know we flipped a new tile
		memoryGame.tileFlipped(this.gameObject);
	}
}
