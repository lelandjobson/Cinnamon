using Rhino.Geometry;
using System;

namespace Cinnamon.Models.Effects
{

    /// <summary>
    /// Moves an object.
    /// </summary>
    [Serializable]
    public class MoveObjectWithCapturesEffect : IEffect
    {
        public string Name { get; set; }

        public Guid Id => _id;
        private Guid _id = Guid.NewGuid();

        public bool Local { get => _local; set => _local = value; }
        private bool _local;

        public Guid ObjectId { get; private set; }

        public Curve Curve { get; private set; }

        public MoveObjectWithCapturesEffect(Guid objectId, Curve curve)
        {
            if (!curve.IsValid) { throw new Exception("Could not create effect with invalid curve."); }

            ObjectId = objectId;
            Curve = curve;

            // set curve startpoint to criitical point location
            var ro = objectId.ToDocumentObject();
            if(ro == null) { throw new Exception("Provided Id is invalid or does not refer to an object in the document"); }
        }

        public IEffect Copy()
        {
            return new MoveObjectOnCurveEffect(this.ObjectId, this.Curve);
        }

        Point3d GetValue(double percentage)
        {
            Point3d result = default(Point3d);
            if(Curve is PolylineCurve pc)
            {
                var plc = pc.ToPolyline();
                var mapped = percentage / (1.0 / (plc.Count - 1));
                int lower = Math.Floor(mapped).ToInt32();
                int upper = lower + 1;
                // safeguard
                if (upper >= plc.Count)
                {
                    result = plc.Last;
                }
                else
                {
                    mapped -= lower;
                    result = mapped.PercToValue(plc[lower], plc[upper]);
                }
            }
            else
            {
                result = Curve.PointAtNormalizedLength(percentage);
            }
            return result;
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            var loc = GetValue(percentage);
            if (!state.ObjectPositionStates.ContainsKey(ObjectId))
            {
                state.ObjectPositionStates.Add(ObjectId, new SinglePointObjectOrientationState(ObjectId, loc));
            }
            else
            {
                Logger.Log(
                    $"Object location being overrided by effect \"{Name}\"");

                state.ObjectPositionStates[ObjectId] = new SinglePointObjectOrientationState(ObjectId, loc);
            }
        }
    }
}