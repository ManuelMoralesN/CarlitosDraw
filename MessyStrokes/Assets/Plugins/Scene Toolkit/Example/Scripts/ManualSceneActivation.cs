using UnityEngine;

namespace CG.SceneToolkit
{
    public class ManualSceneActivation : MonoBehaviour, ISceneActivationKey
    {
        [SerializeField] SceneSettings sceneSettings;
        public string Name => "Space Bar";
        public bool IsPressed => Input.GetKeyDown(KeyCode.Space);
        public void ChangeScene(string sceneToLoad) => SceneLoadManager.Load(sceneToLoad, sceneSettings, this);
    }
}