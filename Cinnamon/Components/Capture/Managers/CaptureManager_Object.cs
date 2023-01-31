using Cinnamon.Components.Capture.Managers;
using Cinnamon.Models;
using Rhino.DocObjects;
using System;
using System.Linq;

namespace Cinnamon.Components.Capture
{
    public class CaptureManager_Object : CaptureManager<ObjectOrientationState>
    {
        public readonly Guid Id;

        protected override string BasePath => _basePath;
        private readonly string _basePath;

        public int Next
        {
            get
            {
                Regen();
                if(_captures.Count == 0) { return 0; }
                return _captures.Keys.OrderByDescending(a => a).First() + 1;
            }
        }

        public CaptureManager_Object(Guid id)
        {
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
            throw new NotImplementedException();
        }
    }
}