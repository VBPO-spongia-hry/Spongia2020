using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(EnemyMovement))]
    public class EnemyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("frontSkeleton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sideSkeleton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backSkeleton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("flashlight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("followSpeed"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRange"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("observeTime"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"));
            switch (serializedObject.FindProperty("mode").intValue)
            {
                case 0:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDefendRadius"));
                    break;
                case 1:
                    break;
                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("waypoints"));
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}