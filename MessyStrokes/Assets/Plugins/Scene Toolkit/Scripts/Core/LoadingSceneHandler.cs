using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CG.SceneToolkit
{
    /// <summary>
    /// This Component is used to customize the look and feel of the loading scene graphics
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Scene Toolkit/Loading Scene Customization")]
    public sealed class LoadingSceneHandler : MonoBehaviour
    {        
        SceneSettings settings;

        [Tooltip("The Image that will be shown during the loading process. set it in the 'Scene Setting' scriptable object ")]
        [Space]
        [SerializeField] Image backgroundImage;

        [Tooltip("The message that will be shown during the loading process. set it in the 'Scene Setting' scriptable object ")]
        [SerializeField] TextMeshProUGUI loadingInfoMessage;        

        [Tooltip("A slider used to show the loading progress, can be turned off in the 'Scene Setting' ")]
        [Space]
        [SerializeField] Slider loadingProgress;

        [Tooltip("Show the loading progress as a percentage, can be turned off in the 'Scene Setting' ")]
        [SerializeField] TextMeshProUGUI loadingPercentage;

        AsyncOperation operation;

        private void Awake()
        {
            settings = SceneLoadManager.Instance.ActiveSceneSettings;
            
            if (settings == null) Debug.LogError("Scene Settings Object is Not added. Please create and addd a 'Scene Settings Object here'", 
                                                 gameObject);

            backgroundImage.gameObject.SetActive(true);

            if (settings.BackgroundImages.Length > 0)
                backgroundImage.sprite = settings.RandomBackgroundImage;

            if (settings.InfoMessages.Length > 0)
                loadingInfoMessage.text = settings.RandomInfoMessage;
        }
        private void Start()
        {
            loadingPercentage.gameObject.SetActive(settings.ShowLoadingPercentage);
            loadingProgress.gameObject.SetActive(settings.ShowLoadingProgress);
            StartCoroutine(CycleLoadingMessagesRoutine(settings.InfoMessageDelay));
            StartCoroutine(LoadLevelRoutine(SceneLoadManager.SceneToLoad, SceneLoadManager.SceneActivationKey));
        }
        private IEnumerator LoadLevelRoutine(string sceneName, ISceneActivationKey sceneActivationKey, bool autoActivate = false)
        {
            operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);

                loadingProgress.value = progress;
                loadingPercentage.text = "Loading: " + progress * 100f + " %";

                if (autoActivate)
                {
                    operation.allowSceneActivation = true;
                }
                else
                {
                    //Change the Text to show the Scene is ready
                    loadingPercentage.gameObject.SetActive(true);

                    if (sceneActivationKey != null)
                        loadingPercentage.text = "Press " + sceneActivationKey.Name + " to continue";

                    if (operation.progress >= 0.9f)
                    {

                        loadingInfoMessage.gameObject.SetActive(false);

                        //Activate the Scene
                        if (sceneActivationKey != null)
                        {
                            if (sceneActivationKey.IsPressed)
                                operation.allowSceneActivation = true;
                        }
                        else
                        {
                            operation.allowSceneActivation = true;
                        }
                    }
                    loadingInfoMessage.gameObject.SetActive(true);
                }

                yield return null;
            }
        }
        private IEnumerator CycleLoadingMessagesRoutine(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                loadingInfoMessage.text = settings.RandomInfoMessage;
            }
        }    
    }
}