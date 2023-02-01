using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Capture
{

    public class SavedCaptures : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SavedCaptures()
          : base("SavedCaptures", "SavedCaptures",
            "Loads saved captures from the document",
            "Cinnamon", "1_Capture")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "Object", "The object. If blank, uses the camera instead.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset", "Reset", "Recalculates. Have this plugged into your capture button.", GH_ParamAccess.item);

            pManager[0].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Captures", "Captures", "Captures found in the document", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.Message = "";
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
            if (string.IsNullOrEmpty(objectId))
            {
                // Camera
                this.Message = "Camera";
                DA.SetDataList(0, CameraCaptureManager.Default.CaptureKeys);
                return;
            }
            if (!Guid.TryParse(objectId, out var objectIdGuid)) { return; }

            //if (!Document_CaptureManagers.ContainsCapture(objectIdGuid)) { this.Message = "Could not find Captures \n for that object."; return; }
            this.Message = "Object";

            if(!DocumentCaptureManagers.TryGetOrCreateObjectCaptureManager(objectIdGuid, out var capMap))
            {
                DA.SetDataList(0, new List<int>());
            }
            else
            {
                DA.SetDataList(0, capMap.CaptureKeys ?? new List<int>());
            }
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.list_02;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("C655BA31-752A-4FE5-896F-1D8D8B1A153C");
    }
}