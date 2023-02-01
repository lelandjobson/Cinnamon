using Cinnamon.Components.Capture.Managers;
using Cinnamon.Models;
using Rhino.DocObjects;
using System;
using System.Linq;

namespace Cinnamon.Components.Capture
{
    public class ObjectCaptureManager : CaptureManager<ObjectOrientationState>
    {
        public readonly Guid Id;

        protected override string BasePath => _basePath;

        public ObjectOrientationState FirstState => _captures.Count > 0 ? ExtractOrientation(_captures[CaptureKeys[0]]) : null;

        private readonly string _basePath;

        public ObjectCaptureManager(Guid id) 
        {
            Id = id;

            _basePath = $"ObjectCaptures/{id}";

            Regen();
        }

        protected override ObjectOrientationState ExtractOrientation(LayerStoragePayload payload)
        {
            var layerPoints = payload.Points;
            if (layerPoints.Count <= 2)
            {
                return new SinglePointObjectOrientationState(Id, layerPoints["A"]);
            }
            else
            {
                return new ThreePointObjectOrientationState(Id,
                    layerPoints["A"],
                    layerPoints["B"],
                    layerPoints["C"]);
            }
        }

        protected override void CreateCapture(int order, ObjectOrientationState data = null)
        {
            if (_captures.ContainsKey(order))
            {
                ClearCaptureData(order, out _);
            }

            Layer layer = LayerStorageManager.GetOrCreateLayerAtPath($"{BasePath}/{order}");

            var pointDict = new System.Collections.Generic.Dictionary<string, Rhino.Geometry.Point3d>();
            if(data != null)
            {
                if(data.A != null)
                {
                    pointDict.Add("A", data.A);
                }
                if(data.B != null)
                {
                    pointDict.Add("B", data.B);
                }
                if(data.C != null)
                {
                    pointDict.Add("C", data.C);
                }
            }

            LayerStorageManager.Store(new LayerStoragePayload(layer, pointDict));
        }
    }
}