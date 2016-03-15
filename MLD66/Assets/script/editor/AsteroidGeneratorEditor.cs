using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AsteroidGenerator))]
public class AsteroidGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if (GUILayout.Button("Generate")) {
			((AsteroidGenerator) target).Generate();
		}
	}

}
