using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		int layer = property.intValue;
		layer = EditorGUI.LayerField(position, label, layer);
		property.intValue = layer;
	}

}
