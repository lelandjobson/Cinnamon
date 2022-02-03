using Cinnamon.Components.Object_Rec;
using Cinnamon.Models;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinnamon
{

    [Serializable]
    public class Player
    {
        public PlayStyle PlayStyle { get; set; }

        public Player() { }

        private FrameState _previousState = FrameState.BaseState;

        public Movie Movie
        {
            get => _movie;
            set
            {
                _movie = value; 
            }
        }
        private Movie _movie;


        private Dictionary<Guid, RhinoObject> _docObjects => __docObjects ?? (__docObjects = Rhino.RhinoDoc.ActiveDoc.Objects.GetObjectList(Rhino.DocObjects.ObjectType.AnyObject).ToDictionary(o => o.Id, o => o));
        private Dictionary<Guid, RhinoObject> __docObjects;

        public void Reset()
        {
            __docObjects = null;
        }

        public void ScanFrame(int frame)
        {
            if(_movie == null) { return; }
            FrameState fs = default(FrameState);
            switch (PlayStyle)
            {
                case PlayStyle.StartToEnd:
                    fs = _movie.GetFrameState(frame);
                    break;
                default:
                    throw new Exception("Not implemented yet!");
            }
            RenderState(fs,_previousState);
        }

        public void RenderCameraState(CameraState state)
        {
            //RhinoAppMappings.ActiveViewport.SetCameraLocation(state.PositionState, false);
            //RhinoAppMappings.ActiveViewport.SetCameraTarget(state.TargetPositionState, false);
            if (!double.IsNaN(state.FocalLengthState))
            {
                RhinoAppMappings.ActiveViewport.Camera35mmLensLength = state.FocalLengthState;
            }
            if(state.PositionState != Point3d.Unset && state.TargetPositionState != Point3d.Unset)
            {
                RhinoAppMappings.ActiveViewport.SetCameraLocations(state.TargetPositionState, state.PositionState);
            }
            else if(state.PositionState == Point3d.Unset && state.TargetPositionState != Point3d.Unset)
            {
                RhinoAppMappings.ActiveViewport.SetCameraTarget(state.TargetPositionState, false);
            }
            else if(state.PositionState != Point3d.Unset && state.TargetPositionState == Point3d.Unset)
            {
                RhinoAppMappings.ActiveViewport.SetCameraLocation(state.PositionState, false);
            }
        }

        public void RenderObjectState(ObjectState state)
        {
            var rhObj = state.Id.ToDocumentObject();
            var vec = state.PositionState - rhObj.ToBBPoint();
            //rhObj.Geometry.Translate(vec); 
            //RhinoDoc.ActiveDoc.Objects.Transform(state.Id, Transform.Translation(vec), true);
            Rhino.RhinoDoc.ActiveDoc.Objects.Transform(rhObj.Id, Transform.Translation(vec), true);
        }

        /// <summary>
        /// Renders the frame state.
        /// Beware of running this outside of the context of a movie!!
        /// </summary>
        /// <param name="curState">The state that you want to render</param>
        /// <param name="prevState">Previous state is required for tween animations</param>
        public void RenderState(FrameState curState, FrameState prevState)
        {
            // Camera

            RenderCameraState(curState.CameraState);

            if (curState.HasLayerStates)
            {
                foreach(var ls in curState.LayerStates)
                {
                    if (ls.Value.HasTransparencyState)
                    {
                        ls.Key.SetTransparency(ls.Value.TransparencyState);
                    }
                    ls.Key.IsVisible = ls.Value.IsVisible;
                }
            }

            // Animating Objects
            if (curState.HasObjectsAnimating && prevState != null)
            {
                foreach(var objectInMotion in curState.ObjectPositionStates.Keys)
                {
                    // Move object to location
                    if (_docObjects.TryGetValue(objectInMotion, out var ro) &&
                        prevState.ObjectPositionStates.ContainsKey(objectInMotion))
                    {
                        Point3d start = prevState.ObjectPositionStates[objectInMotion];
                        Point3d end = curState.ObjectPositionStates[objectInMotion];
                        //ro.Geometry.Translate(start.VectorTo(end));
                        Rhino.RhinoDoc.ActiveDoc.Objects.Transform(objectInMotion, Transform.Translation(start.VectorTo(end)), true);
                    }
                }
            }

            _previousState = curState;
        }


        #region Static members

        private static Guid MainPlayerId = Guid.NewGuid();

        private static Dictionary<Guid, Player> _players = new Dictionary<Guid, Player>();

        public static Player MainPlayer
        {
            get
            {
                if (!_players.ContainsKey(MainPlayerId))
                {
                    _players.Add(MainPlayerId, new Player());
                }
                return _players[MainPlayerId];
            }
        }


        #endregion
    }
}
