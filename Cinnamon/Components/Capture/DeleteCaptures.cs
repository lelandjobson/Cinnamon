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

    public class DeleteCaptures : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public DeleteCaptures()
          : base("DeleteCaptures", "DeleteCaptures",
            "Deletes saved captures from the document",
            "Cinnamon", "1_Capture")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "Object", "The object. If blank, uses the camera instead.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("CaptureIds", "CaptureIds", "The captures to delete.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Delete", "Delete", "Deletes the CaptureIds provided in the CaptureIds parameter.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("DeleteAll", "DeleteAll", "Deletes all captures for this if hit.", GH_ParamAccess.item, false);

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

            List<int> captures = new List<int>();
            DA.GetDataList(1, captures);
            bool delete = false;
            DA.GetData(2, ref delete);
            bool deleteAll = false;
            DA.GetData(3, ref deleteAll);

            bool success = false;
            if (string.IsNullOrEmpty(objectId))
            {
                // Camera
                if (deleteAll)
                {
                    foreach(var c in CaptureManager_Camera.Captures)
                    {
                        success = CaptureManager_Camera.ClearCaptureData(c, out var l) || success;
                        CaptureManager_Camera.DeleteLayer(l);
                    }    
                }
                else if(captures == null || captures.Count == 0 || !delete) { return; }
                else {
                    foreach(var c in captures)
                    {
                        success = CaptureManager_Camera.ClearCaptureData(c, out var l) || success;
                        CaptureManager_Camera.DeleteLayer(l);
                    }
                }
            }
            else if (Guid.TryParse(objectId, out var objectIdGuid))
            {
                if(!Document_CaptureManagers.TryGetOrCreateCaptureManager(objectIdGuid, out var capManager)) { return; }

                if (deleteAll)
                {
                    foreach (var c in CaptureManager_Camera.Captures)
                    {
                        success = capManager.ClearCaptureData(c, out var l) || success;
                        capManager.DeleteLayer(l);
                    }
                }
                else if (captures == null || captures.Count == 0 || !delete) { return; }
                else
                {
                    foreach (var c in captures)
                    {
                        success = capManager.ClearCaptureData(c, out var l) || success;
                        capManager.DeleteLayer(l);
                    }
                }
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
        public override Guid ComponentGuid => new Guid("C115BA31-752A-4AA5-896F-1D8D8B1A153C");
    }
}