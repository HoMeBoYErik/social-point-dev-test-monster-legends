#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace CJFinc {

	[CustomEditor(typeof(BitmapFontBestFit))]
	[CanEditMultipleObjects]
	public class BitmapFontBestFitEditor : Editor {
		SerializedProperty base_font_size;
		SerializedProperty base_size;
		SerializedProperty min_size;
		SerializedProperty max_size;

		BitmapFontBestFit script;

		void OnEnable () {
			base_font_size = serializedObject.FindProperty ("base_font_size");
			base_size = serializedObject.FindProperty ("base_size");
			min_size = serializedObject.FindProperty ("min_size");
			max_size = serializedObject.FindProperty ("max_size");

			script = (BitmapFontBestFit)target;
		}

		public override void OnInspectorGUI() {
			// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
			serializedObject.Update ();

			GUILayout.Label("Base font params", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("font size: ", base_font_size.intValue.ToString());
			EditorGUILayout.LabelField("area width: ", base_size.vector2Value.x.ToString());
			EditorGUILayout.LabelField("area height: ", base_size.vector2Value.y.ToString());
			this.Repaint();

			if (GUILayout.Button("Save current base size")) {
				script.SaveBaseSize();
			}

			GUILayout.Label("Best fit", EditorStyles.boldLabel);
			EditorGUILayout.IntSlider (min_size, 0, 300, new GUIContent ("Min size"));
			EditorGUILayout.IntSlider (max_size, 0, 300, new GUIContent ("Max size"));

			// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
			serializedObject.ApplyModifiedProperties ();
		}

	}

}
#endif
