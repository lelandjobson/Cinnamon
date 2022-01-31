using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Models
{
    public class CameraState
    {

        public Point3d PositionState = Point3d.Unset;

        public Point3d TargetPositionState = Point3d.Unset;

        public double FocalLengthState = -1;


    }

    [Serializable]
    public class FrameState
    {

        #region State Flags

        public bool HasCameraPositionData => CameraState.PositionState != Point3d.Unset;

        public bool HasCameraTargetData => CameraState.TargetPositionState != Point3d.Unset;

        public bool HasObjectsAnimating { get; private set; } = false;

        #endregion

        #region State data

        public CameraState CameraState = new CameraState();

        public Dictionary<Guid, Point3d> ObjectPositionStates
        {
            get
            {
                if(_objectPositionStates == null) { _objectPositionStates = new Dictionary<Guid,Point3d>(); }
                HasObjectsAnimating = true;
                return _objectPositionStates;
            }
        }
        private Dictionary<Guid, Point3d> _objectPositionStates;


        public static FrameState GenerateFromDocument(Movie m, int frame)
        {
            return new FrameState(m, 0);
        }
        #endregion

        public readonly int KeyFrame = -1;
        private Movie _parent;


        public FrameState(Movie movie, int frameNumber) { _parent = movie; KeyFrame = frameNumber; }

        #region Base state

        /// <summary>
        /// The original framestate of all of the objects in the rhino document.
        /// Used to properly reset all of the objects to their locations.
        /// </summary>
        public static FrameState BaseState
        {
            get
            {
                if (_baseState == null) { ExpireBaseState(false); }
                return _baseState;
            }
        }
        private static FrameState _baseState;

        public static void ExpireBaseState(bool resetDocObjects = true)
        {
            if (resetDocObjects && _baseState != null)
            {

            }
            _baseState = _NewBaseState();
        }

        static FrameState _NewBaseState()
        {
            var baseState = new FrameState(null, 0)
            {
                _objectPositionStates = Rhino.RhinoDoc.ActiveDoc.Objects
                .GetObjectList(Rhino.DocObjects.ObjectType.AnyObject)
                .ToDictionary(o => o.Id, o => Point3d.Origin),
                CameraState = new CameraState()
                {
                    PositionState = RhinoAppMappings.ActiveView.ActiveViewport.CameraLocation,
                    TargetPositionState = RhinoAppMappings.ActiveView.ActiveViewport.CameraTarget,
                    FocalLengthState = RhinoAppMappings.ActiveView.ActiveViewport.Camera35mmLensLength,
                }
            };
            return baseState;
        }


        #endregion
    }
}
