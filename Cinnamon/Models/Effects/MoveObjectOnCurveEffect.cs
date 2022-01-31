using Rhino.Geometry;
using System;

namespace Cinnamon.Models.Effects
{

    /// <summary>
    /// Moves an object.
    /// </summary>
    [Serializable]
    public class MoveObjectOnCurveEffect : IEffect
    {
        public string Name { get; set; }

        public Guid Id => _id;
        private Guid _id = Guid.NewGuid();

        public bool Local { get => _local; set => _local = value; }
        private bool _local;

        public Guid ObjectId { get; private set; }

        public Curve Curve { get; private set; }

        public MoveObjectOnCurveEffect(Guid objectId, Curve curve)
        {
            ObjectId = objectId;
            Curve = curve;
        }

        public IEffect Copy()
        {
            return new MoveObjectOnCurveEffect(this.ObjectId, this.Curve);
        }

        Point3d GetValue(double percentage)
        {
            return Curve.PointAt(percentage);
        }

        public void SetFrameStateValue(double percentage, FrameState state)
        {
            var loc = GetValue(percentage);
            if (!state.ObjectPositionStates.ContainsKey(ObjectId))
            {
                state.ObjectPositionStates.Add(ObjectId, loc);
            }
            else
            {
                Logger.Log(
                    $"Object location being overrided by effect \"{Name}\"");

                state.ObjectPositionStates[ObjectId] = loc;
            }
        }
    }
}