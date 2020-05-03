using System;
using System.Collections;
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

        private bool isAnimating = false;
        private float scale = 1.1F;

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

        public void Update()
        {
            RotateToState();
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

        public IEnumerator AnimateMatch()
        {
            if (isAnimating)
                yield break;

            isAnimating = true;
            yield return new WaitForSeconds(0.6F);
            float s = transform.localScale.x * scale;
            transform.localScale = new Vector3(s, s, s);
            yield return new WaitForSeconds(0.2F);
            s = transform.localScale.x / scale;
            transform.localScale = new Vector3(s, s, s);
            isAnimating = false;
        }

        public IEnumerator AnimateUnmatch()
        {
            if (isAnimating)
                yield break;

            isAnimating = true;
            yield return new WaitForSeconds(0.6F);
            float s = transform.localScale.x / scale;
            transform.localScale = new Vector3(s, s, s);
            yield return new WaitForSeconds(0.2F);
            s = transform.localScale.x * scale;
            transform.localScale = new Vector3(s, s, s);
            isAnimating = false;
        }
    }
}
