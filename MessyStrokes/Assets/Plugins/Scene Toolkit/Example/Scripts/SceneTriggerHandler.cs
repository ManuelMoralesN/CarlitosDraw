using UnityEngine;

namespace CG.SceneToolkit
{
    public class SceneTriggerHandler : MonoBehaviour
    {
        [SerializeField] GameObject demoCube;
        [SerializeField] ParticleSystem demoParticleSystem;

        [Space]
        [SerializeField] GameObject[] objectsToHide;
        [SerializeField] GameObject[] objectsToShow;

        public void ActivateDemo()
        {
            if (objectsToHide.Length > 0)
                foreach (var obj in objectsToHide) { obj.SetActive(false); }

            if (objectsToShow.Length > 0)
                foreach (var obj in objectsToShow) { obj.SetActive(true); }

            demoCube.SetActive(true);

            demoParticleSystem.Play();
        }
    }
}