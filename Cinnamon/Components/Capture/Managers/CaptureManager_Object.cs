using Cinnamon.Models;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Capture
{

    public class CaptureManager_Object
    {
        public event EventHandler CaptureChanged;
        public List<int> Captures
        {
            get
            {
                RegenCaptureData();
                return _Captures?.ToList() ?? new List<int>();
            }
        }
        private List<int> _Captures;
        private List<Layer> _CaptureLayers;

        private static LayerTable _rhlyrs => Rhino.RhinoDoc.ActiveDoc.Layers;

        public const string POINTNAME_LOCATION = "objloc";

        public readonly string ObjectId;
        public readonly Guid ObjectIdGuid;

        public CaptureManager_Object(Guid objectId)
        {
            ObjectIdGuid = objectId;
            ObjectId = objectId.ToString();
        }

        private Layer _CaptureLayersParent
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
                        //IsLocked = true
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
                RegenCaptureData();
                if(Captures.Count == 0) { return 0; }
                return Captures.Last() + 1;
            }
        }

        void RegenCaptureData()
        {
            // gather doc layers
            _Captures = null;
            _Captures = new List<int>();
            _CaptureLayers = new List<Layer>();

            foreach (var l in _rhlyrs.Where(l => l.ParentLayerId == _CaptureLayersParent.Id))
            {
                int Capture = -1;
                if(!Int32.TryParse(l.Name, out Capture)) { continue; }
                _Captures.Add(Capture);
                _CaptureLayers.Add(l);
            }
            if(_Captures.Count == 0) { _Captures = null; return; }
            _Captures.Sort();
            _CaptureLayers.Sort((a, b) => Int32.Parse(a.Name).CompareTo(Int32.Parse(b.Name)));
            //CaptureChanged?.Invoke(null, null);
        }

        internal ObjectState GetCaptureData(int Capture)
        {
            if (!Captures.Contains(Capture)) { return null; }
            int idx = Captures.IndexOf(Capture);
            var objs = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(_CaptureLayers[idx]);
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

        public void CreateNewCapture(int Capture, Point3d location = default(Point3d))
        {
            Layer l = null;
            int lyrIndex = -1;
            if (Captures.Contains(Capture))
            {
                // Clear geometry from Capture
                ClearCaptureData(Capture, out l);
                lyrIndex = l.Index;
            }
            else
            {
                // Create Capture layers
                l = new Layer() {
                    Name = Capture.ToString(),
                    IsVisible = false,
                    //IsLocked = true,
                    ParentLayerId = _CaptureLayersParent.Id
                };
                lyrIndex = Rhino.RhinoDoc.ActiveDoc.Layers.Add(l);
                l.ParentLayerId = _CaptureLayersParent.Id;
                RegenCaptureData();
            }

            // Create objects
            Rhino.RhinoDoc.ActiveDoc.Objects.AddPoint(
                location,
                new ObjectAttributes() { 
                    LayerIndex = lyrIndex, 
                    Name = POINTNAME_LOCATION });

            // donezo
        }

        public bool ClearCaptureData(int Capture, out Layer layer)
        {
            layer = null;
            if (!_Captures.Contains(Capture)) { return false; }
            layer = _CaptureLayers[_Captures.IndexOf(Capture)];
            var objectsToDelete = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(layer);
            foreach(var o in objectsToDelete)
            {
                Rhino.RhinoDoc.ActiveDoc.Objects.Delete(o);
            }
            return true;
        }

    }
}