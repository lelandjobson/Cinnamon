using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Cinnamon.Components.Create
{
    public class BuildMovie : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public BuildMovie()
          : base("BuildMovie", "BuildMovie",
            "Builds a movie from scenes",
            "Cinnamon", "2_Build")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Scenes", "Scenes", "Scenes which are in this movie", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Fps", "Fps", "Framerate (frames per second)", GH_ParamAccess.item, 30); // be wary of defaults
            //pManager.AddBooleanParameter("Reset", "Reset", "Clears caching. Hit this if something looking wrong..", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Movie", "Movie", "The compiled movie.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("FrameCount", "FrameCount", 
                "The total number of frames in the movie. " +
                "If you want to connect an int slider to scan individual frames rather than a 0-1 decimal slider, " +
                "you can use this count to inform how big the slider should be.", GH_ParamAccess.item);
        }

        private static Stopwatch _sw = new Stopwatch();

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.Message = "";

            List<Scene> scenes = new List<Scene>();
            int fps = -1;
            //bool reset = false;

            if (!DA.GetDataList<Scene>(0, scenes)) { return; }
            if(!DA.GetData<int>(1, ref fps)) { return; }
            //if (!DA.GetData<bool>(2, ref reset)) { return; }

            _sw.Reset(); _sw.Start();
            var m = new Movie(scenes, fps);
            _sw.Stop();

            this.Message = $"Generated {m.FrameCount} frames from {m.Scenes.Count} scenes with {m.Scenes.Sum(s => s.GetMoments().Sum(mo => mo.GetEffects().Count()))} effects in {_sw.Elapsed.TotalSeconds} seconds";

            DA.SetData(0, m);
            DA.SetData(1, m.FrameCount);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.movie;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("FE72DA7D-A788-4AA3-BB73-9A3435148A23");
    }
}