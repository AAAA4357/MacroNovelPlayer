namespace MNP.Core.Exceptions
{
    [System.Serializable]
    public class AnimationLerpException : System.Exception
    {
        public AnimationLerpException() { }
        public AnimationLerpException(string message) : base(message) { }
        public AnimationLerpException(string message, System.Exception inner) : base(message, inner) { }
        protected AnimationLerpException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}