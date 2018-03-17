using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorInspectionScript : Editor
{
	TerrainGenerator myTarget;
	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();
		 myTarget = (TerrainGenerator)target;
		if (GUILayout.Button("Generate Terrain"))
		{
			myTarget.GenerateTerrain();
		}
		if (EditorGUI.EndChangeCheck())
		{	
			GenerateTerrain();
		}
	}

	private void OnEnable()
	{
		myTarget = (TerrainGenerator)target;
		Undo.undoRedoPerformed += GenerateTerrain;
	}

	private void OnDisable()
	{
		Undo.undoRedoPerformed -= GenerateTerrain;
	}

	private void GenerateTerrain()
	{
		myTarget.GenerateTerrain();
	}
}
