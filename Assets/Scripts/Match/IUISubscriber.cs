using UnityEngine;

namespace Assets.Scripts.Match
{
    public interface IInputSubscriber
    {
        void Handle(GameObject gameObject);
    }
}
