using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Match.Exceptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Match
{
    public class CardManager : IInputSubscriber
    {
        private GameManager gameManager;

        private Card[][] cards;
        private Card lastActive = null;

        private Transform fieldParent;

        private const string cardPacksFolder = "CardPacks";
        private const string cardsFolder = "Cards";
        private const string cardbackFileName = "CardBack";
        private const float portraitScaleFactor = 0.75F;

        private bool isAnimating = false;
        private const float cardAnimationLength = 1F;

        public CardManager(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void InitializeField(FieldParams prs, CardPack cardPack)
        {
            Debug.Log("Start initializing field");

            List<Object> cardPrefabs = new List<Object>();
            try
            {
                ClearField();
                cards = new Card[prs.Height][];
                cardPrefabs = GetCardPrefabs(cardPack.Name, out Vector2 cardSize);
                if (cardPrefabs.Count < prs.Width * prs.Height / 2)
                    throw new Exception("Not enougth images for field size");

                List<int> seed = new List<int>(prs.Width * prs.Height);
                for (int i = 0; i < prs.Width * prs.Height; i++)
                {
                    seed.Add(i);
                }

                seed.Shuffle();

                fieldParent = new GameObject().transform;
                fieldParent.gameObject.name = "Field";

                float cardScale = GetCardScale(prs.Width, prs.Height, Screen.width, Screen.height);
                cardSize = new Vector2(cardSize.x * cardScale, cardSize.y * cardScale);

                for (int j = 0; j < prs.Height; j++)
                {
                    cards[j] = new Card[prs.Width];
                    for (int i = 0; i < prs.Width; i++)
                    {
                        Vector2 position = GetCardPosition(prs.Width, prs.Height, Screen.width, Screen.height, i, j,
                            cardSize);
                        int index = seed[j * prs.Width + i] / 2;
                        Card card = CreateCard(cardPrefabs[index], position, cardScale, index);
                        cards[j][i] = card;
                    }
                }
            }
            finally
            {
                foreach (Object cardPrefab in cardPrefabs)
                {
                    Object.Destroy(cardPrefab);
                }
            }

            Debug.Log("Finish initializing field");
        }

        public void ClearField()
        {
            if (fieldParent != null)
            {
                Object.Destroy(fieldParent);
                fieldParent = null;
            }

            if (cards == null)
                return;

            foreach (Card[] cardList in cards)
            {
                foreach (Card card in cardList)
                {
                    Object.Destroy(card);
                }
            }
        }

        public bool IsAnimating() => isAnimating;

        public void Handle(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<Card>(out Card card))
            {
                Debug.Log($"Handling card {card.Index}");
                if (lastActive == card)
                    return;
                if (card.State == CardState.Revealed)
                    return;
                if (card.State == CardState.Unactive)
                {
                    card.State = CardState.Active;
                }
                if (lastActive == null)
                {
                    lastActive = card;
                }
                else
                {
                    isAnimating = true;
                    Task.Run(() =>
                    {
                        Thread.Sleep((int)(cardAnimationLength * 1000));
                        isAnimating = false;
                    });

                    if (lastActive.Index == card.Index)
                    {
                        Debug.Log($"Match");
                        Match(lastActive, card);
                    }
                    else
                    {
                        Debug.Log($"Unmatch");
                        Unmatch(lastActive, card);
                    }
                    lastActive = null;
                }
            }
        }

        private void Match(Card a, Card b)
        {
            gameManager.Match();
            a.State = CardState.Revealed;
            b.State = CardState.Revealed;

            CheckForGameEnd();
        }

        private void Unmatch(Card a, Card b)
        {
            gameManager.Unmatch();
            a.State = CardState.Unactive;
            b.State = CardState.Unactive;
        }

        private void CheckForGameEnd()
        {
            if (cards.All(cs => cs.All(c => c.State == CardState.Revealed)))
            {
                gameManager.EndGame();
            }
        }

        private List<Object> GetCardPrefabs(string packagePath, out Vector2 cardSize)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(Path.Combine(cardPacksFolder, packagePath, cardsFolder));
            Sprite cardbackSprite = Resources.Load<Sprite>(Path.Combine(cardPacksFolder, packagePath, cardbackFileName));
            if (sprites == null || sprites.Length == 0)
                throw new ResourceLoadException("Error loading card prefab");

            cardSize = sprites.First().rect.size / sprites.First().pixelsPerUnit;

            List<Object> prefabs = new List<Object>();
            foreach (Sprite sprite in sprites)
            {
                GameObject prefab = new GameObject("Card");
                SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;

                GameObject back = new GameObject("CardBack");
                back.transform.SetParent(prefab.transform);
                back.transform.localPosition = new Vector3(0, 0, 0.01F);
                sr = back.AddComponent<SpriteRenderer>();
                sr.sprite = cardbackSprite;

                prefabs.Add(prefab);
            }

            return prefabs;
        }

        private Vector2 GetCardPosition(int widthAmount, int heigthAmount, float screenWidth, float screenHeigth, int i, int j, Vector2 cardSize)
        {
            // Even placement strategy
            //float x = screenWidth / (widthAmount + 1) * (i + 1);
            //float y = screenHeigth / (heigthAmount + 1) * (j + 1);

            // Dense
            float gapIndex = 1.05F;
            float x = (i - (float)widthAmount / 2 + 0.5F) * cardSize.x * gapIndex;
            float y = (j - (float)heigthAmount / 2 + 0.5F) * cardSize.y * gapIndex;
            return new Vector3(x, y);
        }

        private float GetCardScale(int widthAmount, int heigthAmount, float screenWidth, float screenHeigth)
        {
            return Math.Min(portraitScaleFactor / widthAmount, portraitScaleFactor / heigthAmount);
        }

        private Card CreateCard(Object prefab, Vector2 position, float scale, int index)
        {
            GameObject gameObject = Object.Instantiate(prefab, position, Quaternion.Euler(0, 180, 0)) as GameObject;
            if (gameObject == null)
                throw new Exception("Error instantiating card prefab");
            gameObject.transform.localScale *= scale;
            gameObject.transform.SetParent(fieldParent);
            gameObject.AddComponent<BoxCollider2D>();
            Card card = gameObject.AddComponent<Card>();
            card.Index = index;
            return card;
        }
    }

    public struct FieldParams
    {
        public int Width;

        public int Height;
    }
}
