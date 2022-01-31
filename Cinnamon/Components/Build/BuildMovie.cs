using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

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
            pManager.AddGenericParameter("Scenes", "Scenes", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Fps", "Fps", "", GH_ParamAccess.item, 30); // be wary of defaults
            pManager.AddBooleanParameter("Reset", "Reset", "", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Movie", "Movie", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("FrameCount", "FrameCount", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Scene> scenes = new List<Scene>();
            int fps = -1;
            bool reset = false;

            if (!DA.GetDataList<Scene>(0, scenes)) { return; }
            if(!DA.GetData<int>(1, ref fps)) { return; }
            if (!DA.GetData<bool>(2, ref reset)) { return; }

            

            if (reset)
            {

            }

            var m = new Movie(scenes, fps);

            DA.SetData(0, m);
            DA.SetData(1, m.FrameCount);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("FE72DA7D-A788-4AA3-BB73-9A3435148A23");
    }
}