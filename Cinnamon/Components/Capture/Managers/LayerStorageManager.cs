using Cinnamon.Models;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Cinnamon.Components.Capture.Managers
{
    /// <summary>
    /// Simple rhinoobject based storage for geometry and data
    /// on layers in a heirarchy
    /// </summary>
    internal static class LayerStorageManager
    {
        private static LayerTable _rhlyrs => Rhino.RhinoDoc.ActiveDoc.Layers;
        private static Layer _cinnamonParentLayer
        {
            get
            {
                var parent = _rhlyrs.FindName("Cinnamon");
                if (parent == null)
                {
                    var p = new Layer()
                    {
                        Name = "Cinnamon",
                        IsVisible = false,
                        //IsLocked = true
                    };
                    // create parent layer
                    _rhlyrs.Add(p);
                    return _rhlyrs.FindName("Cinnamon");
                }
                else
                {
                    return parent;
                }
            }
        }

        public static LayerStoragePayload Load(params string[] path)
        {
            if(path.Length == 0) { throw new Exception("must provide path to load information from layer"); }

            Layer cur = _cinnamonParentLayer;
            for (int i =0; i < path.Length; i++)
            {
                cur = GetCinnamonLayer(path[i], cur, false);
                if(cur == null)
                {

                }
            }
            return ReadPayload(cur);
        }

        public static LayerStoragePayload ReadPayload(Layer layer)
        {
            Dictionary<string, Point3d> payloadData = new Dictionary<string, Point3d>(StringComparer.OrdinalIgnoreCase);
            var objs = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(layer);
            foreach(var obj in objs)
            {
                if (payloadData.ContainsKey(obj.Name)) { continue; }
                payloadData.Add(obj.Name, obj.Geometry.ObjectType == ObjectType.Point ? (obj.Geometry as Point).Location : Point3d.Origin);
            }
            return new LayerStoragePayload(payloadData);
        }

        public static void WritePayload(LayerStoragePayload payload, Layer layer)
        {
            foreach (var point in payload.Points)
            {
                // Create objects
                Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(
                    point.Value,
                    new ObjectAttributes()
                    {
                        LayerIndex = layer.Index,
                        Name = point.Key,
                    });
            }
        }

        public static void Store(LayerStoragePayload payload, bool clearPrevious = true, params string[] path)
        {
            if(path.Length == 0) { throw new Exception("must provide path to store information to layer"); }

            try
            {
                Layer cur = _cinnamonParentLayer;
                for (int i = 0; i < path.Length; i++)
                {
                    cur = GetCinnamonLayer(path[i], cur, true);
                }
                if (clearPrevious)
                {
                    var objectsToDelete = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(cur);
                    foreach (var o in objectsToDelete)
                    {
                        Rhino.RhinoDoc.ActiveDoc.Objects.Delete(o);
                    }
                }
                WritePayload(payload, cur);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Layer GetCinnamonLayer(string name, Layer parent, bool createIfNotFound = false)
        {
            var layer = _rhlyrs.FindName(name);
            if (layer == null)
            {
                goto CreateNew;
            }
            else if(parent != null)
            {
                foreach(var possibleMatch in _rhlyrs.Where(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    if(possibleMatch.ParentLayerId == parent.Id)
                    {
                        return possibleMatch;
                    }
                }
                goto CreateNew;
            }
            else
            {
                return layer;
            }

            CreateNew:

            if (!createIfNotFound) { return null; }

            // create parent layer
            var p = new Layer()
            {
                Name = name,
                IsVisible = false,
                ParentLayerId = parent != null ? parent.Id : _cinnamonParentLayer.Id
            };
            _rhlyrs.Add(p);
            layer = _rhlyrs.FindName(name);
            return layer;
        }
    }
}
