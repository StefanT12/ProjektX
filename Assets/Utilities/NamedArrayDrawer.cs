using UnityEngine;
using UnityEditor;

namespace Assets.Utilities
{
    [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
    public class NamedArrayDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            try
            {
                int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
               
                //EditorGUILayout.PropertyField(property);
                //EditorGUI.indentLevel += 1;
                //for (int i = 0; i < property.arraySize; i++)
                //{
                //    EditorGUILayout.PropertyField(property.GetArrayElementAtIndex(i), label: );
                //}
                //EditorGUI.indentLevel -= 1;

                EditorGUI.PropertyField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]), property.isExpanded);
                var a = property.propertyType;
            }
            catch
            {
            }
        }
    }
}
