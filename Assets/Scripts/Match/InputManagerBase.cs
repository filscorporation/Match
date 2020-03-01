using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Match
{
    public abstract class InputManagerBase : IInputManager
    {
        private List<IUISubscriber> subscribers = new List<IUISubscriber>();

        public void AddSubscriber(IUISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public abstract void CheckForInput();

        protected bool ProcessInput(Vector2 input)
        {
            var wp = Camera.main.ScreenToWorldPoint(input);
            var position = new Vector2(wp.x, wp.y);

            Collider2D collider = Physics2D.OverlapPoint(position);
            if (collider == null)
                return false;

            foreach (IUISubscriber subscriber in subscribers)
            {
                subscriber.Handle(collider.gameObject);
            }

            return true;
        }
    }
}
