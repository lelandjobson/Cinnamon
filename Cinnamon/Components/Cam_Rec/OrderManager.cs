using Cinnamon.Models;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.CameraTools
{
    public static class OrderManager
    {
        public static event EventHandler OrderChanged;

        public const string CAMERA_ORDERSPARENTNAME = "cinnamon_cameras";

        public const string POINTNAME_CAMERALOCATION = "loc";
        public const string POINTNAME_CAMERATARGET   = "tar";

        public static List<int> Orders
        {
            get
            {
                RegenOrderData();
                return _orders?.ToList() ?? new List<int>();
            }
        }
        private static List<int> _orders;
        private static List<Layer> _orderLayers;

        private static LayerTable _rhlyrs => Rhino.RhinoDoc.ActiveDoc.Layers;

        private static Layer _orderLayersParent
        {
            get
            {
                var parent = _rhlyrs.FindName(CAMERA_ORDERSPARENTNAME);
                if (parent == null)
                {
                    // create parent layer
                    var p = new Layer()
                    {
                        Name = CAMERA_ORDERSPARENTNAME,
                        IsVisible = false,
                        IsLocked = true
                    };
                    _rhlyrs.Add(p);
                    return _rhlyrs.FindName(CAMERA_ORDERSPARENTNAME);
                }
                else
                {
                    return parent;
                }
            }
        }

        public static int Next
        {
            get
            {
                RegenOrderData();
                if(Orders.Count == 0) { return 0; }
                return Orders.Last() + 1;
            }
        }

        static void RegenOrderData()
        {
            // gather doc layers
            _orders = null;
            _orders = new List<int>();
            _orderLayers = new List<Layer>();

            foreach (var l in _rhlyrs.Where(l => l.ParentLayerId == _orderLayersParent.Id))
            {
                int order = -1;
                if(!Int32.TryParse(l.Name, out order)) { continue; }
                _orders.Add(order);
                _orderLayers.Add(l);
            }
            if(_orders.Count == 0) { _orders = null; return; }
            _orders.Sort();
            _orderLayers.Sort((a, b) => Int32.Parse(a.Name).CompareTo(Int32.Parse(b.Name)));
            //OrderChanged?.Invoke(null, null);
        }

        internal static CameraState GetOrderData(int order)
        {
            if (!Orders.Contains(order)) { return null; }
            int idx = Orders.IndexOf(order);
            var objs = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(_orderLayers[idx]);
            CameraState output = new CameraState();
            foreach(var o in objs)
            {
                if (o.Name.Contains(POINTNAME_CAMERALOCATION))
                {
                    double focalLength = RhinoAppMappings.ActiveViewport.Camera35mmLensLength;
                    double.TryParse(o.Name.Split('_').Last(), out focalLength);
                    output.FocalLengthState = focalLength;
                    output.PositionState = (o.Geometry as Point).Location;
                }
                else if (o.Name.Contains(POINTNAME_CAMERATARGET))
                {
                    output.TargetPositionState = (o.Geometry as Point).Location;
                }
            }
            return output;
        }

        public static void CreateNewOrder(int order, Point3d cameraLocation = default(Point3d), Point3d cameraTarget = default(Point3d))
        {
            Layer l = null;
            int lyrIndex = -1;
            if (Orders.Contains(order))
            {
                // Clear geometry from order
                ClearOrderData(order, out l);
                lyrIndex = l.Index;
            }
            else
            {
                // Create order layers
                l = new Layer() {
                    Name = order.ToString(),
                    IsVisible = false,
                    IsLocked = true,
                    ParentLayerId = _orderLayersParent.Id
                };
                lyrIndex = Rhino.RhinoDoc.ActiveDoc.Layers.Add(l);
                l.ParentLayerId = _orderLayersParent.Id;
                RegenOrderData();
            }
            if(cameraLocation == default(Point3d))
            {
                cameraLocation = RhinoAppMappings.ActiveViewport.CameraLocation;
            }
            if(cameraTarget == default(Point3d))
            {
                cameraTarget = RhinoAppMappings.ActiveViewport.CameraTarget;
            }

            // get layer index

            // Create objects
            Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(
                cameraLocation, 
                new ObjectAttributes() { 
                    LayerIndex = lyrIndex, 
                    Name = POINTNAME_CAMERALOCATION + "_" + RhinoAppMappings.ActiveViewport.Camera35mmLensLength });
            Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(
                cameraTarget, 
                new ObjectAttributes() { 
                    LayerIndex = lyrIndex, 
                    Name = POINTNAME_CAMERATARGET });

            // donezo
        }

        public static bool ClearOrderData(int order, out Layer layer)
        {
            layer = null;
            if (!_orders.Contains(order)) { return false; }
            layer = _orderLayers[_orders.IndexOf(order)];
            var objectsToDelete = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(layer);
            foreach(var o in objectsToDelete)
            {
                Rhino.RhinoDoc.ActiveDoc.Objects.Delete(o);
            }
            return true;
        }

    }
}