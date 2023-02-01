using Cinnamon.Models;
using Cinnamon.Models.Effects;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Display;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Cinnamon.Components.Create
{
    public class CreatePlayer : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CreatePlayer()
          : base("Player", "Player",
            "Playes a movie",
            "Cinnamon", "4_Play")
        {
        }

        ~CreatePlayer()
        {
            _activeDocumentPlayers.Remove(this._playerId);
            if (_playerActive && _activeDocumentPlayers.Count == 0) { _playerActive = false; }
        }

        private Guid _playerId = Guid.NewGuid();

        private static bool _playerActive
        {
            get => __playerActive;
            set
            {
                if(value == __playerActive) { return; }
                if (value)
                {
                    DisplayConduitManager.RenderMessage(0, "Cinnamon Player is Active!");
                }
                else
                {
                    DisplayConduitManager.HideMessage(0);
                }
                __playerActive = value;
            }
        }
        private static bool __playerActive = false;

        private HashSet<Guid> _activeDocumentPlayers = new HashSet<Guid>();

        private static Player _mainPlayer => __mainPlayer ?? (__mainPlayer = Player.DefaultPlayer);
        private static Player __mainPlayer;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Movie", "Movie", "The movie to play", GH_ParamAccess.item);
            pManager.AddNumberParameter("Seek", "Seek", "The state in the movie to play. If byFrame is true, then the number of the frame.", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("by Frame?", "by Frame?", 
                "If false, then a number between 0 and 1 will be turned into the percentage of the movie to be rendered. " +
                "If true, then the number of the frame will be used.", GH_ParamAccess.item, false);

            pManager.AddBooleanParameter("Reset View", "Reset View", "On play, resets the view to the base state of the document's camera.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Enabled", "Enabled", "Enables or disabled the player component", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Show Warning", "Show Warning", "Enables or disabled the player active warning", GH_ParamAccess.item, true);

            pManager[3].Optional = true;
            pManager[5].Optional = true;
            // TODO ADD RESET VIEW
            // Ensure correct viewport is rendering
            //

            // TODO add enabled! 
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        private IEffect _pastEffect;
        private Movie _pastMovie;

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            _activeDocumentPlayers.Add(_playerId);

            this.Message = "";
            ICanUseInPlayer data = null;
            double seek = 0;
            bool byFrame = false;
            Movie m = null;
            bool enabled = false;
            bool reset = false;
            bool showWarning = true;

            DA.GetData(3, ref reset);
            if(!DA.GetData(4, ref enabled) || !enabled) { 
                _playerActive = false;
                return; 
            }
            else
            {
                DA.GetData(5, ref showWarning);
                _playerActive = showWarning;
            }

            try
            {
                if (!DA.GetData(0, ref data)) { return; }
                // Check if its an effect instead
                if (data is IEffect effect)
                {
                    if (_pastEffect != null && effect.Id == _pastEffect.Id)
                    {
                        m = _pastMovie;
                    }
                    else
                    {
                        m = Movie.OfJustOneEffect(effect, 3);
                        _pastEffect = effect;
                        _pastMovie = m;
                    }
                }
                else if (data is Movie mov)
                {
                    m = mov;
                } 

                if (!DA.GetData<double>(1, ref seek)) { return; }
                DA.GetData(2, ref byFrame);

                if (byFrame) { SeekFrame(seek.ToInt32(), m); }
                else { SeekFrame(((m.FrameCount - 1) * seek).ToInt32(), m); }
            }
            catch
            {
                this.Message = "Unable to render movie.";
            }
        }

        void SeekFrame(int frame, Movie m)
        {
            _mainPlayer.Movie = m;
            _mainPlayer.ScanFrame(frame);
            this.Message = $"Showing \n {frame + 1}/{m.FrameCount}";
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.play_main;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("AB72DA7D-A788-4EC3-BB73-9A3435148C63");
    }
} 