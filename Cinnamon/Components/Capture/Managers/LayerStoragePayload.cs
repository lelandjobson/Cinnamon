using Cinnamon.Models;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;


namespace Cinnamon.Components.Capture.Managers
{
    public class LayerStoragePayload
    {
        public bool IsValidCapture => CaptureNum != -1;
        public int CaptureNum = -1;
        public readonly string LayerName;
        public Layer Layer { get; private set; }
        public readonly Dictionary<string, Point3d> Points;
        public string LayerParentName;

        public LayerStoragePayload(Layer layer, Dictionary<string, Point3d> points)
        {
            this.Layer = layer;
            LayerName = layer.Name;
            Points = points;
            int.TryParse(LayerName, out CaptureNum);
        }

        /// <summary>
        /// Used only when the layer attached to this payload is invalid
        /// i.e. deleted and recreated.
        /// </summary>
        /// <param name="layerFixed"></param>
        internal void SetLayer(Layer layerFixed)
        {
            this.Layer = layerFixed;
        }
    }
}
