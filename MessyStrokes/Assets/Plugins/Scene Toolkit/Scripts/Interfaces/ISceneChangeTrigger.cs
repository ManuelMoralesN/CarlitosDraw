namespace CG.SceneToolkit
{
    /// <summary>
    /// Used as a manual trigger for changing the scene,
    /// </summary>
    public interface ISceneChangeTrigger
    {
        public string KeyName {  get; }

        /// <summary>
        /// Whether or not to trigger the scene change
        /// </summary>
        public bool CanTrigger { get; }
    }
}