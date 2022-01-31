using Rhino.Geometry;
using System;

namespace Cinnamon.Models.Effects
{

    /// <summary>
    /// Moves an object.
    /// </summary>
    [Serializable]
    public class MoveObjectEffect : IEffect
    {
        public string Name { get; set; }

        public Guid Id => _id;
        private Guid _id = Guid.NewGuid();

        public bool Local { get => _local; set => _local = value; }
        private bool _local;

        public Guid ObjectId { get; private set; }

        public Point3d StartLocation { get; private set; }

        public Point3d EndLocation { get; private set; }

        readonly Vector3d TotalVector;

        public MoveObjectEffect(Guid objectId, Point3d start, Point3d end)
        {
            ObjectId = objectId;
            StartLocation = start;
            EndLocation = end;
            TotalVector = end - start;
        }

        public IEffect Copy()
        {
            return new MoveObjectEffect(this.ObjectId, this.StartLocation, this.EndLocation);
        }

        Point3d GetValue(double percentage)
        {
            var vec = (TotalVector * percentage);
            return new Point3d(StartLocation.X + vec.X, 
                               StartLocation.Y + vec.Y, 
                               StartLocation.Z + vec.Z);
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