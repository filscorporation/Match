using UnityEngine;

namespace Assets.Scripts.Match.InputManagement
{
    /// <summary>
    /// Interface for subscribers to input
    /// </summary>
    public interface IInputSubscriber
    {
        /// <summary>
        /// Handles click on some object
        /// </summary>
        /// <param name="gameObject">Object that was clicked</param>
        /// <param name="notifyServer"></param>
        void Handle(GameObject gameObject, bool notifyServer = true);
    }
}
