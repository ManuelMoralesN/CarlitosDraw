using UnityEngine;

namespace CG.SceneToolkit
{
    /// <summary>
    /// This Component is used to create a trigger zone/area, which upon activating/interacted can be
    /// used to trigger a scene change. This is used for 3D objects and collider
    /// </summary>
    [AddComponentMenu("Scene Toolkit/Scene Change Trigger 3D")]
    [RequireComponent(typeof(BoxCollider))]
    public sealed class SceneSwitchTrigger3D : TriggerBase
    {
        [Space(20)]
        [Tooltip("The area within which the detection will occur")]
        [SerializeField] Vector3 detectionArea = new Vector3(5f, 1f, 5f);

        BoxCollider boxCollider;
        Collider[] colliders = new Collider[10];

        private void OnValidate()
        {
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();

            boxCollider.isTrigger = true;

            if(triggerSettings != null)
            {
                boxCollider.size = detectionArea;
                boxCollider.enabled = (triggerSettings.Mode == DetectionMode.Collider);

                if (triggerSettings.SceneToLoad == "Loading")
                    Debug.LogWarning("Loading Scene Can Not Be used Here...Use a different scene to load");

                //if (!IsSceneNameExist(triggerSettings.SceneToLoad))
                //    Debug.LogWarning("Scene is Not in the buildindex");

            }
        }
        private void FixedUpdate() => HandlePhysicsDetection();
        private void OnTriggerStay(Collider other) => HandleTriggerStay(other);
        private void OnTriggerEnter(Collider other) => HandleTriggerEnter(other);
        protected override void HandlePhysicsDetection()
        {
            base.HandlePhysicsDetection();

            if (triggerSettings.Mode != DetectionMode.Physics) return;

            isDetected = Physics.OverlapBoxNonAlloc(transform.position, detectionArea * .5f, colliders, transform.rotation,
                                        triggerSettings.LayersToDetect) > 0;
            if(isDetected) TriggerSceneChangeAfterDelay();
        }
        private void OnDrawGizmos()
        {
            if (triggerSettings.Mode == DetectionMode.Collider) return;

            Gizmos.color = (isDetected) ? Color.green : Color.white;
            Gizmos.DrawWireCube(transform.position, detectionArea);
        }
    }
}