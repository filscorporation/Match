using System;

namespace Assets.Scripts.Match.Exceptions
{
    public class ResourceLoadException : Exception
    {
        public ResourceLoadException(string message) : base(message)
        {

        }
    }
}
