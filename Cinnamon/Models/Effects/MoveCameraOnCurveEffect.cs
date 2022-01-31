using Rhino.Geometry;
using System;

namespace Cinnamon.Models.Effects
{

    /// <summary>
    /// Moves an object.
    /// </summary>
    [Serializable]
    public class MoveCameraOnCurveEffect : IEffect
    {
        public string Name { get; set; }

        public Guid Id => _id;
        private Guid _id = Guid.NewGuid();

        public bool Local { get => _local; set => _local = value; }
        private bool _local;

        public Curve CameraCurve { get; private set; }
        public Curve TargetCurve { get; private set; }

        public MoveCameraOnCurveEffect(Curve cameraCurve = null, Curve targetCurve = null)
        {
            CameraCurve = cameraCurve;
            TargetCurve = targetCurve;
        }

        public IEffect Copy()
        {
            return new MoveCameraOnCurveEffect(this.CameraCurve, this.TargetCurve);
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            if(CameraCurve != null)
            {
                state.CameraState.PositionState = CameraCurve.PointAt(percentage);
            }
            if(TargetCurve != null)
            {
                state.CameraState.TargetPositionState = TargetCurve.PointAt(percentage);
            }
        }
    }
}