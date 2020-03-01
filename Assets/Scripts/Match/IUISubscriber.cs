using UnityEngine;

namespace Assets.Scripts.Match
{
    public interface IUISubscriber
    {
        void Handle(GameObject gameObject);
    }
}
