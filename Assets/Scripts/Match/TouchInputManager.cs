using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Match
{
    public class TouchInputManager : InputManagerBase
    {
        public override void CheckForInput()
        {
            foreach (Touch touch in Input.touches.Where(t => t.phase == TouchPhase.Began))
            {
                if (ProcessInput(touch.position))
                    return;
            }
        }
    }
}
