using UnityEngine;

namespace Assets.Scripts.Match
{
    public interface IUISubscriber
    {
        void Handle(Vector3 click);
    }
}
