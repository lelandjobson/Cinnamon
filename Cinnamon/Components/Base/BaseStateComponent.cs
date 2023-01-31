using Cinnamon.Models;
using GH_IO.Types;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Base
{

    public class BaseStateComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public BaseStateComponent()
          : base("BaseStateStats", "BaseStateStats",
            "Shows base state stats for the document",
            "Cinnamon", "0_Base")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Reset", "Reset", "Resets the base state of the document", GH_ParamAccess.item);
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
            bool reset = false;
            if(DA.GetData<bool>(0, ref reset) && reset)
            {
                // Reset base state
                DocumentBaseState.ActiveBase.Reset();
            }

            // Read out statistics
            this.Message = $"Initialized: {DocumentBaseState.HasBeenInitialized} \n" +
                $"Objects: {DocumentBaseState.ActiveBase.ObjectBaseStateCount} \n" +
                //$"Path: {DocumentBaseState.ActiveBase.SavePath} +" +
                $"Viewport: {DocumentBaseState.ActiveBase.Viewport.Name}";
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.play_02;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("C655BA32-152A-4FE5-891F-1D8D8A4A153C");
    }
}