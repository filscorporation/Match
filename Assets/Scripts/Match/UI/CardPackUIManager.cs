using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Match.CardManagement;
using UnityEngine;
using UnityEngine.EventSystems;
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

        public void Start()
        {
            contentObjectRect = ContentObject.GetComponent<RectTransform>();
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
            contentObjectRect.anchoredPosition = Vector3.Lerp(
                contentObjectRect.anchoredPosition,
                GetPackTargetPosition(currentPack),
                Time.deltaTime * swipeSpeed);
        }

        private void Drag(Vector2 inputPoint)
        {
            if (!swipeStartPoint.HasValue)
                return;

            float dx = swipeStartPoint.Value.x - inputPoint.x;
            lastX = inputPoint.x;
            Vector2 target = GetPackTargetPosition(currentPack);
            contentObjectRect.anchoredPosition = target - new Vector2(dx, 0);
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
                SetPack(direction == Direction.Left ? currentPack + 1 : currentPack - 1);
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

                contentSize += cardPackPreviewSize + cardPackPreviewOffset;
            }
            contentSize += cardPackPreviewOffset;

            RectTransform rt = ContentObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(contentSize, rt.sizeDelta.y);
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
