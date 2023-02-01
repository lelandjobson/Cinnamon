using Cinnamon.Components.Capture;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

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

        public OrientableMovementCurve Curve { get; private set; }

        public MoveObjectWithCapturesEffect(Guid objectId, OrientableMovementCurve curve)
        {
            if (!curve.IsValid) { throw new Exception("Could not create effect with invalid curve."); }

            ObjectId = objectId;
            Curve = curve;

            if(!DocumentCaptureManagers.TryGetOrCreateObjectCaptureManager(ObjectId, out _))
            {
                throw new Exception("Object does not have any captures loaded in memory.");
            }

            // Check orientation state type
            if(!DocumentBaseState.ActiveBase.TryGetObjectBaseState(ObjectId, out _))
            {
                throw new Exception("Cannot animate object without base state.");
            }

            // set curve startpoint to criitical point location
            var ro = objectId.ToDocumentObject();
            if(ro == null) { throw new Exception("Provided Id is invalid or does not refer to an object in the document"); }
        }

        public IEffect Copy()
        {
            return new MoveObjectWithCapturesEffect(this.ObjectId, this.Curve);
        }

        IEnumerable<Point3d> GetValue(double percentage)
        {      
            foreach(var curve in Curve.Iterate())
            {
                Point3d result = default(Point3d);
                if (curve is PolylineCurve pc)
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
                    result = curve.PointAtNormalizedLength(percentage);
                }
                yield return result;
            }
            yield break;
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            if (state.ObjectPositionStates.ContainsKey(ObjectId))
            {
                Logger.Log(
                     $"Object location being overrided by effect \"{Name}\"");
            }
            else
            {
                state.ObjectPositionStates.Add(ObjectId, null);
            }

            if (!ObjectOrientationState.TryCreate(ObjectId, GetValue(percentage), out var orientation))
            {
                throw new Exception("Could not orient object for some reason.");
            }

            state.ObjectPositionStates[ObjectId] = orientation;

            //// Discern object orientation type
            //switch (_baseState.Kind)
            //{
            //    case OrientationStateKind.ThreePoint:
            //        SetThreePoint();
            //        break;
            //    default:
            //        SetSinglePoint();
            //        break;
            //}

            //void SetSinglePoint()
            //{

            //}

            //void SetThreePoint()
            //{
            //    throw new NotImplementedException();
            //}
        }
    }
}