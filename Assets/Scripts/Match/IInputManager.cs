using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Match
{
    public interface IInputManager
    {
        void AddSubscriber(IUISubscriber subscriber);

        void CheckForInput();
    }
}
