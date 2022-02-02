using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Create
{
    public class RestoreBaseState : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public RestoreBaseState()
          : base("Restore", "Restore",
            "Captures the base state of the document. Press button to restore state.",
            "Cinnamon", "3_Play")
        {
        }

        private static Player _mainPlayer => __mainPlayer ?? (__mainPlayer = Player.MainPlayer);
        private static Player __mainPlayer;

        private FrameState _restoreState;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
  
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Restore", "Restore", "", GH_ParamAccess.item, false);
            // Save base state
            _restoreState = FrameState.CurrentDocumentState;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            _mainPlayer.RenderState(_restoreState, null);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.create_rewind;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("AB72BBBB-A788-4EC3-BB73-9A3435148C63");
    }
} 