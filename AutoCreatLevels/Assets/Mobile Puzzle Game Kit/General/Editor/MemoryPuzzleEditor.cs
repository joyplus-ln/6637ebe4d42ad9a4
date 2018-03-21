using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;

[CustomEditor(typeof(MemoryGame))]
public class MemoryPuzzleEditor : Editor{
	
	MemoryGame memoryGame;
	ReorderableList puzzleList;
	
	Texture2D switchOn;
	Texture2D switchOff;
	
	int toolbarSelected;
	AnimBool viewImages;
	
	Color color = new Color(1f, 1f, 1f, 0.3f);
	
	void OnEnable(){
		memoryGame = (target as MemoryGame).gameObject.GetComponent<MemoryGame>();
		
		switchOn = (Texture2D)Resources.Load("switch on") as Texture2D;
		switchOff = (Texture2D)Resources.Load("switch off") as Texture2D;
		
		viewImages = new AnimBool(false);
		viewImages.valueChanged.AddListener(Repaint);
	}
	
	public override void OnInspectorGUI(){
		if(EditorApplication.isPlaying){
			EditorGUILayout.HelpBox("Playmode", MessageType.Info);
			return;
		}
		
		GUILayout.Space(5);
		toolbarSelected = GUILayout.Toolbar(toolbarSelected, new string[] {"Settings", "Other"}, GUILayout.Height(25));
		
		if(toolbarSelected == 0){
			GUIContent switchImage = null;
		
			if(memoryGame.useLevels){
				switchImage = new GUIContent("  On", switchOn);
			}
			else{
				switchImage = new GUIContent("  Off", switchOff);
			}
			
			GUILayout.Space(5);
			GUILayout.Label("Level system", EditorStyles.largeLabel);
			if(GUILayout.Button(switchImage, EditorStyles.label, GUILayout.Height(20)))
				memoryGame.useLevels = !memoryGame.useLevels;
			
			GUILayout.Space(10);
			
			if(memoryGame.useLevels){				
				for(int i = 0; i < memoryGame.levels.Count; i++){
					GUILayout.Label("Level " + (i + 1) + " (" + memoryGame.levels[i].x + " x " + memoryGame.levels[i].y + ")", EditorStyles.largeLabel);
					
					GUILayout.BeginHorizontal();
					GUI.color = new Color(0, 0, 0, 0.2f);
					GUILayout.BeginVertical("Box");
					GUI.color = Color.white;
					
					GUILayout.Space(5);
					GUILayout.BeginHorizontal();
					GUILayout.Label("X", GUILayout.Width(15));
					
					if(GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20), GUILayout.Height(15)) && memoryGame.levels[i].x > 1)
						memoryGame.levels[i].x--;
					
					if(GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20), GUILayout.Height(15)))
						memoryGame.levels[i].x++;
					
					GUILayout.Space(30);
					
					GUILayout.Label("Y", GUILayout.Width(15));
					
