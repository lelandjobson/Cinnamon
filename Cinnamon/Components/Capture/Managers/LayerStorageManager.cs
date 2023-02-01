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
        private const string CinnamonLayerRootName = "Cinnamon_Gh";

        private static LayerTable _rhlyrs => RhinoAppMappings.ActiveDoc.Layers;
        private static Layer _cinnamonParentLayer
        {
            get
            {
                var parent = _rhlyrs.FindName(CinnamonLayerRootName);
                if (parent == null)
                {
                    var p = new Layer()
                    {
                        Name = CinnamonLayerRootName,
                        IsVisible = false,
                    };
                    _rhlyrs.Add(p);
                    return _rhlyrs.FindName(CinnamonLayerRootName);
                }
                else
                {
                    return parent;
                }
            }
        }

        public static void DeleteLayer(Layer layer) => RhinoAppMappings.ActiveDoc.Layers.Delete(layer);


        public static LayerStoragePayload ReadPayload(string path)
        {
            if(path.Length == 0) { throw new Exception("must provide path to load information from layer"); }

            //Layer cur = _cinnamonParentLayer;
            //for (int i =0; i < path.Length; i++)
            //{
            //    cur = GetCinnamonLayer(path[i], cur, false);
            //    if(cur == null)
            //    {
            //    }
            //}
            return Load(GetOrCreateLayerAtPath(path));
        }

        public static LayerStoragePayload Load(Layer layer)
        {
            Dictionary<string, Point3d> payloadData = new Dictionary<string, Point3d>(StringComparer.OrdinalIgnoreCase);
            var objs = RhinoAppMappings.ActiveDoc.Objects.FindByLayer(layer);
            foreach(var obj in objs)
            {
                if (payloadData.ContainsKey(obj.Name)) { continue; }
                payloadData.Add(obj.Name, obj.Geometry.ObjectType == ObjectType.Point ? (obj.Geometry as Point).Location : Point3d.Origin);
            }
            return new LayerStoragePayload(layer,payloadData);
        }

        public static void WritePayload(LayerStoragePayload payload)
        {
            int layerIndex = RhinoAppMappings.DocumentLayers.FindByFullPath(payload.Layer.FullPath,-99);
            if(layerIndex == -99) { 
                var layerFixed = GetOrCreateLayerAtPath(payload.Layer.FullPath); 
                payload.SetLayer(layerFixed); 
                WritePayload(payload); 
            }
            foreach (var point in payload.Points)
            {
                // Create objects
                RhinoAppMappings.ActiveDoc.Objects.AddPoint(
                    point.Value,
                    new ObjectAttributes()
                    {
                        LayerIndex = layerIndex,
                        Name = point.Key,
                    });
            }
        }

        public static Layer GetOrCreateLayerAtPath(string path)
        {
            int layerIndex = _rhlyrs.FindByFullPath(CreatePath(path), -99);
            if(layerIndex == -99)
            {
                // Create new layer
                string[] pathPieces = path.Split('/');
                Layer parent = _cinnamonParentLayer;
                for(int i = 0; i < pathPieces.Length; i++)
                {
                    string curLayer = pathPieces[i];
                    Layer existingLayer = parent.GetChildrenSafe().FirstOrDefault(l => l.Name.Equals(curLayer, StringComparison.OrdinalIgnoreCase));
                    if(existingLayer != null)
                    {
                        parent = existingLayer;
                        continue;
                    }
                    else
                    {
                        var l = new Layer()
                        {
                            Name = curLayer,
                            IsVisible = false,
                            //IsLocked = true,
                            ParentLayerId = parent.Id
                        };
                        int lyrIndex = RhinoAppMappings.ActiveDoc.Layers.Add(l);
                        l.ParentLayerId = parent.Id;
                        parent = RhinoAppMappings.ActiveDoc.Layers[lyrIndex];
                    }
                }
                return parent;
                // May need to call regen
            }
            return _rhlyrs[layerIndex];
        }

        static string CreatePath(string path) => $"{CinnamonLayerRootName}/{path}".Replace("//","/");

        public static void Store(LayerStoragePayload payload, bool clearPrevious = true)
        {
            try
            {
                Layer layer = payload.Layer;
                if (clearPrevious)
                {
                    var objectsToDelete = RhinoAppMappings.ActiveDoc.Objects.FindByLayer(layer);
                    foreach (var o in objectsToDelete)
                    {
                        RhinoAppMappings.ActiveDoc.Objects.Delete(o);
                    }
                }
                WritePayload(payload);
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
