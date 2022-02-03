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

        Point3d GetValue(Curve curve, double percentage)
        {
            if (curve is PolylineCurve pc)
            {
                var plc = pc.ToPolyline();
                var mapped = percentage / (1.0 / (plc.Count - 1));
                int lower = Math.Floor(mapped).ToInt32();
                int upper = lower + 1;
                mapped -= lower;
                return mapped.PercToValue(plc[lower], plc[upper]);
            }
            return curve.PointAtNormalizedLength(percentage);
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            if(CameraCurve != null)
            {
                state.CameraState.PositionState = GetValue(CameraCurve,percentage);
            }
            if(TargetCurve != null)
            {
                state.CameraState.TargetPositionState = GetValue(TargetCurve,percentage);
            }
        }
    }
}