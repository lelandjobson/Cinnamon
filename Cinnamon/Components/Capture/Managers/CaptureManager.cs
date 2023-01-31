using Cinnamon.Components.Capture.Managers;
using Rhino.DocObjects;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Capture
{
    public abstract class CaptureManager<T>
    {
        protected abstract string BasePath { get; }

        protected abstract T ExtractOrientation(LayerStoragePayload payload);

        public T GetCaptureData(int cap) => ExtractOrientation(_captures[cap]);

        protected Dictionary<int, LayerStoragePayload> _captures = new Dictionary<int, LayerStoragePayload>();

        public CaptureManager()
        {

        }

        protected void Regen()
        {
            Layer parent = LayerStorageManager.GetOrCreateLayerAtPath(BasePath);

            if (parent != null)
            {
                var output = parent.GetChildren().Select(l => (int.TryParse(l.Name, out var i) ? i : -1, LayerStorageManager.Load(l))).ToList();
                _captures = output
                    .Where(c => c.Item1 != -1)
                    .ToDictionary(c => c.Item1, c => c.Item2);
            }
            else
            {
                _captures = new Dictionary<int, LayerStoragePayload>();
            }
        }

        public void CreateNewCapture(int order, T data)
        {
            CreateCapture(order, data);
            Regen();
        }
        protected abstract void CreateCapture(int order, T data);

        protected void ClearAllCaptureData()
        {
            var layersToClear = _captures.Select(c => c.Value.Layer).ToHashSet();

            foreach(var l in layersToClear)
            {
                var objectsToDelete = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(l);
                foreach (var o in objectsToDelete)
                {
                    Rhino.RhinoDoc.ActiveDoc.Objects.Delete(o);
                }
            }
            _captures.Clear();
            Regen();
        }

        protected bool ClearCaptureData(int order, out Layer layer)
        {
            if (!_captures.ContainsKey(order)) { layer = null; return false; }
            Layer l = _captures[order].Layer;

            var objectsToDelete = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(l);
            foreach (var o in objectsToDelete)
            {
                Rhino.RhinoDoc.ActiveDoc.Objects.Delete(o);
            }
            layer = l;
            _captures.Remove(order);
            return true;
        }

    }
}