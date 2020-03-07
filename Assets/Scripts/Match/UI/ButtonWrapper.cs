using UnityEngine.UI;

namespace Assets.Scripts.Match.UI
{
    /// <summary>
    /// Button and option value that it represents
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ButtonWrapper<T>
    {
        public Button Button { get; set; }

        public T Parameter { get; set; }

        public ButtonWrapper(Button button, T parameter)
        {
            Button = button;
            Parameter = parameter;
        }
    }
}
