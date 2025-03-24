using UnityEngine;

namespace CG.SceneToolkit
{
    /// <summary>
    /// This Component is used to create a trigger zone/area, which upon activating/interacted can be
    /// used to trigger a scene change. This is used for 2D objects and collider
    /// </summary>
    [AddComponentMenu("Scene Toolkit/Scene Change Trigger 2D")]
    [RequireComponent(typeof(BoxCollider2D))]
    public sealed class SceneSwitchTrigger2D : TriggerBase
    {
        [Tooltip("The area within which the detection will occur")]
        [SerializeField] Vector2 detectionArea = new Vector2(3f, 1f);

        BoxCollider2D boxCollider;
        Collider2D[] colliders = new Collider2D[10];

        private void OnValidate()
        {
            if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

            if(triggerSettings != null)
            {
                boxCollider.size = detectionArea;
                boxCollider.isTrigger = true;
                boxCollider.enabled = (triggerSettings.Mode == DetectionMode.Collider);

                if (triggerSettings.SceneToLoad == "Loading")
                    Debug.LogWarning("Loading Scene Can Not Be used Here...Use a different scene to load");

                if (!IsSceneNameExist(triggerSettings.SceneToLoad))
                    Debug.LogWarning("Scene is Not in the buildindex");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) => HandleTriggerEnter2D(collision);
        private void OnTriggerStay2D(Collider2D collision) => HandleTriggerStay2D(collision);
        private void FixedUpdate() => HandlePhysicsDetection();

        protected override void HandlePhysicsDetection()
        {
            base.HandlePhysicsDetection();
            Collider2D collider = Physics2D.OverlapBox(transform.position, detectionArea * .5f, 360f, triggerSettings.LayersToDetect);
            isDetected = collider != null;
            if (isDetected) TriggerSceneChangeAfterDelay();
        }

        private void OnDrawGizmos()
        {
            if (triggerSettings.Mode == DetectionMode.Collider) return;

            Gizmos.color = (isDetected) ? Color.green : Color.white;
            Gizmos.DrawWireCube(transform.position, detectionArea);
        }
    }
}