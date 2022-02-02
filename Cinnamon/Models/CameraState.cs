using Rhino.Geometry;
using System;

namespace Cinnamon.Models
{
    /// <summary>
    /// Contains the state information of the active
    /// view camera necessary for the app
    /// </summary>
    [Serializable]
    public class CameraState
    {

        public Point3d PositionState = Point3d.Unset;

        public Point3d TargetPositionState = Point3d.Unset;

        public double FocalLengthState = -1;

    }
}
