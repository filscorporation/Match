using System;
using UnityEngine;

namespace Assets.Scripts.Match.CardManagement
{
    public enum CardState
    {
        Unactive,
        Active,
        Revealed,
    }

    public class Card : MonoBehaviour
    {
        public int Index;
        public int X;
        public int Y;

        private CardState state = CardState.Unactive;

        private const float rotationSpeed = 0.1F;
        private const float rotationDelay = 1F;
        private float rotationDelayTimer = 0F;

        public CardState State
        {
            get => state;
            set => SetState(value);
        }

        private void SetState(CardState state)
        {
            if (this.state == state)
                return;
            if (this.state != CardState.Active && state != CardState.Unactive)
            {
                Reveal();
            }
            else if (state == CardState.Unactive)
            {
                Hide();
            }
            this.state = state;
        }

        protected void Reveal()
        {
            Debug.Log($"Reveal card {Index}");
            rotationDelayTimer = 0F;
        }

        protected void Hide()
        {
            Debug.Log($"Hide card {Index}");
            rotationDelayTimer = rotationDelay;
        }

        public void Update()
        {
            RotateToState();
        }

        private void RotateToState()
        {
            Quaternion target;
            if (State != CardState.Unactive || Math.Abs(rotationDelayTimer) > Mathf.Epsilon)
            {
                target = Quaternion.Euler(0, 0, 0);
                rotationDelayTimer = Math.Max(0F, rotationDelayTimer - Time.deltaTime);
            }
            else
            {
                target = Quaternion.Euler(0, 180, 0);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed);
        }
    }
}
