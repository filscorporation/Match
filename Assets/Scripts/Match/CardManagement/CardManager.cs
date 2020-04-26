using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Match.Exceptions;
using Assets.Scripts.Match.InputManagement;
using Assets.Scripts.Match.Networking;
using NetworkShared.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Match.CardManagement
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
        private const float portraitScaleFactor = 0.9F;
        private const string backgroundFileName = "Background";

        private bool isAnimating = false;
        private const float cardAnimationLength = 0.6F; public AudioClip AudioClip;

        private const string soundFolderName = "Sounds";
        private AudioSource audioSource;
        private const string cardFlipSoundName = "CardFlipSound";
        private AudioClip cardFlipSound;

        private const string backgroundObjectName = "Background";

        public CardManager(GameManager gameManager)
        {
            this.gameManager = gameManager;

            PrepareSound();
        }

        public void InitializeField(FieldParams prs, CardPack cardPack)
        {
            Debug.Log("Start initializing field");

            if (cardPack == null)
                throw new ArgumentNullException(nameof(cardPack));

            List<Object> cardPrefabs = new List<Object>();
            try
            {
                ClearField();

                SetBackground(cardPack.Name);

                cards = new Card[prs.Height][];
                cardPrefabs = GetCardPrefabs(cardPack.Name, out Vector2 cardSize);
                if (cardPrefabs.Count < prs.Width * prs.Height / 2)
                    throw new Exception("Not enough images for field size");

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
                        card.X = j;
                        card.Y = i;
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

        public void InitializeField(FieldParams prs, CardPack cardPack, int[,] fieldData)
        {
            Debug.Log("Start initializing field");

            if (cardPack == null)
                throw new ArgumentNullException(nameof(cardPack));

            List<Object> cardPrefabs = new List<Object>();
            try
            {
                ClearField();
                cards = new Card[prs.Height][];
                cardPrefabs = GetCardPrefabs(cardPack.Name, out Vector2 cardSize);
                if (cardPrefabs.Count < prs.Width * prs.Height / 2)
                    throw new Exception("Not enough images for field size");
                
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
                        int index = fieldData[j, i];
                        Card card = CreateCard(cardPrefabs[index], position, cardScale, index);
                        card.X = j;
                        card.Y = i;
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

        /// <summary>
        /// Handles click on card by X Y
        /// </summary>
        /// <param name="data"></param>
        public void Handle(PlayersTurnData data)
        {
            Handle(cards[data.CardRevealedX][data.CardRevealedY].gameObject, false);
        }

        /// <summary>
        /// Handles click on some object
        /// </summary>
        /// <param name="gameObject">Object that was clicked</param>
        /// <param name="notifyServer"></param>
        public void Handle(GameObject gameObject, bool notifyServer = true)
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
                    if (GameSettings.IsOnline && notifyServer)
                    {
                        PlayersTurnData data = new PlayersTurnData();
                        data.CardRevealedX = card.X;
                        data.CardRevealedY = card.Y;
                        NetworkManager.Instance.SendPlayersTurn(data);
                    }
                    PlayCardFlipSound();
                    card.State = CardState.Active;
                }
                if (lastActive == null)
                {
                    lastActive = card;
                }
                else
                {
                    isAnimating = true;
                    PlayCardFlipSound();
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
                        // TODO: remove
                        gameManager.StartCoroutine(PlayCardFlipSound(1F));
                    }
                    lastActive = null;
                }
            }
        }

        private void SetBackground(string cardPackName)
        {
            GameObject backgroundObject = GameObject.Find(backgroundObjectName);
            Sprite backgroundSprite = Resources.Load<Sprite>(Path.Combine(cardPacksFolder, cardPackName, backgroundFileName));
            SpriteRenderer sr = backgroundObject.GetComponent<SpriteRenderer>();
            sr.sprite = backgroundSprite;

            backgroundObject.transform.localScale = new Vector3(1, 1, 1);

            float width = sr.sprite.bounds.size.x;
            float height = sr.sprite.bounds.size.y;

            float worldScreenHeight = Camera.main.orthographicSize * 2.0F;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
            float scale = Math.Max(worldScreenWidth / width, worldScreenHeight / height);

            backgroundObject.transform.localScale = new Vector2(scale, scale);
        }

        private void PrepareSound()
        {
            audioSource = Resources.FindObjectsOfTypeAll<AudioSource>().FirstOrDefault();
            if (audioSource == null)
            {
                Debug.LogWarning("No audiosource");
                return;
            }

            cardFlipSound = Resources.Load<AudioClip>(Path.Combine(soundFolderName, cardFlipSoundName));
        }

        private void PlayCardFlipSound()
        {
            Debug.Log("Playing");
            audioSource.PlayOneShot(cardFlipSound);
        }

        IEnumerator PlayCardFlipSound(float delay)
        {
            yield return new WaitForSeconds(delay);
            audioSource.PlayOneShot(cardFlipSound);
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
            float yOffset = 0.5F;
            float x = (i - (float)widthAmount / 2 + 0.5F) * cardSize.x * gapIndex;
            float y = (j - (float)heigthAmount / 2 + 0.5F) * cardSize.y * gapIndex - yOffset;
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
