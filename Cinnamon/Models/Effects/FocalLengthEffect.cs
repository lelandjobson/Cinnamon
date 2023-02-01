using System;
using System.Collections.Generic;
using System.Linq;

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


        public List<double> FocalLengthStates { get; private set; } = new List<double>();

        public FocalLengthEffect(double startingFocalLength, double endingFocalLegnth)
        {
            FocalLengthStates.Add(startingFocalLength);
            FocalLengthStates.Add(endingFocalLegnth);
            // Create range
        }

        public FocalLengthEffect(IEnumerable<double> states)
        {
            FocalLengthStates = states.Select(s => s).ToList();
        }

        public IEffect Copy()
        {
            return new FocalLengthEffect(FocalLengthStates);
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            if(state.CameraState == null) { state.CameraState = DocumentBaseState.GetActiveCameraState(); }
            if(FocalLengthStates.Count == 2)
            {
                state.CameraState.FocalLength = percentage.PercToValue(FocalLengthStates[0], FocalLengthStates[1]);
            }
            else
            {
                var mapped = percentage / (1.0 / (FocalLengthStates.Count - 1));
                int lower = Math.Floor(mapped).ToInt32();
                if(lower == FocalLengthStates.Count - 1) { lower--; }
                int upper = lower + 1;
                mapped -= lower;
                state.CameraState.FocalLength = mapped.PercToValue(FocalLengthStates[lower], FocalLengthStates[upper]);
            }
        }
    }
}