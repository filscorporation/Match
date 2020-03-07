using System.Collections.Generic;

namespace Assets.Scripts.Match.UI
{
    /// <summary>
    /// Controlls enabling and disabling of multiple buttons where only on can be selected
    /// </summary>
    public class OptionsButtonsManager<T>
    {
        private List<ButtonWrapper<T>> _buttons;

        public OptionsButtonsManager(List<ButtonWrapper<T>> buttons, T initialState)
        {
            _buttons = buttons;

            SelectOption(initialState);
        }

        /// <summary>
        /// Activates button by option
        /// </summary>
        /// <param name="option"></param>
        public void SelectOption(T option)
        {
            foreach (ButtonWrapper<T> button in _buttons)
            {
                if (button.Parameter.Equals(option))
                {
                    button.Button.interactable = false;
                }
                else
                {
                    button.Button.interactable = true;
                }
            }
        }
    }
}
