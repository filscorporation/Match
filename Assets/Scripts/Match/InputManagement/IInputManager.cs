namespace Assets.Scripts.Match.InputManagement
{
    public interface IInputManager
    {
        void AddSubscriber(IInputSubscriber subscriber);

        void CheckForInput();
    }
}
