using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;
using System.Collections.Generic;

[CustomEditor(typeof(WordSearch))]
public class WordSearchEditor : Editor{
	
	List<bool> categories;
	int toolbarSelected;
	WordSearch wordSearch;
	string[] arrows = {"arrow7", "arrow", "arrow1", "arrow6", "center", "arrow2", "arrow5", "arrow4", "arrow3"};
	bool showDirections;
	
	void OnEnable(){
		wordSearch = (target as WordSearch).gameObject.GetComponent<WordSearch>();
		categories = new List<bool>();
		
		for(int i = 0; i < wordSearch.categories.Count; i++){
			categories.Add(new bool{});
		}
	}
	
	public override void OnInspectorGUI(){	
	GUILayout.Space(10);
	toolbarSelected = GUILayout.Toolbar(toolbarSelected, new string[] {"Settings", "Other"}, GUILayout.Height(25));

	if(toolbarSelected == 0){
		GUILayout.Label("Categories", EditorStyles.largeLabel);
		showCategories();
		
		string label = "";
		if(!showDirections){
			label = "► Enabled directions";
		}
		else{
			label = "▼ Enabled directions";
		}
		
		if(GUILayout.Button(label, EditorStyles.largeLabel))
			showDirections = !showDirections;
		
		if(showDirections)
			enabledDirections(30);
	}
	if(toolbarSelected == 1){
		GUI.color = Color.white;
		DrawDefaultInspector();
	}
	
    serializedObject.ApplyModifiedProperties();
	Undo.RecordObject(wordSearch, "change in word search editor");
	}	
	
