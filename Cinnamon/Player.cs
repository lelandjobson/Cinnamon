using Cinnamon.Components.Capture;
using Cinnamon.Models;
using Rhino;
using Rhino.DocObjects;
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

        public static Player DefaultPlayer = new Player();

        public Player() {
        }


        private FrameState _previousState = DocumentBaseState.ActiveBase.FrameState;


        public Movie Movie
        {
            get => _movie;
            set
            {
                if(_movie == value) { return; }
                _movie = value;
            }
        }
        private Movie _movie;

        private int _previousFrame = -1;


        private static Dictionary<Guid, RhinoObject> _docObjects => __docObjects ?? (__docObjects = RhinoAppMappings.ActiveDoc.Objects.GetObjectList(Rhino.DocObjects.ObjectType.AnyObject).ToDictionary(o => o.Id, o => o));
        private static Dictionary<Guid, RhinoObject> __docObjects;

        /// <summary>
        /// Resets the cache of docobjects
        /// </summary>
        public static void ExpireDocObjects() => __docObjects = null;

        public void Reset()
        {
            __docObjects = null;
        }

        public void ScanFrame(int frame, bool backTracking = false)
        {
            if (_movie == null || frame < 0) { return; }

            if(!backTracking)
            {
                if (_previousFrame > frame)
                {
                    // Rewind
                    for (int i = _previousFrame - 1; i > frame; i--)
                    {
                        ScanFrame(i, true);
                    }
                }
                else if (_previousFrame < frame)
                {
                    // Forward
                    for (int i = _previousFrame + 1; i < frame; i++)
                    {
                        ScanFrame(i, true);
                    }
                }
            }
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

            if (!backTracking) { _previousFrame = frame; }            
        }

        public void RenderCameraState(CameraState state)
        {
            state?.Apply();
        }

        public void RenderObjectState(ObjectOrientationState state)
        {
            state?.Apply();
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

            // Frame actions
            foreach(var a in curState.FrameActions)
            {
                a.Invoke(curState);
            }

            // Animating Objects
            if (curState.HasObjectsAnimating)
            {
                foreach(var objectInMotion in curState.ObjectPositionStates.Keys)
                {
                    RenderObjectState(curState.ObjectPositionStates[objectInMotion]);
                }
            }

            _previousState = curState;
        }
    }
}
