using UnityEngine;

namespace Assets.Scripts.Match
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

        private CardState state = CardState.Unactive;

        private const float rotationSpeed = 0.05F;

        public CardState State
        {
            get { return state; }
            set { SetState(value); }
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

        }

        protected void Hide()
        {

        }

        public void Update()
        {
            RotateToState();
        }

        private void RotateToState()
        {
            Quaternion target = State == CardState.Unactive ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed);
        }
    }
}
