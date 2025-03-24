using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

namespace CG.SceneToolkit
{
    /// <summary>
    /// Type of transition to be applied when changing the scene
    /// </summary>
    public enum TransitionType { Fade, Pattern }

    /// <summary>
    /// This Class contains all the values releated to Transition FX and Audio Fading,
    /// When Changing Scene
    /// </summary>
    [System.Serializable]
    public class SceneTransitionSettings
    {
        [Tooltip("A GrayScale Image, used as transition pattern")]
        [SerializeField] Texture2D pattern = null;

        [Tooltip("Enable or Disable Scene Transition")]
        [SerializeField] bool enableTransition = false;

        //[Tooltip("Color to fade In/Out, when Fade mode is used")]
        //[SerializeField] Color fadeTintColor = Color.white;

        [Tooltip("The Speed of the Scene transition")]
        [SerializeField, Range(.25f, 5f)] float transitionSpeed = 1f;

        [Tooltip("Fade or a pattern to be used")]
        [SerializeField] TransitionType transitionType = TransitionType.Fade;

        [Tooltip("Enable or Disable Audio Fade during Transition")]
        [SerializeField] bool enableAudioFade = false;

        [Tooltip("The main Audio mixer in use")]
        [SerializeField] AudioMixer audioMixer;

        [Tooltip("Name of the exposed parameter set inside the audio mixer, Must Match Exactly")]
        [SerializeField] string exposedParameter = "";

        [Tooltip("Duration of the Audio Fade")]
        [SerializeField] float audioFadeDuration = 1f;

        Material transitionMaterial;

        /// <summary>
        /// The Speed of the Scene transition
        /// </summary>
        public float TransitionSpeed => transitionSpeed;

        /// <summary>
        /// Enable or Disable Scene Transition
        /// </summary>
        public bool EnableTransition => enableTransition;

        /// <summary>
        /// The Transition Material, Added as a renderer feature. It is Loaded automatically from the 'Resources' folder
        /// </summary>
        public Material TransitionMaterial
        {
            get
            {
                if(transitionMaterial != null)
                    return transitionMaterial;

                transitionMaterial = Resources.Load("TransitionMaterial") as Material;
                return transitionMaterial;
            }
        }

        /// <summary>
        /// Fade or a pattern to be used
        /// </summary>
        public TransitionType TransitionType => transitionType;

        /// <summary>
        /// Color to fade In/Out, when Fade mode is used
        /// </summary>
        //public Color FadeTintColor => fadeTintColor;

        /// <summary>
        /// A GrayScale Image, used as transition pattern
        /// </summary>        
        public Texture2D Pattern => pattern;

        /// <summary>
        /// The main Audio mixer in use
        /// </summary>
        public AudioMixer AudioMixer => audioMixer;

        /// <summary>
        /// Enable or Disable Audio Fade during Transition
        /// </summary>
        public bool EnableAudioFade => enableAudioFade;

        /// <summary>
        /// Name of the exposed parameter set inside the audio mixer, Must Match Exactly
        /// </summary>
        public string ExposedParameter => exposedParameter;        

        /// <summary>
        /// Handles the fading In/Out of audio volume, based on the targetVolume passed.
        /// </summary>
        /// <param name="targetVolume">The volume in decible unit</param>
        /// <returns>Returns a Co-Routine</returns>
        public IEnumerator StartFade(float targetVolume)
        {
            float currentTime = 0;
            float currentVol;
            audioMixer.GetFloat(exposedParameter, out currentVol);
            currentVol = Mathf.Pow(10, currentVol / 20);
            float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
            while (currentTime < audioFadeDuration)
            {
                currentTime += Time.deltaTime;
                float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / audioFadeDuration);
                audioMixer.SetFloat(exposedParameter, Mathf.Log10(newVol) * 20);
                yield return null;
            }            
            yield break;
        }
    }
}