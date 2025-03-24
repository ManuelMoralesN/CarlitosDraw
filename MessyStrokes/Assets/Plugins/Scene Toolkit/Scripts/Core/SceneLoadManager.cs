using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CG.SceneToolkit
{
    /// <summary>
    /// This is Main Singleton Component that Handles The Actual scene loading.
    /// this is automatically loaded from the resources folder. no need to attach this to a gameobject
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SceneLoadManager : MonoBehaviour
    { 
        public SceneSettings ActiveSceneSettings { get; private set; }

        #region Singleton
        public static SceneLoadManager Instance { get; private set; }
        private void Awake() => Instance = this;

        private void OnApplicationQuit()
        {
            Instance = null;
            //if(settings!= null) settings.TransitionSettings.TransitionMaterial.SetFloat("_Progress", 0f);
            if(ActiveSceneSettings != null) ActiveSceneSettings.TransitionSettings.TransitionMaterial.SetFloat("_Progress", 0f);
            Destroy(gameObject);
        }
        #endregion        

        /// <summary>
        /// The scene that is to be loaded
        /// </summary>
        public static string SceneToLoad { get; private set; }

        /// <summary>
        /// This Event is called when The Current scene is Fading In/Out
        /// the bool parameter passed here determines a fade in or out
        /// false = Fade Out
        /// true = Fade In
        /// </summary>
        public static event System.Action<bool> OnAudioFadeEvent;
        
        /// <summary>
        /// Used for manually activating the loaded scene based on a keypress
        /// </summary>
        public static ISceneActivationKey SceneActivationKey { get; private set; }

        /// <summary>
        /// This Event is Raised when a scene transition effect is finished
        /// </summary>
        public static event System.Action OnTransitionFinished;

        bool inProgress;       

        private void OnEnable() => SceneManager.activeSceneChanged += HandleSceneChange;
        private void OnDisable() => SceneManager.activeSceneChanged -= HandleSceneChange;

        /// <summary>
        /// Loads The scene given as a string value, 
        /// Scene Setting object is used to customize the scene transition and loading scene customization
        /// </summary>
        /// <param name="sceneToLoad">The name of the Scene That is to be loaded, must already be added to build settings</param>
        /// <param name="settings">A Scene Settings Scriptable object asset to handle the configuraion of scene load and transition</param>
        /// <param name="activationKey">Optionally passed, used to manually activate the loaded scene</param>
        public static void Load(string sceneToLoad, SceneSettings settings, ISceneActivationKey activationKey = null)
        {
            if (Instance.inProgress) return;

            SceneToLoad = sceneToLoad;
            SceneActivationKey = activationKey;            

            Instance.ActiveSceneSettings = settings;

            Instance.ActiveSceneSettings.Initialize();

            if (settings.TransitionSettings.EnableTransition)
            {
                Instance.StartCoroutine(Instance.FadeOutRoutine());
            }
            else
            {
                SceneManager.LoadScene("Loading");
            }
        }

        /// <summary>
        /// Asynchronously loads the scene specefied. The target scene is loaded additively.
        /// </summary>
        /// <param name="sceneToLoad">The target scene that is to be loaded</param>
        public static void Load(string sceneToLoad)
        {
            SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        }

        private IEnumerator FadeOutRoutine()
        {
            OnAudioFadeEvent?.Invoke(false);

            if (ActiveSceneSettings.TransitionSettings.AudioMixer != null && ActiveSceneSettings.TransitionSettings.EnableAudioFade)
            {
                Instance.StartCoroutine(ActiveSceneSettings.TransitionSettings.StartFade(-40f));
            }
            yield return StartCoroutine(Interpolate(0, 1.1f));
            SceneManager.LoadScene("Loading");            
        }        
        private IEnumerator Interpolate(float start, float end)
        {
            inProgress = true;            
            float current = start;
            for (float t = 0; current != end; t += Time.deltaTime * ActiveSceneSettings.TransitionSettings.TransitionSpeed)
            {
                current = Mathf.Clamp(Mathf.SmoothStep(start, end, t), 0f, 1.1f);                
                ActiveSceneSettings.TransitionSettings.TransitionMaterial.SetFloat("_Progress", current);                
                yield return null;
            }

            //Debug.Log("Fade Complete");

            OnTransitionFinished?.Invoke();

            inProgress = false;
        }        
        private void HandleSceneChange(Scene oldScene, Scene newScene)
        {
            if (newScene.name == "Loading") return;            

            if(ActiveSceneSettings == null) return;
            ActiveSceneSettings.Initialize();

            OnAudioFadeEvent?.Invoke(true);            

            if (ActiveSceneSettings.TransitionSettings.EnableTransition)
            {
                StartCoroutine(Interpolate(1.1f, 0));

                if (ActiveSceneSettings.TransitionSettings.AudioMixer != null && ActiveSceneSettings.TransitionSettings.EnableAudioFade)
                {
                    ActiveSceneSettings.TransitionSettings.AudioMixer.SetFloat(ActiveSceneSettings.TransitionSettings.ExposedParameter, -40f);
                    Instance.StartCoroutine(ActiveSceneSettings.TransitionSettings.StartFade(1f));
                }
            }
        }
    }
}