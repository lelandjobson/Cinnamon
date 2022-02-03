using Cinnamon.Models;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Object_Rec
{

    public class OrderManager_Object
    {
        public event EventHandler OrderChanged;
        public List<int> Orders
        {
            get
            {
                RegenOrderData();
                return _orders?.ToList() ?? new List<int>();
            }
        }
        private List<int> _orders;
        private List<Layer> _orderLayers;

        private static LayerTable _rhlyrs => Rhino.RhinoDoc.ActiveDoc.Layers;

        public const string POINTNAME_LOCATION = "objloc";

        public readonly string ObjectId;
        public readonly Guid ObjectIdGuid;

        public OrderManager_Object(Guid objectId)
        {
            ObjectIdGuid = objectId;
            ObjectId = objectId.ToString();
        }

        private Layer _orderLayersParent
        {
            get
            {
                var parent = _rhlyrs.FindName(ObjectId);
                if (parent == null)
                {
                    // create parent layer
                    var p = new Layer()
                    {
                        Name = ObjectId,
                        IsVisible = false,
                        IsLocked = true
                    };
                    _rhlyrs.Add(p);
                    return _rhlyrs.FindName(ObjectId);
                }
                else
                {
                    return parent;
                }
            }
        }

        public int Next
        {
            get
            {
                RegenOrderData();
                if(Orders.Count == 0) { return 0; }
                return Orders.Last() + 1;
            }
        }

        void RegenOrderData()
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

        internal ObjectState GetOrderData(int order)
        {
            if (!Orders.Contains(order)) { return null; }
            int idx = Orders.IndexOf(order);
            var objs = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(_orderLayers[idx]);
            ObjectState output = new ObjectState(this.ObjectIdGuid);
            foreach(var o in objs)
            {
                if (o.Name.Contains(POINTNAME_LOCATION))
                {
                    double focalLength = RhinoAppMappings.ActiveViewport.Camera35mmLensLength;
                    double.TryParse(o.Name.Split('_').Last(), out focalLength);
                    output.PositionState = (o.Geometry as Point).Location;
                }
            }
            return output;
        }

        public void CreateNewOrder(int order, Point3d location = default(Point3d))
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

            // Create objects
            Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(
                location,
                new ObjectAttributes() { 
                    LayerIndex = lyrIndex, 
                    Name = POINTNAME_LOCATION });

            // donezo
        }

        public bool ClearOrderData(int order, out Layer layer)
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