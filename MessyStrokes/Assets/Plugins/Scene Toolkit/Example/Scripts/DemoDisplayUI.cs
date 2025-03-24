using UnityEngine;

namespace CG.SceneToolkit
{
    public class DemoDisplayUI : MonoBehaviour
    {
        [SerializeField] GameObject demoCanvas;

        private void OnEnable() => SceneLoadManager.OnTransitionFinished += HandleTransitionFinished;
        private void OnDisable() => SceneLoadManager.OnTransitionFinished -= HandleTransitionFinished;

        private void HandleTransitionFinished() => demoCanvas.SetActive(true);
    }
}