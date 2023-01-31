using Rhino.Geometry;
using System;

namespace Cinnamon.Models
{
    /// <summary>
    /// Contains the state information of the active
    /// view camera necessary for the app
    /// </summary>
    [Serializable]
    public class CameraState : ObjectOrientationState
    {

        public Point3d PositionState = Point3d.Unset;

        public Point3d TargetPositionState = Point3d.Unset;

        public double FocalLengthState = -1;

        public CameraState(Point3d cameraLocation, Point3d cameraTarget, double focalLengthState = -1) : base(Guid.Empty, new[] {cameraLocation,cameraTarget})
        {
            if (cameraLocation == default(Point3d))
            {
                PositionState = RhinoAppMappings.ActiveViewport.CameraLocation;
            }
            if (cameraTarget == default(Point3d))
            {
                TargetPositionState = RhinoAppMappings.ActiveViewport.CameraTarget;
            }
            if(focalLengthState == -1)
            {
                FocalLengthState = RhinoAppMappings.ActiveViewport.Camera35mmLensLength;
            }
        }

        public CameraState(params Point3d[] points) : base(Guid.Empty, points)
        {
        }

        public override void Apply()
        {
            throw new NotImplementedException();
        }
    }
}
