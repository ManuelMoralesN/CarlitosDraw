using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace CG.SceneToolkit
{
    [CustomEditor(typeof(TriggerBase), true)]
    public class SceneChangeTriggerEditor : Editor
    {
        public override void OnInspectorGUI()
        {            
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            TriggerBase trigger = (TriggerBase)target;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 20;

            if (trigger.TriggerSettings.SceneToLoad == "Loading") return;

            if (string.IsNullOrEmpty(trigger.TriggerSettings.SceneToLoad)) return;

            string message = IsSceneNameExist(trigger.TriggerSettings.SceneToLoad) ? "Destination: " + trigger.TriggerSettings.SceneToLoad 
                                                                             : "Scene Not Found In BuildIndex";

            Handles.Label(trigger.transform.position + Vector3.up, message, style);            
        }

        private bool IsSceneNameExist(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var sceneNameInBuildSetting = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
                if (sceneNameInBuildSetting.Equals(sceneName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}