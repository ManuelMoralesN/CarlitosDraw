using UnityEngine;
using UnityEngine.Events;

namespace CG.SceneToolkit
{
    /// <summary>
    /// Type of trigger Interaction.
    /// Enter => OnTriggerEnter, Stay => OnTriggerStay, 
    /// OnKeyPress => When the designated key is being pressed. Requires the ISceneChangeTrigger Interface
    /// </summary>
    public enum TriggerType { Enter, Stay, OnKeyPress }    

    /// <summary>
    /// Whether to use Trigger Collider or Overlap Physics.
    /// </summary>
    public enum DetectionMode { Collider, Physics }

    /// <summary>
    /// Specefies how to load the scene, Additive load ignores all the transitions and loadings screens
    /// </summary>
    public enum SceneLoadMode { Single, Additive }

    /// <summary>
    /// A Class that holds all the configuraion values for Scene transition and audio fade settings
    /// </summary>
    [System.Serializable]
    public class SceneChangeTriggerSettings
    {        
        /// <summary>
        /// A custom class that inherits from 'UnityEvent' passes a 'string' parameter when raised.
        /// </summary>
        [System.Serializable] public class SceneChangeEvent : UnityEvent<string> { }
        
        [Tooltip("Name of the scene this trigger will load, scene must already be added to build settings")]
        [SerializeField] string sceneToLoad;
        
        [Tooltip("Determines whether to use a collider based approach or simple physics overlap")]
        [SerializeField] DetectionMode detectionMode;
        
        [Tooltip("Layer/Layers that this trigger will detect")]
        [SerializeField] LayerMask layersToDetect;

        [Tooltip("Determies what type of trigger is used")]
        [SerializeField] TriggerType triggerType;

        [Tooltip("The delay/wait time before switching the scene")]
        [SerializeField] float switchingDelay;

        [Tooltip("To automatically activate the scene when loading is complete")]
        [SerializeField] bool autoActivateScene = true;

        [Tooltip("Specefies how to load the scene, Additive load ignores all the transitions and loadings screens")]
        [SerializeField] SceneLoadMode loadMode = SceneLoadMode.Single;

        [Tooltip("A Unity Event that is to be used when manually activating the loaded scene")]
        [SerializeField] SceneChangeEvent sceneChangeEvent;

        /// <summary>
        /// Name of the scene this trigger will load, scene must already be added to build settings
        /// </summary>
        public string SceneToLoad => sceneToLoad;

        /// <summary>
        /// Determines whether to use a collider based approach or simple physics overlap
        /// </summary>
        public DetectionMode Mode => detectionMode;

        /// <summary>
        /// The delay/wait time before switching the scene
        /// </summary>
        public float SwitchingDelay => switchingDelay;

        /// <summary>
        /// Determies what type of trigger is used.
        /// Enter = OnTriggerEnter,
        /// Stay = OnTriggerStay,
        /// OnKeyPress = Manually Triggered when a key is pressed, uses ISceneChangeTrigger interface
        /// </summary>
        public TriggerType TriggerMode => triggerType;

        /// <summary>
        /// Layer/Layers that this trigger will detect
        /// </summary>
        public LayerMask LayersToDetect => layersToDetect;

        /// <summary>
        /// To automatically activate the scene when loading is complete
        /// </summary>
        public bool AutoActivateScene => autoActivateScene;

        /// <summary>
        /// Specefies how to load the scene, Additive load ignores all the transitions and loadings screens
        /// </summary>
        public SceneLoadMode LoadMode => loadMode;

        /// <summary>
        /// In case of manual scene activation this event is used to load and activate the required scene
        /// </summary>
        public SceneChangeEvent SceneChangeTriggerEvent => sceneChangeEvent;        
    }
}