using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Match.Utility
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Start()
        {
            if (FindObjectsOfType<DontDestroyOnLoad>().Any(o => o != this))
            {
                Destroy(gameObject);
            }
        }
    }
}
