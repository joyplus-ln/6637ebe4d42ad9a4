using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UI;

[CustomEditor(typeof(Sudoku))]
public class SudokuEditor : Editor{
	
	int toolbarSelected;
	
	Sudoku sudoku;
	
	void OnEnable(){
		sudoku = (target as Sudoku).gameObject.GetComponent<Sudoku>();
	}
	
	public override void OnInspectorGUI(){	
	
	GUILayout.Space(10);
	
	GUILayout.BeginHorizontal();
	toolbarSelected = GUILayout.Toolbar(toolbarSelected, new string[] {"Settings", "Other"}, GUILayout.Height(25));
	
	GUILayout.EndHorizontal();
	
	if(toolbarSelected == 0){
		sudoku.clueGeneratorType = (Sudoku.clueGenerator)EditorGUILayout.EnumPopup("Clue generator: ", sudoku.clueGeneratorType);
		if(sudoku.clueGeneratorType == Sudoku.clueGenerator.automatic){
			EditorGUILayout.HelpBox("Any amount of clues smaller then this slider value will use the efficient clue generator and any amount above the value will use the simple clue generator", MessageType.Info);
			sudoku.simpleClueGeneratorMinClues = EditorGUILayout.IntSlider(sudoku.simpleClueGeneratorMinClues, 17, 80);
		}
		
		GUI.color = new Color(1, 1, 1, 0.4f);
		GUILayout.BeginVertical("Box");
		GUI.color = Color.white;
		
		sudoku.emptyColor = EditorGUILayout.ColorField("Normal cell color", sudoku.emptyColor);
		sudoku.labelColor = EditorGUILayout.ColorField("Normal label color", sudoku.labelColor);
		sudoku.clueColor = EditorGUILayout.ColorField("Clue cell color", sudoku.clueColor);
		sudoku.clueLabelColor = EditorGUILayout.ColorField("Clue label color", sudoku.clueLabelColor);
		sudoku.correctColor = EditorGUILayout.ColorField("Correct answer color", sudoku.correctColor);
		sudoku.wrongColor = EditorGUILayout.ColorField("Wrong answer color", sudoku.wrongColor);
		
		GUILayout.EndVertical();
		
		sudoku.effectTime = EditorGUILayout.FloatField("Cell spawn effect time", sudoku.effectTime);
		sudoku.fadeSpeed = EditorGUILayout.FloatField("Cell fade speed", sudoku.fadeSpeed);
	}
	
	if(toolbarSelected == 1){
		GUI.color = Color.white;
		DrawDefaultInspector();
	}
	
    serializedObject.ApplyModifiedProperties();
	Undo.RecordObject(sudoku, "change in sudoku board");
	}	
	
	[MenuItem("Window/Delete PlayerPrefs")]
    static void DeletePlayerPrefs(){
		if(EditorUtility.DisplayDialog("Delete PlayerPrefs", "Are you sure you want to delete all PlayerPrefs?", "Yes", "No"))
			PlayerPrefs.DeleteAll();
    }
}