	void showCategories(){
		for(int i = 0; i < wordSearch.categories.Count; i++){
			GUI.color = new Color(1f, 1f, 1f, 0.9f);
			
			if(categories[i])
				GUI.color = new Color(0.6f, 0.8f, 0.8f, 1);
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(wordSearch.categories[i].name, GUILayout.Height(20)))
				categories[i] = !categories[i];
			
			GUI.color = new Color(1f, 0.5f, 0.5f, 1);
			if(GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)) && EditorUtility.DisplayDialog("Remove entire category", "Are you sure you want to remove this category: '" + wordSearch.categories[i].name + "'?", "Yes", "No")){
				wordSearch.categories.RemoveAt(i);
				categories.RemoveAt(i);
			}
			
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
			
			if(i < wordSearch.categories.Count && categories[i]){
				GUILayout.Space(-2);
				GUILayout.BeginHorizontal();
				GUI.color = new Color(0.6f, 0.6f, 0.6f, 0.3f);
				GUILayout.BeginVertical("Box");
				GUI.color = Color.white;
				showCategory(i);
				GUILayout.EndVertical();
				GUILayout.Space(25);
				GUILayout.EndHorizontal();
			}
		}
		
		GUILayout.BeginHorizontal();
		GUI.color = new Color(1f, 0.65f, 0.2f, 1f);
		if(GUILayout.Button("Add category", EditorStyles.miniButton)){
			categories.Add(true);
			wordSearch.categories.Add(new category{name = "New category", wordPuzzles = new List<wordPuzzle>()});
		}
		GUI.color = Color.white;
		
		GUILayout.Label("Use text files", GUILayout.Width(80));
		wordSearch.textFileWords = EditorGUILayout.Toggle(wordSearch.textFileWords, GUILayout.Width(15));
		GUILayout.EndHorizontal();
	}
	
	void showCategory(int i){
		wordSearch.categories[i].name = EditorGUILayout.TextField("Category", wordSearch.categories[i].name);
		
		if(wordSearch.textFileWords)
			wordSearch.categories[i].words = EditorGUILayout.ObjectField("Words", wordSearch.categories[i].words, typeof(TextAsset), false) as TextAsset;
		
		if(wordSearch.categories[i].wordPuzzles != null){
		for(int I = 0; I < wordSearch.categories[i].wordPuzzles.Count; I++){
			GUI.color = new Color(1, 1, 1, 0.3f);
			GUILayout.BeginVertical("Box");
			GUI.color = Color.white;
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Puzzle " + (I + 1), EditorStyles.boldLabel);
			
			GUI.color = new Color(1, 1, 1, 0.4f);
			if(GUILayout.Button("X", EditorStyles.boldLabel, GUILayout.Width(15)))
				wordSearch.categories[i].wordPuzzles.RemoveAt(I);
			GUI.color = Color.white;
			
			GUILayout.EndHorizontal();
			
			if(I != wordSearch.categories[i].wordPuzzles.Count){
				wordSearch.categories[i].wordPuzzles[I].size = EditorGUILayout.IntField("Grid size", wordSearch.categories[i].wordPuzzles[I].size);
			
				if(wordSearch.textFileWords){
					wordSearch.categories[i].wordPuzzles[I].numberOfWords = EditorGUILayout.IntField("Number of words", wordSearch.categories[i].wordPuzzles[I].numberOfWords);
				}
				else{
					GUI.color = new Color(1, 1, 1, 0.3f);
					GUILayout.BeginVertical("Box");
					GUI.color = Color.white;
					
					if(wordSearch.categories[i].wordPuzzles[I].words != null && wordSearch.categories[i].wordPuzzles[I].words.Count != 0){
						for(int j = 0; j < wordSearch.categories[i].wordPuzzles[I].words.Count; j++){
							GUILayout.BeginHorizontal();
							wordSearch.categories[i].wordPuzzles[I].words[j] = EditorGUILayout.TextField("Word " + (j + 1), wordSearch.categories[i].wordPuzzles[I].words[j]);
							GUI.color = new Color(0.9f, 0.5f, 0.5f, 0.8f);
							if(GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
								wordSearch.categories[i].wordPuzzles[I].words.RemoveAt(j);
							GUI.color = Color.white;
							GUILayout.EndHorizontal();
						}
					}
					
					if(GUILayout.Button("Add word", EditorStyles.miniButton))
						wordSearch.categories[i].wordPuzzles[I].words.Add("");
						
					GUILayout.EndVertical();
				}
			}
			GUILayout.EndVertical();
		}
		}
		
		GUI.color = new Color(0.6f, 0.85f, 1f, 1f);
		if(GUILayout.Button("Add puzzle", EditorStyles.toolbarButton))
			wordSearch.categories[i].wordPuzzles.Add(new wordPuzzle{size = 4, words = new List<string>()});
		
		GUI.color = Color.white;
	}
	
	void enabledDirections(int buttonWidth){
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
		GUI.color = new Color(0, 0, 0, 0.15f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		int index = 0;
		
		for(int i = 0; i < 3; i++){
			GUILayout.BeginHorizontal();
			for(int I = 0; I < 3; I++){
				int element = -1;
				
				switch(index){
					case 0: element = 7; break;
					case 1: element = 0; break;
					case 2: element = 1; break;
					case 3: element = 6; break;
					case 5: element = 2; break;
					case 6: element = 5; break;
					case 7: element = 4; break;
					case 8: element = 3; break;
				}
				
				if(element != -1 && wordSearch.enabledDirections[element]){
					GUI.color = new Color(0.5f, 1.0f, 0.7f, 1.0f);
				}
				else{
					GUI.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
				}
				
				Texture2D arrow = Resources.Load(arrows[index]) as Texture2D;
				
				if(GUILayout.Button(new GUIContent(arrow), EditorStyles.label, GUILayout.Width(buttonWidth), GUILayout.Height(buttonWidth)) && element != -1)
					wordSearch.enabledDirections[element] = !wordSearch.enabledDirections[element];
					
				GUI.color = Color.white;
				index++;
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	bool direction(){
		for(int i = 0; i < 7; i++){
			if(wordSearch.enabledDirections[i])
				return true;
		}
		
		return false;
	}
}