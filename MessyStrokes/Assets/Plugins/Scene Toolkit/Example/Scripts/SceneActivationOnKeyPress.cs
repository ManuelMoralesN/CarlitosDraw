using UnityEngine;

namespace CG.SceneToolkit
{
    public class SceneActivationOnKeyPress : MonoBehaviour, ISceneChangeTrigger
    {
        public bool CanTrigger => Input.GetKeyDown(KeyCode.Space);
        public string KeyName => "Space";

    }
}