﻿using Cinnamon.Components.Capture.Managers;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace Cinnamon.Models
{
    public class ThreePointObjectOrientationState : ObjectOrientationState
    {
        public ThreePointObjectOrientationState(Guid id, Point3d a, Point3d b, Point3d c) : base(id, new[] {a,b,c})
        {
        }

        public ThreePointObjectOrientationState(Guid id, Point3d[] pts) : base(id, pts)
        {
        }

        public override void Apply()
        {
            var ro = Id.ToDocumentObject();
            if (!ro.TryGetOrientationState(out var oldState))
            {
                throw new Exception($"Unable to set the orientation state of object \"{ro.Name}\" with Id {ro.Id}");
            }
            var xform = Transform.PlaneToPlane(oldState.Plane, this.Plane);


            RhinoAppMappings.ActiveDoc.Objects.Transform(Id, xform, true);

            // Check for tethers
            if (TetherManager.Tethers.ContainsKey(Id))
            {
                foreach(var follower in TetherManager.Tethers[Id])
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