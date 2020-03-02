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
        private List<IInputSubscriber> subscribers = new List<IInputSubscriber>();

        public void AddSubscriber(IInputSubscriber subscriber)
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

            foreach (IInputSubscriber subscriber in subscribers)
            {
                subscriber.Handle(collider.gameObject);
            }

            return true;
        }
    }
}
