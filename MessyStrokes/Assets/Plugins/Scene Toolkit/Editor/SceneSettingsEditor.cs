using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using CG.RenderPipeline;
using CG.Utilities;

namespace CG.SceneToolkit
{
    [CustomEditor(typeof(SceneSettings))]
    public class SceneSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SceneSettings settings = target as SceneSettings;

            string loadingSceneName = "Loading";

            if (!loadingSceneName.IsSceneNameExist())
            {
                GUILayout.Space(30);
                EditorGUILayout.HelpBox("Add The Loading Scene To Build Settings", MessageType.Error);

                GUILayout.Space(30);
                EditorGUI.indentLevel++;
                if (GUILayout.Button("Add Loading Scene", GUILayout.Height(30)))
                {
                    var original = EditorBuildSettings.scenes;
                    var newSettings = new EditorBuildSettingsScene[original.Length + 1];
                    System.Array.Copy(original, newSettings, original.Length);
                    var sceneToAdd = new EditorBuildSettingsScene("Assets/Plugins/Scene Toolkit/Resources/Loading.unity", true);
                    newSettings[newSettings.Length - 1] = sceneToAdd;
                    EditorBuildSettings.scenes = newSettings;
                }
                EditorGUI.indentLevel--;

                return;
            }            

            base.OnInspectorGUI();

            if (Application.isPlaying) return;

            if (loadingSceneName.IsSceneNameExist())
            {
                if(EditorSceneManager.GetActiveScene().name != loadingSceneName)
                {
                    GUILayout.Space(30);
                    EditorGUI.indentLevel++;
                    if (GUILayout.Button("Go To Loading Scene", GUILayout.Height(30)))
                    {
                        settings.currentScenePath = EditorSceneManager.GetActiveScene().path;
                        EditorSceneManager.OpenScene("Assets/Plugins/Scene Toolkit/Resources/Loading.unity");
                    }
                    EditorGUI.indentLevel--;
                }                
            }

            GUILayout.Space(20);
            EditorGUI.indentLevel++;            
            var isActive = UniversalRenderPipelineUtilities.IsRendererFeatureActive("FullScreenPassRendererFeature");
            if(!isActive)
            {
                if (GUILayout.Button("Setup Renderer", GUILayout.Height(30)))
                {
                    SetupRendererFeature.AddRendererFeature<FullScreenPassRendererFeature>();

                    UniversalRenderPipelineUtilities.GetRendererFeature<FullScreenPassRendererFeature>().passMaterial 
                        = Resources.Load("TransitionMaterial") as Material;
                }
            }
            EditorGUI.indentLevel--;

            if (EditorSceneManager.GetActiveScene().name == loadingSceneName)
            {
                GUILayout.Space(30);
                EditorGUI.indentLevel++;
                if (GUILayout.Button("Back", GUILayout.Height(30)))
                {
                    EditorSceneManager.OpenScene(settings.currentScenePath);
                }
                EditorGUI.indentLevel--;
            }            
        }
    }
}