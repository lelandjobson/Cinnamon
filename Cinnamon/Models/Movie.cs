using Cinnamon.Models.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinnamon.Models
{

    /// <summary>
    /// A compiled movie with effects
    /// </summary>
    public class Movie : ICanUseInPlayer, IEquatable<Movie>
    {
        public readonly Guid Id = Guid.NewGuid();

        public bool IsReady { get; private set; } = false;
        public bool IsProcessingFrames { get; private set; } = false;

        public bool InitializeWithDocumentState = false;

        public int Loops { get; private set; } = 1;

        public readonly List<Scene> Scenes = new List<Scene>();
        public readonly TimelineTime Duration = TimelineTime.Empty;
        public readonly int FrameCount = 0;
        public readonly int Fps = 0;

        private FrameState[] _frames
        {
            get
            {
                if (__frames == null) { GenerateFrameState(); }
                return __frames;
            }
        }

        internal FrameState GetFrameState(int frame)
        {
            return _frames[Math.Min(frame, _frames.Length - 1)];
        }

        private FrameState[] __frames;

        public Movie(List<Scene> scenes, int fps, int loops = 1, bool generateFrameState = true)
        {
            Fps = fps;
            Scenes = scenes ?? this.Scenes;
            Loops = loops;
            Duration =
                (Scenes == null || Scenes.Count == 0) ?
                this.Duration :
                new TimelineTime() { Start = 0, End = Scenes.Max(s => s.Range.End) };
            FrameCount =
                (Scenes == null || Scenes.Count == 0) ?
                0 :
                System.Convert.ToInt32(Math.Ceiling(this.Duration.End * Fps));
            if (generateFrameState)
            {
                GenerateFrameState();
            }
        }

        public void ExpireFrameState() => __frames = null;

        public void GenerateFrameState()
        {
            IsReady = false;
            IsProcessingFrames = true;

            try
            {

                __frames = new FrameState[FrameCount * Loops];
                __frames.Fill();

                // Organize moments by frame
                // Create framestates where necessary
                // Refer to prior framestates if there is no active frame.

                // For each scene - For each moment
                // - get start frame in movie & fps
                // - use this to get the duration
                // - foreach frame
                //   - calculate the frame value
                //   - apply frame value state. overwrite

                // When scrubbing
                // - just load the state into the scene
                //   - state should be definite and not relative to any previous state, such as using vectors.
                //   - for object locations and orientation, it seems best to initialze a dictionary.
                //     then iterate through keys, grabbing the matching element and updating the state.

                foreach (var s in Scenes)
                {
                    foreach (var m in s.GetMoments())
                    {
                        if (!m.IsValid) { continue; }
                        int frameStart = Math.Floor(m.Time.Start * Fps).ToInt32();
                        int frameEnd = Math.Ceiling((m.Time.Duration * Fps)+ frameStart).ToInt32();
                        for (int i = frameStart; i < frameEnd; i++)
                        {
                            if (_frames[i] == null) { _frames[i] = new FrameState(this, i); }
                            FrameState frameState = _frames[i];
                            double effectPerc = AnimationCurveExtensions.GetNormalizedValue(m.Curve, i.Remap(frameStart, frameEnd - 1, 0, 1));
                            foreach (var e in m.GetEffects())
                            {
                                e.SetFrameStateValue(effectPerc, frameState);
                            }
                        }
                    }
                }

                // Set empty framestates
                if (_frames[0] == null)
                {
                    _frames[0] = FrameState.GenerateFromDocument(this, 0);
                }
                for (int i = 1; i < FrameCount; i++)
                {
                    if (_frames[i] == null)
                    {
                        // Fill in empty frame with previous state
                        _frames[i] = _frames[i - 1];
                    }
                }

                // Create loops
                for(int i = 1; i<Loops; i++)
                {
                    int cur = FrameCount * i;
                    for (int j = 0; j<FrameCount; j++)
                    {
                        _frames[cur] = _frames[j];
                        cur++;
                    }
                }

                // Okay, we're good to go.
                IsProcessingFrames = false;
                IsReady = true;
            } 
            catch(Exception e)
            { 
                IsProcessingFrames = false; 
                Logger.LogException(e); 
            }
        }

        internal static Movie OfJustOneEffect(IEffect effect, int duration = 1, int frameRate = 30)
        {
            return new Movie(
                new List<Scene>() {
                    new Scene("TestingScene",
                    new List<Moment>() {
                        new Moment(
                            new TimelineTime() { Start = 0, End = duration },
                            AnimationCurve.EaseInOut,
                            new List<IEffect>() { effect })
                    })
                }, frameRate);
        }

        public bool Equals(Movie other)
        {
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.ToString().GetHashCode();
        }
    }
}
