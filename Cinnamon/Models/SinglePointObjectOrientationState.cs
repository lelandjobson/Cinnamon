using Cinnamon.Components.Capture.Managers;
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
            Transform xform = Transform.Translation(vec);
            RhinoAppMappings.ActiveDoc.Objects.Transform(rhObj.Id, xform, true);

            // Check for tethers
            if (TetherManager.Tethers.ContainsKey(Id))
            {
                foreach (var follower in TetherManager.Tethers[Id])
                {
                    try
                    {
                        RhinoAppMappings.ActiveDoc.Objects.Transform(follower, xform, true);
                    }
                    catch { }
                }
            }
        }
    }
}