namespace CG.SceneToolkit
{
    /// <summary>
    /// This interface enables manual activation of the loaded scene based on a key press
    /// </summary>
    public interface ISceneActivationKey
    {
        /// <summary>
        /// Name of The Key that activates the scene
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Whether the key is being pressed or not
        /// </summary>
        bool IsPressed { get; }
    }
}