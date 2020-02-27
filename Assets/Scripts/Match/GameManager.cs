using UnityEngine;

namespace Assets.Scripts.Match
{
    public class GameManager : MonoBehaviour
    {
        public CardManager CardManager;

        public void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            CardManager = new CardManager();
            CardManager.InitializeField(new FieldParams { Height = 3, Width = 3 });
        }
    }
}
