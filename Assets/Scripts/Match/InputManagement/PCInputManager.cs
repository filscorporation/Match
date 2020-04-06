using UnityEngine;

namespace Assets.Scripts.Match.InputManagement
{
    public class PCInputManager : InputManagerBase
    {
        public override void CheckForInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (ProcessInput(Input.mousePosition))
                    return;
            }
        }
    }
}
