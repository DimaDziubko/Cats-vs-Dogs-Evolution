using System;
using System.Collections.Generic;
using _Game.Common.CustomAttribute;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Editor
{
    [CustomPropertyDrawer(typeof(SerializeInterfaceAttribute))]
    public class SerializeInterfaceDrawer : PropertyDrawer
    {
        private const string ErrorMessage = "SerializeInterfaceAttribute works only with GameObject";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsValidField() == false)
            {
                EditorGUI.HelpBox(position, ErrorMessage, MessageType.Error);
                return;
            }

            Type requiredType = (attribute as SerializeInterfaceAttribute).Type;
            
            UpdatePropertyValue(property, requiredType);
            
            EditorGUI.ObjectField(position, label, property.exposedReferenceValue, typeof(GameObject), true);
        }

        private bool IsValidField()
        {
            return fieldInfo.FieldType == typeof(GameObject) ||
                   typeof(IEnumerable<GameObject>).IsAssignableFrom(fieldInfo.FieldType);
        }

        private bool IsInvalidObject(Object @object, Type requiredType)
        {
            if (@object is GameObject gameObject)
            {
                return gameObject.GetComponent(requiredType) == null;
            }

            return true;
        }
        
        private void UpdatePropertyValue(SerializedProperty property, Type requiredType)
        {
            if(property.objectReferenceValue == null)
                return;

            if (IsInvalidObject(property.objectReferenceValue, requiredType))
            {
                property.objectReferenceValue = null;
            }
        }

        private void UpdateDropItem(Rect position, Type requiredType)
        {
            if (position.Contains(Event.current.mousePosition) == false)
            {
                return;
            }

            foreach (var reference in DragAndDrop.objectReferences)
            {
                if (IsInvalidObject(reference, requiredType))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
            }
            
        }
    }
}