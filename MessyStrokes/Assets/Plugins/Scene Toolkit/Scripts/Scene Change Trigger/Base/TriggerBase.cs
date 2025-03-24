using UnityEngine;
using UnityEngine.SceneManagement;

namespace CG.SceneToolkit
{
    /// <summary>
    /// Base class that handles triggering of scene changes.
    /// This is an abstract class, which is used by both
    /// 'SceneSwitchTrigger2D' and 'SceneSwitchTrigger3D'
    /// Inhereting from this class is Not Recomended
    /// </summary>
    public abstract class TriggerBase : MonoBehaviour
    {
        [Tooltip("'Scene Settings' ScriptableObject, Created as an asset")]
        [Space]
        [SerializeField] protected SceneSettings sceneSettings;

        [Tooltip("Contains the settings that handles a scene change trigger")]
        [Space]
        [SerializeField] protected SceneChangeTriggerSettings triggerSettings;

        /// <summary>
        /// Contains the settings that handles a scene change trigger.
        /// </summary>
        public SceneChangeTriggerSettings TriggerSettings => triggerSettings;

        protected float entryTime;
        protected bool isDetected;       

        protected void HandleTriggerEnter(Collider other) 
        {
            if (!IsInLayerMask(other)) return;
            if(triggerSettings.TriggerMode == TriggerType.Enter) TriggerSceneChange();
        }
        protected void HandleTriggerEnter2D(Collider2D other) 
        {
            if (!IsInLayerMask(other)) return;
            if (triggerSettings.TriggerMode == TriggerType.Enter) TriggerSceneChange();
        }
        protected void HandleTriggerStay(Collider other)
        {
            if (!IsInLayerMask(other)) return;
            if (triggerSettings.TriggerMode == TriggerType.Stay) TriggerSceneChangeAfterDelay();

            if (triggerSettings.TriggerMode == TriggerType.OnKeyPress)
            {
                if (other.TryGetComponent(out ISceneChangeTrigger sceneChangeTrigger))
                    TriggerSceneChangeAfterKeyPress(sceneChangeTrigger);
            }
        }
        protected void HandleTriggerStay2D(Collider2D other) 
        {
            if (!IsInLayerMask(other)) return;
            if (triggerSettings.TriggerMode == TriggerType.Stay) TriggerSceneChangeAfterDelay();
            
            if (triggerSettings.TriggerMode == TriggerType.OnKeyPress)
            {
                if (other.TryGetComponent(out ISceneChangeTrigger sceneChangeTrigger))
                    TriggerSceneChangeAfterKeyPress(sceneChangeTrigger);
            }
        }
        
        /// <summary>
        /// Detect gameObjects using the Overlap Physics approach.
        /// override this method to be used for 3D as well as 2D
        /// </summary>
        protected virtual void HandlePhysicsDetection() 
        {
            if (triggerSettings.SceneToLoad == "Loading") return;
            if (triggerSettings.Mode != DetectionMode.Physics) return;
            if (!IsSceneNameExist(triggerSettings.SceneToLoad)) return;            
        }

        /// <summary>
        /// Is Objct Collided belongs to the layer we are looking for. Used for 2D
        /// </summary>
        /// <param name="collider">A collider 2D attached to the object</param>
        /// <returns>true/false</returns>
        protected bool IsInLayerMask(Collider2D collider) => ((triggerSettings.LayersToDetect.value & (1 << collider.gameObject.layer)) > 0);
        
        /// <summary>
        /// Is Objct Collided belongs to the layer we are looking for. Used for 3D
        /// </summary>
        /// <param name="collider">A collider 3D attached to the object</param>
        /// <returns>true/false</returns>
        protected bool IsInLayerMask(Collider collider) => ((triggerSettings.LayersToDetect.value & (1 << collider.gameObject.layer)) > 0);
        
        /// <summary>
        /// Change Scene When a specific key is pressed
        /// </summary>
        /// <param name="trigger">An object Implementing the 'ISceneChangeTrigger' interface</param>
        protected void TriggerSceneChangeAfterKeyPress(ISceneChangeTrigger trigger)
        {
            if (trigger == null)
            {
                Debug.LogWarning("No Manual Scene Change Trigger Found, Use Enter or Stay Trigger Mode");
                return;
            }
            if (trigger.CanTrigger) TriggerSceneChange();
        }
        
        /// <summary>
        /// Trigger The Scene Change After a the specified delay
        /// </summary>
        protected void TriggerSceneChangeAfterDelay()
        {
            entryTime += Time.deltaTime;

            if (entryTime >= triggerSettings.SwitchingDelay)
            {
                entryTime = 0;
                TriggerSceneChange();               
            }
        }

        /// <summary>
        /// Trigger a Scene Change
        /// </summary>
        protected void TriggerSceneChange()
        {            
            if (triggerSettings.SceneToLoad == "Loading") return;
            if (!IsSceneNameExist(triggerSettings.SceneToLoad)) return;

            if(sceneSettings == null)
            {
                Debug.LogError("Scene Settings is null, Please Create a Scene Settings Object and assign It to " + gameObject.name, gameObject);
                return;
            }

            if(triggerSettings.LoadMode == SceneLoadMode.Single)
            {
                if (triggerSettings.AutoActivateScene)
                {
                    SceneLoadManager.Load(triggerSettings.SceneToLoad, sceneSettings);
                }
                else
                {
                    triggerSettings.SceneChangeTriggerEvent?.Invoke(triggerSettings.SceneToLoad);
                }
            }
            else
            {
                SceneLoadManager.Load(triggerSettings.SceneToLoad);
            }            
        }

        protected bool IsSceneNameExist(string sceneName)
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