using System;

namespace Cinnamon.Models.Effects
{
    [Serializable]
    public class FocalLengthEffect : IEffect
    {
        public string Name { get; set; }

        public bool Local { get => _local; set => _local = value; }
        private bool _local;

        public Guid Id => _id;
        private Guid _id = Guid.NewGuid();


        public double StartingFocalLength { get; private set; }
        public double EndingFocalLength { get; private set; }

        public FocalLengthEffect(double startingFocalLength, double endingFocalLegnth)
        {
            StartingFocalLength = startingFocalLength; 
            EndingFocalLength = endingFocalLegnth; 
        }

        public IEffect Copy()
        {
            return new FocalLengthEffect(StartingFocalLength, EndingFocalLength);
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            state.CameraState.FocalLengthState = percentage.PercToValue(StartingFocalLength, EndingFocalLength);
        }
    }
}