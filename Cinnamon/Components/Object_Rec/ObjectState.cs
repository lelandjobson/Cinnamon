using Rhino.Geometry;
using System;

namespace Cinnamon.Components.Object_Rec
{
    public class ObjectState
    {
        public readonly Guid Id;
        public Point3d PositionState { get; internal set; }

        public ObjectState(Guid id) { Id = id; }
    }
}