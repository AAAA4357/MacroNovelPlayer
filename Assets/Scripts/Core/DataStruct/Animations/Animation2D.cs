namespace MNP.Core.DataStruct.Animations
{
    public struct Animation2D
    {
        public Animation2DFrame StartFrame;

        public Animation2DFrame EndFrame;

        public float Duration;

        public AnimationProperty PositionProperty;

        public AnimationProperty RotationProperty;

        public AnimationProperty ScaleProperty;
    }
}
