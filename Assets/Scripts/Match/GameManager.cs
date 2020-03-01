using UnityEngine;

namespace Assets.Scripts.Match
{
    public class GameManager : MonoBehaviour
    {
        public CardManager CardManager;

        public UIManager UIManager;

        public IInputManager InputManager;

        private bool getInput = true;

        private const string defaultCardPackage = "DefaultPack";

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
            CardManager = new CardManager(this);
            CardManager.InitializeField(new FieldParams { Height = 5, Width = 6 }, defaultCardPackage);
            InputManager = new PCInputManager();
            InputManager.AddSubscriber(CardManager);
        }

        public void Score()
        {

        }
    }
}
