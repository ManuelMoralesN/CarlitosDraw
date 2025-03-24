using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

namespace CG.RenderPipeline
{
    public static class SetupRendererFeature
    {
        public static void AddRendererFeature<T>() where T : ScriptableRendererFeature
        {
            var handledDataObjects = new List<ScriptableRendererData>();

            int levels = QualitySettings.names.Length;
            for (int level = 0; level < levels; level++)
            {
                // Fetch renderer data
                var asset = QualitySettings.GetRenderPipelineAssetAt(level) as UniversalRenderPipelineAsset;
                // Do NOT use asset.LoadBuiltinRendererData().
                // It's a trap, see: https://github.com/Unity-Technologies/Graphics/blob/b57fcac51bb88e1e589b01e32fd610c991f16de9/Packages/com.unity.render-pipelines.universal/Runtime/Data/UniversalRenderPipelineAsset.cs#L719
                var data = GetDefaultRenderer(asset);

                // This is needed in case multiple renderers share the same renderer data object.
                // If they do then we only handle it once.
                if (handledDataObjects.Contains(data))
                {
                    continue;
                }
                handledDataObjects.Add(data);

                // Create & add feature if not yet existing
                bool found = false;
                foreach (var feature in data.rendererFeatures)
                {
                    if (feature is T)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    // Create the feature
                    var feature = ScriptableObject.CreateInstance<T>();
                    feature.name = typeof(T).Name;

                    // Add it to the renderer data.
                    AddRenderFeature(data, feature);

                    Debug.Log("Added render feature '" + feature.name + "' to " + data.name);
                }
            }
        }

        /// <summary>
        /// Gets the default renderer index.
        /// Thanks to: https://forum.unity.com/threads/urp-adding-a-renderfeature-from-script.1117060/#post-7184455
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        static int GetDefaultRendererIndex(UniversalRenderPipelineAsset asset)
        {
            return (int)typeof(UniversalRenderPipelineAsset).GetField("m_DefaultRendererIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(asset);
        }

        /// <summary>
        /// Gets the renderer from the current pipeline asset that's marked as default.
        /// Thanks to: https://forum.unity.com/threads/urp-adding-a-renderfeature-from-script.1117060/#post-7184455
        /// </summary>
        /// <returns></returns>
        static ScriptableRendererData GetDefaultRenderer(UniversalRenderPipelineAsset asset)
        {
            if (asset)
            {
                ScriptableRendererData[] rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset)
                        .GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(asset);
                int defaultRendererIndex = GetDefaultRendererIndex(asset);

                return rendererDataList[defaultRendererIndex];
            }
            else
            {
                Debug.LogError("No Universal Render Pipeline is currently active.");
                return null;
            }
        }

        /// <summary>
        /// Based on Unity add feature code.
        /// See: AddComponent() in https://github.com/Unity-Technologies/Graphics/blob/d0473769091ff202422ad13b7b764c7b6a7ef0be/com.unity.render-pipelines.universal/Editor/ScriptableRendererDataEditor.cs#180
        /// </summary>
        /// <param name="data"></param>
        /// <param name="feature"></param>
        static void AddRenderFeature(ScriptableRendererData data, ScriptableRendererFeature feature)
        {
            // Let's mirror what Unity does.
            var serializedObject = new SerializedObject(data);

            var renderFeaturesProp = serializedObject.FindProperty("m_RendererFeatures"); // Let's hope they don't change these.
            var renderFeaturesMapProp = serializedObject.FindProperty("m_RendererFeatureMap");

            serializedObject.Update();

            // Store this new effect as a sub-asset so we can reference it safely afterwards.
            // Only when we're not dealing with an instantiated asset
            if (EditorUtility.IsPersistent(data))
                AssetDatabase.AddObjectToAsset(feature, data);
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(feature, out var guid, out long localId);

            // Grow the list first, then add - that's how serialized lists work in Unity
            renderFeaturesProp.arraySize++;
            var componentProp = renderFeaturesProp.GetArrayElementAtIndex(renderFeaturesProp.arraySize - 1);
            componentProp.objectReferenceValue = feature;

            // Update GUID Map
            renderFeaturesMapProp.arraySize++;
            var guidProp = renderFeaturesMapProp.GetArrayElementAtIndex(renderFeaturesMapProp.arraySize - 1);
            guidProp.longValue = localId;

            // Force save / refresh
            if (EditorUtility.IsPersistent(data))
            {
                AssetDatabase.SaveAssetIfDirty(data);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}


