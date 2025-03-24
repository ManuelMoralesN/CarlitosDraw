using UnityEngine;

namespace CG.SceneToolkit
{
    /// <summary>
    /// Used as a Scriptable Object Data Container. It contains all the settings needed to
    /// configure, Scene Transition, Audio Fade and Loading Scene Customization
    /// </summary>
    [CreateAssetMenu(fileName ="New Scene Settings", menuName = "Scene Toolkit/Scene Setting")]
    public class SceneSettings : ScriptableObject
    {
        [HideInInspector] public string currentScenePath;

        [Header("Loading Scene Customization")]
        [Tooltip("An array of images to be used as the loading scene background, when added multiple image a random one is selected at each load")]
        [SerializeField] Sprite[] backgroundImages;
        
        [Tooltip("An array of messages to be used as the loading scene info message, when added multiples a random one is selected at each load")]
        [SerializeField] [TextArea] string[] infoMessages;

        [Tooltip("Time delay while cycling between each info messages")]
        [SerializeField] float infoMessageDelay = 1.5f;

        [Space]
        [Tooltip("Show/Hide Loading completion percentage")]
        [SerializeField] bool showLoadingPercentage;

        [Tooltip("Show/Hide Loading progress bar")]
        [SerializeField] bool showLoadingProgress;
        
        [Space]
        [SerializeField] SceneTransitionSettings transitionSettings;

        /// <summary>
        /// An array of messages to be used as the loading scene info message, when added multiples a random one is selected at each load
        /// </summary>
        public string[] InfoMessages => infoMessages;

        /// <summary>
        /// Time delay while cycling between each info messages
        /// </summary>
        public float InfoMessageDelay => infoMessageDelay;

        /// <summary>
        /// An array of images to be used as the loading scene background, when added multiple image a random one is selected at each load
        /// </summary>        
        public Sprite[] BackgroundImages => backgroundImages;

        /// <summary>
        /// Show/Hide Loading progress bar
        /// </summary>
        public bool ShowLoadingProgress => showLoadingProgress;
        /// <summary>
        /// Show/Hide Loading completion percentage
        /// </summary>
        public bool ShowLoadingPercentage => showLoadingPercentage;
        
        public SceneTransitionSettings TransitionSettings => transitionSettings;       
        /// <summary>
        /// Get a random Info messsage if more than one message is available, otherwise get the first one.
        /// </summary>
        public string RandomInfoMessage => infoMessages[infoMessages.Length > 0 ? Random.Range(0, infoMessages.Length) : 0];
        /// <summary>
        /// Get a random image. if more than one message is available, otherwise get the first one.
        /// </summary>
        public Sprite RandomBackgroundImage => backgroundImages[backgroundImages.Length > 0 ? Random.Range(0, backgroundImages.Length) : 0];
        
        private void OnValidate()
        {            
            if (transitionSettings.TransitionMaterial == null) return;
            //transitionSettings.TransitionMaterial.SetColor("_FadeTintColor", transitionSettings.FadeTintColor);
            transitionSettings.TransitionMaterial.SetTexture("_PatternTexture", transitionSettings.Pattern);
            transitionSettings.TransitionMaterial.SetFloat("_UseFade", transitionSettings.TransitionType == TransitionType.Fade ? 1f : 0f);
        }
        
        public void Initialize()
        {
            if (transitionSettings.TransitionMaterial == null) return;
            //transitionSettings.TransitionMaterial.SetColor("_FadeTintColor", transitionSettings.FadeTintColor);
            transitionSettings.TransitionMaterial.SetTexture("_PatternTexture", transitionSettings.Pattern);
            transitionSettings.TransitionMaterial.SetFloat("_UseFade", transitionSettings.TransitionType == TransitionType.Fade ? 1f : 0f);
        }

        private void OnDisable() => transitionSettings.TransitionMaterial.SetFloat("_Progress", 0f);
    }
}