using Rhino;
using Rhino.Geometry;
using System;

namespace Cinnamon.Models
{
    public class SinglePointObjectOrientationState : ObjectOrientationState
    {

        public Point3d PositionState { get; internal set; } 

        public SinglePointObjectOrientationState(Guid id, Point3d loc) : base(id, loc) { PositionState = loc; }

        public override void Apply()
        {
            // Single point orientation
            var rhObj = Id.ToDocumentObject();
            var vec = PositionState - rhObj.ToBBPoint();
            RhinoAppMappings.ActiveDoc.Objects.Transform(rhObj.Id, Transform.Translation(vec), true);
        }
    }
}