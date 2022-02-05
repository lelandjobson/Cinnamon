using Rhino.Geometry;
using System;

namespace Cinnamon.Models
{
    public class ObjectState
    {
        public readonly Guid Id;
        public Point3d PositionState { get; internal set; }

        public ObjectState(Guid id) { Id = id; }
    }
}