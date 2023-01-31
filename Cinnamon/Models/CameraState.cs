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

        public CameraState(params Point3d[] points) : base(Guid.Empty, points)
        {
        }

        public override void Apply()
        {
            throw new NotImplementedException();
        }
    }
}
