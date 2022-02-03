using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Object_Rec
{

    public class SaveObjectOrder : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SaveObjectOrder()
          : base("SaveObjectOrder", "SaveObjectOrder",
            "Captures the current object location and saves it to an order.",
            "Cinnamon", "0B_Obj-Rec")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ObjectId", "ObjectId", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Order", "Order", "", GH_ParamAccess.item, 0);
            pManager.AddBooleanParameter("Save", "Save", "", GH_ParamAccess.item, false);
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
            string id = "";
            int order = -1;
            bool run = false;
            DA.GetData(0, ref id);
            DA.GetData(1, ref order);
            DA.GetData(2, ref run);

            if (!run) { return; }
            if (!Guid.TryParse(id, out Guid gid)) { return;  }
            var rhobj = Rhino.RhinoDoc.ActiveDoc.Objects.FindId(gid);   
            if(rhobj == null) {
                this.Message = "Could not find an object with that id.";
                return;
            }

            var bb = rhobj.Geometry?.GetBoundingBox(true) ?? null;
            if(bb == null) {
                this.Message = "Could not produce an order from this object.";
                return; 
            }

            var orderManager = Document_OrderManagers.GetOrCreateOrderManager(gid);
            orderManager.CreateNewOrder(order, bb.Value.Center);


            //OrderManager.CreateNewOrder(order);
            //var p = new GH_Path();
            //p.AppendElement(0);

            //// test if this works
            //this.Params.Input[0].AddVolatileData(p, 0, _nextOrderUp);
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