					if(GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(20), GUILayout.Height(15)) && memoryGame.levels[i].y > 1)
						memoryGame.levels[i].y--;
					
					if(GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20), GUILayout.Height(15)))
						memoryGame.levels[i].y++;
					
					GUILayout.EndHorizontal();
					
					int totalImages = memoryGame.levels[i].x * memoryGame.levels[i].y;
		
					if(memoryGame.levels[i].images.Count > totalImages/2){
						memoryGame.levels[i].images.RemoveAt(memoryGame.levels[i].images.Count - 1);
					}
					else if(memoryGame.levels[i].images.Count < totalImages/2){
						memoryGame.levels[i].images.Add(new Sprite{});
					}
					
					GUILayout.Space(5);
					bool imagesSelected = true;		
					for(int I = 0; I < memoryGame.levels[i].images.Count; I++){
						memoryGame.levels[i].images[I] = EditorGUILayout.ObjectField(memoryGame.levels[i].images[I], typeof(Sprite), false) as Sprite;
			
						if(memoryGame.levels[i].images[I] == null)
							imagesSelected = false;
					}
					GUILayout.Space(5);
		
					if(!imagesSelected)
						EditorGUILayout.HelpBox("Please add enough images", MessageType.Warning);
		
					if(totalImages % 2 != 0)
						EditorGUILayout.HelpBox("Please make sure the total number of images is even (x * y) in order to make pairs", MessageType.Error);
					GUILayout.EndVertical();
					
					GUI.color = new Color(0.8f, 0.4f, 0.4f, 1);
					if(GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(18), GUILayout.Height(18)))
						memoryGame.levels.RemoveAt(i);
					
					GUI.color = Color.white;
					GUILayout.EndHorizontal();
					
					GUILayout.Space(5);
				}
				
				if(GUILayout.Button("New level"))
					memoryGame.levels.Add(new level{x = 1, y = 1, images = new List<Sprite>()});
				
				if(GUILayout.Button("Clear all") && EditorUtility.DisplayDialog("Clear all levels", "Are you sure you want to clear all levels?", "Yes", "No"))
					memoryGame.levels.Clear();
			}
			else{							
				string buttonContent = "► Current images (" + memoryGame.images.Count + ")";
				
				if(viewImages.target)
					buttonContent = "▼ Current images (" + memoryGame.images.Count + ")";
				
				if(GUILayout.Button(buttonContent, EditorStyles.largeLabel, GUILayout.Width(140)) && memoryGame.images.Count != 0)
					viewImages.target = !viewImages.target;
				
				if(EditorGUILayout.BeginFadeGroup(viewImages.faded)){				
					for(int i = 0; i < memoryGame.images.Count; i++){
						GUILayout.BeginHorizontal();
						memoryGame.images[i] = EditorGUILayout.ObjectField(memoryGame.images[i], typeof(Sprite), false) as Sprite;
					
						GUI.color = new Color(0.8f, 0.4f, 0.4f, 1);
						if(GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.Width(15), GUILayout.Height(15)))
							memoryGame.images.RemoveAt(i);
					
						GUI.color = Color.white;
					
						GUILayout.EndHorizontal();
						GUILayout.Space(4);
					}
					
					if(GUILayout.Button("Clear", EditorStyles.miniButton) && EditorUtility.DisplayDialog("Clear all images", "Are you sure you want to clear all images?", "Yes", "No")){
						memoryGame.images.Clear();	
						viewImages.target = false;
					}
				}
				EditorGUILayout.EndFadeGroup();
				
				if(memoryGame.images.Count < (memoryGame.x * memoryGame.y)/2)
					EditorGUILayout.HelpBox("Please make sure to add at least " + (memoryGame.x * memoryGame.y)/2 + " images. If you add more images then necessary, the images will be picked randomly.", MessageType.Error);
				
				GUILayout.Space(10);
				GUILayout.Label("Drag & drop sprites:", EditorStyles.largeLabel);
				dropArea();
				GUILayout.Space(5);
				
				GUILayout.Label("Size:", EditorStyles.largeLabel);
				GUILayout.BeginHorizontal();
				GUILayout.Label("X", GUILayout.Width(15));
				memoryGame.x = EditorGUILayout.IntField(memoryGame.x, GUILayout.Width(50));
				GUILayout.Space(20);
				GUILayout.Label("Y", GUILayout.Width(15));
				memoryGame.y = EditorGUILayout.IntField(memoryGame.y, GUILayout.Width(50));
				GUILayout.EndHorizontal();
				
				if((memoryGame.x * memoryGame.y) % 2 != 0)
					EditorGUILayout.HelpBox("The total number of images (x * y) should be even in order to make pairs", MessageType.Error);
				
				GUILayout.Space(5);
			}
		}
		else{
			DrawDefaultInspector();
		}
	
		serializedObject.ApplyModifiedProperties();
		Undo.RecordObject(memoryGame, "change in memory game settings");
	}	
	
	public void dropArea(){
		Event evt = Event.current;
		GUILayout.BeginHorizontal();
		Rect drop_area = GUILayoutUtility.GetRect(200.0f, 40.0f);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUI.color = color;
		GUI.Box(drop_area, "");
		GUI.color = Color.white;
     
		switch(evt.type){
			case EventType.DragUpdated:
			case EventType.DragPerform:
					
			if(!drop_area.Contains(evt.mousePosition)){
				color = new Color(1f, 1f, 1f, 0.3f);
				return;
			}
			else{
				if(allowed(DragAndDrop.objectReferences)){
					color = new Color(0f, 1f, 0.4f, 0.3f);
				}
				else{
					color = new Color(1f, 0f, 0f, 0.3f);
				}
			}
        
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
         
			if(evt.type == EventType.DragPerform){
				DragAndDrop.AcceptDrag();
				
				foreach(Object draggedObject in DragAndDrop.objectReferences){
					if(draggedObject.GetType() == typeof(UnityEngine.Texture2D))
						addObject(draggedObject);
				}
				
				color = new Color(1f, 1f, 1f, 0.3f);
				viewImages.target = true;
			}
			
			break;
		}
	}
	
	public void addObject(Object draggedObject){
		Texture2D tex = draggedObject as Texture2D;
		Sprite sprite = Sprite.Create(draggedObject as Texture2D, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
		sprite.name = draggedObject.name;
		memoryGame.images.Add(sprite);
	}
	
	public bool allowed(Object[] objectReferences){
		foreach(Object draggedObject in objectReferences){
			if(draggedObject.GetType() != typeof(UnityEngine.Texture2D))
				return false;
		}
		
		return true;
	}
}