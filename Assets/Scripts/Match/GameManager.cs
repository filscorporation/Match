using UnityEngine;

namespace Assets.Scripts.Match
{
    public class GameManager : MonoBehaviour
    {
        public CardManager CardManager;

        public UIManager UIManager;

        public IInputManager InputManager;

        private bool getInput = true;

        public void Start()
        {
            StartGame();
        }

        public void Update()
        {
            if (getInput)
                InputManager.CheckForInput();
        }

        public void StartGame()
        {
            CardManager = new CardManager();
            CardManager.InitializeField(new FieldParams { Height = 5, Width = 6 });
            InputManager = new PCInputManager();
            InputManager.AddSubscriber(CardManager);
        }
    }
}
