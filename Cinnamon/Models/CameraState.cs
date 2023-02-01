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
        public Point3d Position
        {
            get => A;
            set => A = value;
        }

        public Point3d Target
        {
            get => B;
            set => B = value;   
        }

        public double FocalLength = 35;

        public CameraState(Point3d cameraLocation, Point3d cameraTarget, double focalLength = -1) : base(Guid.Empty, new[] {cameraLocation,cameraTarget})
        {
            FocalLength = focalLength;
        }

        public override void Apply()
        {
            if (!double.IsNaN(FocalLength))
            {
                DocumentBaseState.ActiveBase.Viewport.Camera35mmLensLength = FocalLength;
            }
            if (Position != Point3d.Unset && Target != Point3d.Unset)
            {
                DocumentBaseState.ActiveBase.Viewport.SetCameraLocations(Target, Position);
            }
            else if (Position == Point3d.Unset && Target != Point3d.Unset)
            {
                DocumentBaseState.ActiveBase.Viewport.SetCameraTarget(Target, false);
            }
            else if (Position != Point3d.Unset && Target == Point3d.Unset)
            {
                DocumentBaseState.ActiveBase.Viewport.SetCameraLocation(Position, false);
            }
        }
    }
}
