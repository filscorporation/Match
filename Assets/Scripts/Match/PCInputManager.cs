using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Match
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
