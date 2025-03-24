using UnityEngine;
using System.Collections;

namespace CG.SceneToolkit
{
    /// <summary>
    /// This Component is used to fade In/Out a single AudioSource during a scene load
    /// </summary>
    [AddComponentMenu("Scene Toolkit/Audio Source Fader")]
    [RequireComponent(typeof(AudioSource))]    
    public class AudioSourceFadeEventHandler : MonoBehaviour
    {
        [Tooltip("Duration of The Audio Fade")]
        [SerializeField] float fadeDuration = 1f;

        AudioSource audioSource;

        private void OnValidate() => audioSource = GetComponent<AudioSource>();
        private void Awake() => audioSource = GetComponent<AudioSource>();

        private void OnEnable() => SceneLoadManager.OnAudioFadeEvent += HandleAudioFadeEvent;
        private void OnDisable() => SceneLoadManager.OnAudioFadeEvent -= HandleAudioFadeEvent;
        private void HandleAudioFadeEvent(bool fademode) => StartCoroutine(StartFade(fademode));
        private IEnumerator StartFade(bool fadeIn)
        {
            if (audioSource == null) yield break;

            float targetVolume = (fadeIn) ? 1f : 0f;

            float currentTime = 0;
            float start = audioSource.volume;
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / fadeDuration);
                yield return null;
            }
            yield break;
        }
    }
}