using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Capture
{

    public class PreviewCapture : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PreviewCapture()
          : base("PreviewCapture", "PreviewCapture",
            "Previews an object Capture from the document",
            "Cinnamon", "0_Capture")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ObjectId", "ObjectId", "Leave empty to preview camera. Otheriwise the id of the object which was captured", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Capture", "Capture", "The number of the Capture to preview", GH_ParamAccess.item);

            pManager[0].Optional = true;
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
            this.Message = string.Empty;

            int Capture = -1;
            string id = "";
            DA.GetData(0, ref id);
            if (!DA.GetData(1, ref Capture)){ return; }
            if (string.IsNullOrEmpty(id))
            {
                // camera
                Player.MainPlayer.RenderCameraState(CaptureManager_Camera.GetCaptureData(Capture));
                this.Message = "Rendering Camera";
                return;
            }
            if(!Guid.TryParse(id, out Guid gid)) { return; }

            if (!Document_CaptureManagers.ContainsCapture(gid))
            {
                this.Message = "No Captures found in the document for this object";
                return;
            }

            var objState = Document_CaptureManagers.GetOrCreateCaptureManager(gid).GetCaptureData(Capture);
            this.Message = "Rendering Object";
            Player.MainPlayer.RenderObjectState(objState);
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
        public override Guid ComponentGuid => new Guid("C655BA31-752A-4FE5-896F-1D8D8A4A153C");
    }
}