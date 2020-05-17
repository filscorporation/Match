using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Match.CardManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Assets.Scripts.Match.UI
{
    public class CardPackUIManager : MonoBehaviour
    {
        public bool Freeze = false;

        private readonly float minSwipeDistance = Screen.width/10F;
        private int touchID = -1;
        private Vector2? swipeStartPoint;

        public GameObject ContentObject;
        private RectTransform contentObjectRect;
        public GameObject PreviewPrefab;

        private bool isBigPreviewActive = false;
        public GameObject BigPreview;
        public GameObject BigPreviewPrefab;
        public GameObject BigPreviewContentObject;
        public RectTransform BigPreviewContentObjectRect;
        private const float bigPreviewCardOffsetX = 55F;
        private const float bigPreviewCardOffsetY = 112F;
        private const float bigPreviewCardSizeX = 1220F;
        private const float bigPreviewCardSizeY = 1830F;
        private const float bigPreviewContentOffset = 0;

        private const string cardsFolder = "Cards";
        private const string cardPacksFolder = "CardPacks";
        private const string previewFileName = "Preview";
        private const string swipeTag = "Swipe";

        private const float minSwipeSpeed = 3F;
        private const float baseSwipeSpeed = 0.25F;
        private float swipeSpeed = 3F;
        private float lastX;
        private const float cardPackPreviewOffset = 110F;
        private const float cardPackPreviewSize = 1020F;
        private const float contentOffset = -32F;

        private int currentPack;
        private List<GameObject> previewCards = new List<GameObject>();
        private int currentCard = 0;

        public void Start()
        {
            contentObjectRect = ContentObject.GetComponent<RectTransform>();
            BigPreviewContentObjectRect = BigPreviewContentObject.GetComponent<RectTransform>();
            FillContent();

            SetPack(CardPackages.Packages.Values.ToList().IndexOf(GameSettings.CardPackage), true);
        }

        public void Update()
        {
            if (!Freeze)
                CheckForInput();

            if (!Freeze)
                CheckForInputDebug();

            if (!swipeStartPoint.HasValue)
                MoveToCurrent();

        }

        private void CheckForInput()
        {
            foreach (Touch touch in Input.touches.Where(t => t.phase == TouchPhase.Began))
            {
                touchID = touch.fingerId;

                if (ProcessInputBegin(touch.position))
                    return;
            }
            foreach (Touch touch in Input.touches.Where(t => t.phase == TouchPhase.Ended))
            {
                if (touch.fingerId == touchID)
                {
                    ProcessInputEnd(touch.position);
                }
                touchID = -1;
            }
            foreach (Touch touch in Input.touches.Where(t => t.phase == TouchPhase.Moved))
            {
                if (touch.fingerId == touchID)
                {
                    Drag(touch.position);
                }
            }
        }

        private void CheckForInputDebug()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessInputBegin(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                ProcessInputEnd(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                Drag(Input.mousePosition);
            }
        }

        private void MoveToCurrent()
        {
            if (isBigPreviewActive)
            {
                BigPreviewContentObjectRect.anchoredPosition = Vector3.Lerp(
                    BigPreviewContentObjectRect.anchoredPosition,
                    GetCardTargetPosition(currentCard),
                    Time.deltaTime * swipeSpeed);
            }
            else
            {
                contentObjectRect.anchoredPosition = Vector3.Lerp(
                    contentObjectRect.anchoredPosition,
                    GetPackTargetPosition(currentPack),
                    Time.deltaTime * swipeSpeed);
            }
        }

        private void Drag(Vector2 inputPoint)
        {
            if (!swipeStartPoint.HasValue)
                return;

            float dx = swipeStartPoint.Value.x - inputPoint.x;
            lastX = inputPoint.x;
            if (isBigPreviewActive)
            {
                Vector2 target = GetCardTargetPosition(currentCard);
                BigPreviewContentObjectRect.anchoredPosition = target - new Vector2(dx, 0);
            }
            else
            {
                Vector2 target = GetPackTargetPosition(currentPack);
                contentObjectRect.anchoredPosition = target - new Vector2(dx, 0);
            }
        }

        /// <summary>
        /// Sets focus on preview of pack by its id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instant"></param>
        public void SetPack(int id, bool instant = false)
        {
            if (id < 0 || id >= CardPackages.Packages.Count)
                return;

            currentPack = id;
            GameSettings.CardPackage = CardPackages.Packages.ElementAt(id).Value;

            if (instant)
                contentObjectRect.anchoredPosition = GetPackTargetPosition(currentPack);
        }

        private Vector2 GetPackTargetPosition(int id)
        {
            return new Vector2(contentOffset - id * (cardPackPreviewOffset + cardPackPreviewSize), 0);
        }

        /// <summary>
        /// Sets focus on preview of card by its id
        /// </summary>
        /// <param name="id"></param>
        public void SetCard(int id)
        {
            if (id < 0 || id >= previewCards.Count)
                return;

            currentCard = id;
        }

        private Vector2 GetCardTargetPosition(int id)
        {
            return new Vector2(bigPreviewContentOffset - id * (bigPreviewCardOffsetX + bigPreviewCardSizeX), 0);
        }

        /// <summary>
        /// Begin input processing
        /// </summary>
        /// <param name="inputPoint"></param>
        protected bool ProcessInputBegin(Vector2 inputPoint)
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = inputPoint;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            if (results.All(r => !r.gameObject.CompareTag(swipeTag)))
                return false;

            swipeStartPoint = inputPoint;

            return true;
        }

        /// <summary>
        /// Sends input events to all subscribers
        /// </summary>
        /// <param name="inputPoint"></param>
        /// <returns></returns>
        private bool ProcessInputEnd(Vector2 inputPoint)
        {
            if (!swipeStartPoint.HasValue)
                return false;

            if (Vector2.Distance(inputPoint, swipeStartPoint.Value) > minSwipeDistance)
            {
                Direction direction = GetDirection(swipeStartPoint.Value.x, inputPoint.x);

                if (isBigPreviewActive)
                {
                    SetCard(direction == Direction.Left ? currentCard + 1 : currentCard - 1);
                }
                else
                {
                    SetPack(direction == Direction.Left ? currentPack + 1 : currentPack - 1);
                }

                float dx = Mathf.Abs(inputPoint.x - lastX);
                swipeSpeed = Mathf.Max(dx * baseSwipeSpeed, minSwipeSpeed);
                swipeStartPoint = null;
                return true;
            }

            swipeStartPoint = null;
            return false;
        }

        /// <summary>
        /// Returns direction from old point to new
        /// </summary>
        /// <param name="oldX"></param>
        /// <param name="newX"></param>
        /// <returns></returns>
        private Direction GetDirection(float oldX, float newX)
        {
            float dx = newX - oldX;
            return dx > 0 ? Direction.Right : Direction.Left;
        }

        private void FillContent()
        {
            float contentSize = 0F;

            foreach (KeyValuePair<string, CardPack> pair in CardPackages.Packages)
            {
                Sprite preview = GetPreviewFromCardPackName(pair.Key);
                GameObject go = Instantiate(PreviewPrefab);
                go.transform.SetParent(ContentObject.transform);
                go.GetComponent<Image>().sprite = preview;
                float x = contentSize + cardPackPreviewOffset;
                RectTransform cprt = go.GetComponent<RectTransform>();
                cprt.sizeDelta = new Vector2(cardPackPreviewSize, cardPackPreviewSize);
                cprt.anchoredPosition = new Vector2(x, cardPackPreviewOffset);
                go.transform.localScale = new Vector3(1, 1, 1);

                Button previewButton = go.transform.GetChild(0).GetComponent<Button>();
                previewButton.onClick.AddListener(() => OnPreviewButtonClick(pair.Key));

                contentSize += cardPackPreviewSize + cardPackPreviewOffset;
            }
            contentSize += cardPackPreviewOffset;

            RectTransform rt = ContentObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(contentSize, rt.sizeDelta.y);
        }

        public void BigPreviewBackButtonClick()
        {
            isBigPreviewActive = false;

            foreach (GameObject previewCard in previewCards)
            {
                Destroy(previewCard);
            }

            BigPreviewContentObjectRect.anchoredPosition = new Vector2(bigPreviewContentOffset, 0);
            previewCards.Clear();
            currentCard = 0;

            BigPreview.SetActive(false);
        }

        private void OnPreviewButtonClick(string packName)
        {
            isBigPreviewActive = true;

            float contentSize = 0F;

            foreach (Sprite cardSprite in GetPackSprites(packName))
            {
                GameObject go = Instantiate(BigPreviewPrefab);
                go.transform.SetParent(BigPreviewContentObject.transform);
                go.GetComponent<Image>().sprite = cardSprite;
                float x = contentSize + bigPreviewCardOffsetX;
                RectTransform cprt = go.GetComponent<RectTransform>();
                cprt.sizeDelta = new Vector2(bigPreviewCardSizeX, bigPreviewCardSizeY);
                cprt.anchoredPosition = new Vector2(x, bigPreviewCardOffsetY);
                go.transform.localScale = new Vector3(1, 1, 1);

                previewCards.Add(go);

                contentSize += bigPreviewCardSizeX + bigPreviewCardOffsetX;
            }
            contentSize += bigPreviewCardOffsetX;

            RectTransform rt = BigPreviewContentObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(contentSize, rt.sizeDelta.y);

            BigPreview.SetActive(true);
        }

        private Sprite[] GetPackSprites(string packName)
        {
            return Resources.LoadAll<Sprite>(Path.Combine(cardPacksFolder, packName, cardsFolder));
        }

        private Sprite GetPreviewFromCardPackName(string cardPackName)
        {
            return Resources.Load<Sprite>(
                Path.Combine(cardPacksFolder, cardPackName, previewFileName));
        }

        private enum Direction
        {
            Left,
            Right,
        }
    }
}
