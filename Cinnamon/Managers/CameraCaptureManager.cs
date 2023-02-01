using Cinnamon.Components.Capture.Managers;
using Cinnamon.Models;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Linq;

namespace Cinnamon.Components.Capture
{
    public class CameraCaptureManager : CaptureManager<CameraState>
    {
        public static CameraCaptureManager Default => _default ?? (_default = new CameraCaptureManager());
        private static CameraCaptureManager _default;
        protected override string BasePath => "CameraState";

        public CameraCaptureManager()
        {
        }

        protected override CameraState ExtractOrientation(LayerStoragePayload payload)
        {
            Point3d camOrigin = Point3d.Origin;
            Point3d camTarget = Point3d.Origin;
            double camFocalLength = 30;

            // Grab cam location
            var cameraLocKey = payload.Points.Keys.First(k => k.Contains(POINTNAME_CAMERALOCATION));
            if(cameraLocKey != null)
            {
                camOrigin = payload.Points[cameraLocKey];
                double.TryParse(cameraLocKey.Split('_').Last(), out camFocalLength);
            }

            // Grab cam target
            var cameraTarKey = payload.Points.Keys.First(k => k.Contains(POINTNAME_CAMERATARGET));
            if (cameraTarKey != null)
            {
                camTarget = payload.Points[cameraTarKey];
            }

            return new CameraState(camOrigin, camTarget, camFocalLength);   
        }

        private const string POINTNAME_CAMERALOCATION = "camLoc";
        private const string POINTNAME_CAMERATARGET = "camTar";

        protected override void CreateCapture(int order, CameraState data)
        {
            if (_captures.ContainsKey(order))
            {
                ClearCaptureData(order , out _);
            }

            Layer layer = LayerStorageManager.GetOrCreateLayerAtPath($"{BasePath}/{order}");

            LayerStorageManager.Store(new LayerStoragePayload(layer, new System.Collections.Generic.Dictionary<string, Rhino.Geometry.Point3d>()
            {
                { $"{POINTNAME_CAMERALOCATION}_{data.FocalLength}", data.Position},
                { $"{POINTNAME_CAMERATARGET}", data.Target},
            }));

        }
    }
}