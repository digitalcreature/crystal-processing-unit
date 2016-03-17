using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidGenerator))]
public class AsteroidGeneratorEditor : Editor {

	private Editor prefabEditor;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		AsteroidGenerator generator = (AsteroidGenerator) target;
		if (generator.chunkPrefab != null) {
			CreateCachedEditor(generator.chunkPrefab, null, ref prefabEditor);
			EditorGUI.indentLevel ++;
			prefabEditor.OnInspectorGUI();
			EditorGUI.indentLevel --;
		}
		if (GUILayout.Button("Generate")) {
			generator.Generate();
		}
	}

}
