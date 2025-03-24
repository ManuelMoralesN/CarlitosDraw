using UnityEngine;
using UnityEditor;
using CG.Utilities;

namespace CG.SceneToolkit
{
    [CustomPropertyDrawer(typeof(SceneChangeTriggerSettings))]
    public class SceneChangeTriggerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);            

            SerializedProperty autoActivateSceneProperty = property.FindPropertyRelative("autoActivateScene");
            SerializedProperty loadModeProperty = property.FindPropertyRelative("loadMode");
            SerializedProperty sceneChangeEventProperty = property.FindPropertyRelative("sceneChangeEvent");
            SerializedProperty sceneToLoadProperty = property.FindPropertyRelative("sceneToLoad");            
            SerializedProperty layersToDetectProperty = property.FindPropertyRelative("layersToDetect");
            SerializedProperty detectionModeProperty = property.FindPropertyRelative("detectionMode");

            SerializedProperty triggerTypeProperty = property.FindPropertyRelative("triggerType");
            SerializedProperty switchingDelayProperty = property.FindPropertyRelative("switchingDelay");

            if (loadModeProperty.enumValueIndex == 0)
                EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 0f, 0f)), autoActivateSceneProperty);

            if(!autoActivateSceneProperty.boolValue)
            {
                EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 1.5f, 1.5f)), sceneChangeEventProperty);
            }
            else
            {
                EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 1.2f, 0f)), loadModeProperty);
            }

            GUI.color = string.IsNullOrEmpty(sceneToLoadProperty.stringValue) || !sceneToLoadProperty.stringValue.IsSceneNameExist()
                        || sceneToLoadProperty.stringValue == "Loading" ? Color.red : Color.white;            

            EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property,2.75f, 7.25f)), sceneToLoadProperty);
            
            GUI.color = Color.white;            
            EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 4.25f, 8.75f)),layersToDetectProperty);
            EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 5.75f, 10.2f)), detectionModeProperty);

            if(detectionModeProperty.enumValueIndex == 0)
            {
                EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 7f, 11.75f)), triggerTypeProperty);

                if(triggerTypeProperty.enumValueIndex == 1)
                {
                    EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 8.5f, 13.2f)), switchingDelayProperty);
                }
            }

            if (detectionModeProperty.enumValueIndex == 1)
            {
                EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 6f, 14.75f)), switchingDelayProperty);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty activateProperty = property.FindPropertyRelative("autoActivateScene");

            float heightMultiplier = (activateProperty.boolValue) ? 7.5f : 15f;

            if(activateProperty.boolValue)
            {                
                if(property.FindPropertyRelative("triggerType").enumValueIndex == 1)
                {
                    heightMultiplier = 8f;
                }

                if (property.FindPropertyRelative("detectionMode").enumValueIndex == 1)
                {
                    heightMultiplier = 6.5f;
                }
            }
            else
            {
                if (property.FindPropertyRelative("triggerType").enumValueIndex == 1)
                {
                    heightMultiplier = 13f;
                }
                else
                {
                    heightMultiplier = 12f;
                }                
            }            

            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * heightMultiplier;
        }

        private float PropertyHeightOffset(SerializedProperty property, float normal, float expanded)
        {
            SerializedProperty activateProperty = property.FindPropertyRelative("autoActivateScene");
            return (activateProperty.boolValue) ? EditorGUIUtility.singleLineHeight * normal : expanded * EditorGUIUtility.singleLineHeight;
        }
        private Rect PropertyPosition(Rect position, float heightOffset) 
                     => new Rect(position.x, position.y + heightOffset, position.width, EditorGUIUtility.singleLineHeight);
    }
}
