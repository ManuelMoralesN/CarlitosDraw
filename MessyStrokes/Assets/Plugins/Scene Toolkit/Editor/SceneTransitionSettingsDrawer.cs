using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CG.SceneToolkit
{
    [CustomPropertyDrawer(typeof(SceneTransitionSettings))]
    public class SceneTransitionSettingsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty enableTransitionProperty = property.FindPropertyRelative("enableTransition");            

            SerializedProperty enableAudioFadeProperty = property.FindPropertyRelative("enableAudioFade");
            SerializedProperty audioMixerProperty = property.FindPropertyRelative("audioMixer");
            SerializedProperty exposedParameterProperty = property.FindPropertyRelative("exposedParameter");
            SerializedProperty audioFadeDurationProperty = property.FindPropertyRelative("audioFadeDuration");

            SerializedProperty transitionSpeedProperty = property.FindPropertyRelative("transitionSpeed");

            SerializedProperty transitionTypeProperty = property.FindPropertyRelative("transitionType");
            //SerializedProperty fadeTintColorProperty = property.FindPropertyRelative("fadeTintColor");
            SerializedProperty patternProperty = property.FindPropertyRelative("pattern");

            EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 0f, 0f)), enableTransitionProperty);

            if (enableTransitionProperty.boolValue)
            {
                EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 1.25f, 0f)), transitionSpeedProperty);

                EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 2.75f, 0f)), transitionTypeProperty);

                if(transitionTypeProperty.enumValueIndex== 0)
                {
                    //EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 4.5f, 0f)), fadeTintColorProperty);
                }
                
                if(transitionTypeProperty.enumValueIndex == 1)
                {
                    EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 4.5f, 0f)), patternProperty);
                }


                EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 6.25f, 0f)), enableAudioFadeProperty);

                if(enableAudioFadeProperty.boolValue)
                {
                    EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 7.75f, 0f)), audioMixerProperty);
                    EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 9.25f, 0f)), exposedParameterProperty);
                    EditorGUI.PropertyField(PropertyPosition(position, PropertyHeightOffset(property, 10.75f, 0f)), audioFadeDurationProperty);

                    string infoMessage = "1. Create an Audio Mixer\r\n" +
                                         "2. Route any audio you want to fade to Groups on that Mixer\r\n" +
                                         "3. Select Audio Mixer Group, right click on the Volume Component label in the Inspector\r\n" +
                                         "4. Select Expose ‘Volume (of Mixer)’ to script.\r\n" +
                                         "5. Rename the Exposed Parameter in the Audio Mixer panel\r\n" +
                                         "6. Make Sure Parameter mathces to this exposedParameter\r\n" +
                                             "\r\n (Tip: Expose the ‘Volume (of Master Mixer)’ to fade all audio)";

                    EditorGUILayout.Space(15f);
                    EditorGUILayout.HelpBox(infoMessage, MessageType.Info);
                }                
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {            
            float heightMultiplier = (property.FindPropertyRelative("enableTransition").boolValue) ? 6f : 0f;

            if (property.FindPropertyRelative("enableTransition").boolValue)
            {
                if (property.FindPropertyRelative("enableAudioFade").boolValue)
                {
                    heightMultiplier = 10f;
                }
            }

            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * heightMultiplier;
        }

        private float PropertyHeightOffset(SerializedProperty property, float normal, float expanded)
        {
            SerializedProperty activateProperty = property.FindPropertyRelative("enableTransition");
            return (activateProperty.boolValue) ? EditorGUIUtility.singleLineHeight * normal : expanded * EditorGUIUtility.singleLineHeight;
        }
        private Rect PropertyPosition(Rect position, float heightOffset)
                     => new Rect(position.x, position.y + heightOffset, position.width, EditorGUIUtility.singleLineHeight);
    }
}