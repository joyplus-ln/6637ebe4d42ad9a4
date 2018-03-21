using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;

[CustomEditor(typeof(SlidingPuzzle))]
public class SlidePuzzleEditor : Editor{
	
	int toolbarSelected;
	
	SlidingPuzzle puzzle;
	ReorderableList puzzleList;
	
	void OnEnable(){
		puzzle = (target as SlidingPuzzle).gameObject.GetComponent<SlidingPuzzle>();
		
		puzzleList = new ReorderableList(serializedObject, serializedObject.FindProperty("puzzles"), true, true, false, true);
		
		puzzleList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = puzzleList.serializedProperty.GetArrayElementAtIndex(index);
	
			if(puzzle.puzzles[index].image != null){
				int previewSize = 105;
				int spacing = 2;
				
				float previewYPosition = rect.y + 5 + EditorGUIUtility.singleLineHeight;
				
				puzzle.puzzles[index].size = EditorGUI.IntSlider(new Rect(rect.x + previewSize + 5, rect.y + 5 + EditorGUIUtility.singleLineHeight, rect.width - previewSize - 25, 18), puzzle.puzzles[index].size, 2, 10);
				
				Rect rectangle = new Rect(rect.x, previewYPosition, previewSize, previewSize);
				EditorGUI.LabelField(rectangle, new GUIContent(AssetPreview.GetAssetPreview(puzzle.puzzles[index].image)));
				
				Color listColor = new Color(0.9f, 0.9f, 0.9f, 1);
				float cellWidth = previewSize/puzzle.puzzles[index].size;
				
				for(int i = 0; i <= puzzle.puzzles[index].size; i++){
					Rect Rectangle = new Rect(rect.x, previewYPosition + (i * cellWidth), previewSize, spacing);
					EditorGUI.DrawRect(Rectangle, listColor);
				}
				for(int i = 0; i <= puzzle.puzzles[index].size; i++){
					Rect Rectangle = new Rect(rect.x + (i * cellWidth), previewYPosition, spacing, previewSize);
					EditorGUI.DrawRect(Rectangle, listColor);
				}
				
				float cellPosition = cellWidth * (puzzle.puzzles[index].size - 1);
				EditorGUI.DrawRect(new Rect(rect.x + cellPosition, previewYPosition + cellPosition, cellWidth, cellWidth), listColor);
				
				if(!isActive && index != puzzle.puzzles.Count - 1)
					EditorGUI.DrawRect(new Rect(0, rect.y + (EditorGUIUtility.singleLineHeight * 8f), rect.width + 40, 2), new Color(0.75f, 0.75f, 0.75f, 1));
			}
	
			GUI.color = new Color(0.8f, 0.9f, 0.95f, 1);
			EditorGUI.PropertyField(
				new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight),
				element.FindPropertyRelative("image"), GUIContent.none);
				
			GUI.color = Color.white;
		};
	
		puzzleList.elementHeightCallback = (index) => { 
			if(puzzle.puzzles[index].image != null){
				return EditorGUIUtility.singleLineHeight * 8.5f;
			}
			else{
				return EditorGUIUtility.singleLineHeight * 1.5f;
			}
		};
	
		puzzleList.onAddCallback = (ReorderableList l) => {  
			var index = l.serializedProperty.arraySize;
			l.serializedProperty.arraySize++;
			l.index = index;
			//var element = l.serializedProperty.GetArrayElementAtIndex(index);
			//element.FindPropertyRelative("spawnPointIndex").intValue = 0;
			//element.FindPropertyRelative("enemyPrefab").objectReferenceValue = null;
			//element.FindPropertyRelative("delay").floatValue = 0;
		};
	
		puzzleList.onRemoveCallback = (ReorderableList l) => {  
			if(EditorUtility.DisplayDialog("Remove puzzle", "Are you sure you want to remove this puzzle?", "Yes", "No"))
				ReorderableList.defaultBehaviours.DoRemoveButton(l);
		};
	
		puzzleList.drawHeaderCallback = (Rect rect) => {  
			EditorGUI.LabelField(rect, "Puzzles (" + puzzle.puzzles.Count + ")");
		};
	}
	
	public override void OnInspectorGUI(){	
		GUILayout.Space(10);
	
		GUILayout.BeginHorizontal();
		toolbarSelected = GUILayout.Toolbar(toolbarSelected, new string[] {"Settings", "Puzzles"}, GUILayout.Height(25));
	
		GUILayout.EndHorizontal();
	
		if(toolbarSelected == 0){
			GUI.color = Color.white;
			DrawDefaultInspector();
		}
		else if(toolbarSelected == 1){
			serializedObject.Update();
			puzzleList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
			
			if(GUILayout.Button("New puzzle"))
				puzzle.puzzles.Add(new puzzle{ image = null, size = 1});
			
			if(GUILayout.Button("Clear all") && EditorUtility.DisplayDialog("Clear all puzzles", "Are you sure you want to clear all puzzles?", "Yes", "No"))
				puzzle.puzzles.Clear();
		}
	
		serializedObject.ApplyModifiedProperties();
		Undo.RecordObject(puzzle, "change in puzzle board");
	}	
}