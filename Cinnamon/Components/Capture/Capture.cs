using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Capture
{

    public class Capture : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Capture()
          : base("Capture", "Capture",
            "Captures the camera or given object location and saves it to an Capture.",
            "Cinnamon", "1_Capture")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "Object", "The object. If blank, uses the camera instead.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Capture#", "Capture#", "The Capture number which to save the state data", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Save", "Save", "Plug in a button and to save!", GH_ParamAccess.item, false);

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
            //this.Message = "WARNING! \n In case of state changes, \n  Use a COPY of your model file, \n not the original.";
            this.Message = "";
            int Capture = -1;
            bool run = false;

            #region Handle ObjectId / Object Overloading
            object theObject = null;
            DA.GetData(0, ref theObject);
            string objectId = string.Empty;
            if (theObject != null)
            {
                if (theObject is string st)
                {
                    objectId = st;
                }
                else if (theObject is GH_Guid id)
                {
                    objectId = id.Value.ToString();
                }
                else if (theObject is Rhino.DocObjects.RhinoObject ro)
                {
                    objectId = ro.Id.ToString();
                }
                else
                {
                    throw new Exception($"Unhandled type {theObject.GetType()}");
                }
            }
            #endregion

            if (!DA.GetData(1, ref Capture)) { return; }
            DA.GetData(2, ref run);

            if (!run) { return; }
            if (string.IsNullOrEmpty(objectId))
            {
                // Capture the camera
                CaptureManager_Camera.CreateNewCapture(Capture);
                this.Message = $"Capture {Capture} Saved";
                return;
            }
            if (!Guid.TryParse(objectId, out Guid gid)) { return; }
            var rhobj = Rhino.RhinoDoc.ActiveDoc.Objects.FindId(gid);   
            if(rhobj == null) {
                this.Message = "Could not find an object with that id.";
                return;
            }

            if (!Document_CaptureManagers.TryGetOrCreateCaptureManager(gid, out var objManager))
            {
                throw new Exception("Could not animate object.");
            }
            objManager.CreateNewCapture(Capture,rhobj.GetCriticalPoint());
            this.Message = $"Capture {Capture} Saved";


            //CaptureManager.CreateNewCapture(Capture);
            //var p = new GH_Path();
            //p.AppendElement(0);

            //// test if this works
            //this.Params.Input[0].AddVolatileData(p, 0, _nextCaptureUp);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.rec_02;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("C655BA31-752A-4FE5-896F-1D8D8A4A211A");
    }
}