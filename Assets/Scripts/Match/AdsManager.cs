using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Controls unity ads
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        private static AdsManager instance;

        public static AdsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AdsManager>();
                }
                return instance;
            }
        }

        private const string gameId = "3612140";
        private const string placementId = "AdBanner";
        private const bool testMode = false;

        public void Start()
        {
            Advertisement.Initialize(gameId, testMode);
        }

        public IEnumerator ShowBannerWhenReady()
        {
            while (!Advertisement.IsReady(placementId))
            {
                yield return new WaitForSeconds(0.5f);
            }
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(placementId);
            Debug.Log("Showing ads");
        }

        public void HideBanner()
        {
            Advertisement.Banner.Hide();
        }
    }
}
