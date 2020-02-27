using System;
using System.IO;
using Assets.Scripts.Match.Exceptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Match
{
    public class CardManager
    {
        private Card[][] cards;

        private Transform fieldParent;

        private const string cardImagesFolder = "Cards";
        private const string defaultCardImage = "Default";
        private const float defaultCardSize = 0.3F; // TODO: should be calculated

        public void InitializeField(FieldParams prs)
        {
            Debug.Log("Start initializing field");

            ClearField();
            cards = new Card[prs.Width][];
            Object cardPrefab = GetCardPrefab();
            fieldParent = new GameObject().transform;
            fieldParent.gameObject.name = "Field";

            for (int i = 0; i < prs.Width; i++)
            {
                cards[i] = new Card[prs.Height];
                for (int j = 0; j < prs.Height; j++)
                {
                    Vector2 position = GetCardPosition(prs.Width, prs.Height, Screen.width, Screen.height, i, j);
                    GameObject gameObject = Object.Instantiate(cardPrefab, position, Quaternion.identity) as GameObject;
                    if (gameObject == null)
                        throw new Exception("Error instantiating card prefab");
                    gameObject.transform.localScale *= defaultCardSize;
                    gameObject.transform.SetParent(fieldParent);
                    Card card = gameObject.AddComponent<Card>();
                    cards[i][j] = card;
                }
            }

            Object.Destroy(cardPrefab);

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

        private Object GetCardPrefab()
        {
            Sprite sprite = Resources.Load<Sprite>(Path.Combine(cardImagesFolder, defaultCardImage));
            if (sprite == null)
                throw new ResourceLoadException("Error loading card prefab");

            GameObject prefab = new GameObject("Card");
            SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            if (prefab == null)
                throw new Exception("Error creating card prefab");

            return prefab;
        }

        private Vector2 GetCardPosition(int widthAmount, int heigthAmount, float screenWidth, float screenHeigth, int i, int j)
        {
            float x = screenWidth / (widthAmount + 1) * (i + 1);
            float y = screenHeigth / (heigthAmount + 1) * (j + 1);
            return Camera.main.ScreenToWorldPoint(new Vector3(x, y));
        }
    }

    public struct FieldParams
    {
        public int Width;

        public int Height;
    }
}
