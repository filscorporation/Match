using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Match.Exceptions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Match
{
    public class CardManager : IUISubscriber
    {
        private Card[][] cards;

        private Transform fieldParent;

        private const string cardImagesFolder = "Cards";
        private const string defaultCardPackage = "Default";
        private const float portraitScaleFactor = 0.6F;
        private const float landscapeScaleFactor = 1F;

        public void InitializeField(FieldParams prs)
        {
            Debug.Log("Start initializing field");

            ClearField();
            cards = new Card[prs.Height][];
            List<Object> cardPrefabs = GetCardPrefabs(defaultCardPackage);
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

            for (int j = 0; j < prs.Height; j++)
            {
                cards[j] = new Card[prs.Width];
                for (int i = 0; i < prs.Width; i++)
                {
                    Vector2 position = GetCardPosition(prs.Width, prs.Height, Screen.width, Screen.height, i, j);
                    int index = seed[j * prs.Width + i] / 2;
                    Card card = CreateCard(cardPrefabs[index], position, cardScale, index);
                    cards[j][i] = card;
                }
            }

            foreach (Object cardPrefab in cardPrefabs)
            {
                Object.Destroy(cardPrefab);
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

        public void Handle(GameObject gameObject)
        {
            if (gameObject.TryGetComponent<Card>(out Card card))
            {
                Debug.Log("Handling card click");
            }
        }

        private List<Object> GetCardPrefabs(string packagePath)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>(Path.Combine(cardImagesFolder, packagePath));
            if (sprites == null || sprites.Length == 0)
                throw new ResourceLoadException("Error loading card prefab");

            List<Object> prefabs = new List<Object>();
            foreach (Sprite sprite in sprites)
            {
                GameObject prefab = new GameObject("Card");
                SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;

                if (prefab == null)
                    throw new Exception("Error creating card prefab");
                prefabs.Add(prefab);
            }

            return prefabs;
        }

        private Vector2 GetCardPosition(int widthAmount, int heigthAmount, float screenWidth, float screenHeigth, int i, int j)
        {
            float x = screenWidth / (widthAmount + 1) * (i + 1);
            float y = screenHeigth / (heigthAmount + 1) * (j + 1);
            return Camera.main.ScreenToWorldPoint(new Vector3(x, y));
        }

        private float GetCardScale(int widthAmount, int heigthAmount, float screenWidth, float screenHeigth)
        {
            if (screenWidth > screenHeigth)
                 return Math.Min(landscapeScaleFactor / widthAmount, landscapeScaleFactor / heigthAmount);
            else
                return Math.Min(portraitScaleFactor / widthAmount, portraitScaleFactor / heigthAmount);
        }

        private Card CreateCard(Object prefab, Vector2 position, float scale, int index)
        {
            GameObject gameObject = Object.Instantiate(prefab, position, Quaternion.identity) as GameObject;
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
