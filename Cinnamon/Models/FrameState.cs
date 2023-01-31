using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Models
{

    [Serializable]
    public class FrameState
    {

        #region State Flags

        public bool HasCameraPositionData => CameraState.PositionState != Point3d.Unset;

        public bool HasCameraTargetData => CameraState.TargetPositionState != Point3d.Unset;

        public bool HasObjectsAnimating { get; private set; } = false;

        #endregion


        /// <summary>
        /// State of the current document.
        /// </summary>
        public static FrameState BaseState() =>
            new FrameState(null, 0)
            {
                _layerStates = RhinoAppMappings.DocumentLayers.ToDictionary(
                    l => l,
                    l => new LayerState { IsVisible = l.IsVisible, TransparencyState = l.GetTransparency() }
                    ),

                _objectPositionStates = Rhino.RhinoDoc.ActiveDoc.Objects
                    .GetObjectList(Rhino.DocObjects.ObjectType.AnyObject)
                    .ToDictionary(o => o.Id, o => o.TryGetOrientationState(out var state) ? state : new SinglePointObjectOrientationState(o.Id, Point3d.Unset)),
                CameraState = new CameraState(DocumentBaseState.ActiveBase.ViewportId)
                {
                    PositionState = RhinoAppMappings.ActiveView.ActiveViewport.CameraLocation,
                    TargetPositionState = RhinoAppMappings.ActiveView.ActiveViewport.CameraTarget,
                    FocalLengthState = RhinoAppMappings.ActiveView.ActiveViewport.Camera35mmLensLength,
                }
            };

        #region State data

        public CameraState CameraState { get; set; }

        public List<Action<FrameState>> FrameActions = new List<Action<FrameState>>();

        public Dictionary<Guid, ObjectOrientationState> ObjectPositionStates
        {
            get
            {
                if(_objectPositionStates == null) { _objectPositionStates = new Dictionary<Guid, ObjectOrientationState>(); }
                HasObjectsAnimating = true;
                return _objectPositionStates;
            }
        }
        private Dictionary<Guid, ObjectOrientationState> _objectPositionStates;

        public bool HasLayerStates = false;

        public Dictionary<Layer, LayerState> LayerStates
        {
            get
            {
                if(_layerStates == null) { _layerStates = new Dictionary<Layer,LayerState>(); }
                HasLayerStates = true;
                return _layerStates;
            }
        }
        private Dictionary<Layer, LayerState> _layerStates;

        public static FrameState GenerateFromDocument(Movie m, int frame)
        {
            return new FrameState(m, 0);
        }
        #endregion

        public readonly int KeyFrame = -1;
        private Movie _parent;


        public FrameState(Movie movie, int frameNumber) { _parent = movie; KeyFrame = frameNumber; }
    }
}
