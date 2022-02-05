using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Capture
{
    public class NextCapture : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public NextCapture()
          : base("NextCapture", "NextCapture",
            "Provides the next Capture",
            "Cinnamon", "0_Capture")
        {
        }

        private static int _Capture = 0;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ObjectId", "ObjectId", "Leave empty for camera captures. Otherwise the object id to retreive Captures for", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset", "Reset", "Recalculates. Have this plugged into your capture button.", GH_ParamAccess.item);

            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("NextCapture", "NextCapture", "The next unused Capture to save the capture to", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.Message = "";
            string id = string.Empty;
            DA.GetData(0, ref id);

            if (string.IsNullOrEmpty(id))
            {
                // camera
                this.Message = $"Camera: {CaptureManager_Camera.Next}";
                DA.SetData(0, CaptureManager_Camera.Next);
                return;
            }
            if(!Guid.TryParse(id, out Guid gId)) { return; }
            //this.Message = "Object";
            var omg = Document_CaptureManagers.GetOrCreateCaptureManager(gId);
            //_Capture = omg.Next;
            this.Message = $"Object: {omg.Next}";
            DA.SetData(0, omg.Next);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.next_02;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("C655BA31-752A-4FE5-AB6F-1D8D8A4A253C");
    }
}