using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Match.UI
{
    /// <summary>
    /// Script for button to play sound on click
    /// </summary>
    public class ButtonPlaySoundOnClick : MonoBehaviour
    {
        public AudioClip AudioClip;

        private AudioSource source;

        public void Start()
        {
            source = Resources.FindObjectsOfTypeAll<AudioSource>().FirstOrDefault();
            if (source == null)
            {
                Debug.LogWarning("No audiosource");
                return;
            }

            Button button = this.GetComponent<Button>();
            button.onClick.AddListener(ButtonOnClick);
        }

        private void ButtonOnClick()
        {
            source.PlayOneShot(AudioClip);
        }
    }
}
