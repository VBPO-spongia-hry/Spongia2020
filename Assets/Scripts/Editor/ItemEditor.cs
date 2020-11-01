using Items;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(Item))]
    public class ItemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("itemName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
            switch (serializedObject.FindProperty("type").intValue)
            {
                case 0:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("protectionLevel"));
                    break;
                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fireRate"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("range"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mode"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("projectile"));
                    break;
                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("toughness"));
                    break;
                case 3:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("backpackCapacity"));
                    break;
                case 4:
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}