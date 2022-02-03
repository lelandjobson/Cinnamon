using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Object_Rec
{

    public class PreviewOrder_Object : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public PreviewOrder_Object()
          : base("PreviewOrder", "PreviewOrder",
            "Previews an object order from the document",
            "Cinnamon", "0B_Obj-Rec")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("ObjectId", "ObjectId", "", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Order", "Order", "", GH_ParamAccess.item);
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

            int order = -1;
            string id = "";
            if(!DA.GetData(0, ref id)){ return; }
            if(!DA.GetData(1, ref order)){ return; }
            if(!Guid.TryParse(id, out Guid gid)) { return; }

            if (!Document_OrderManagers.ContainsOrder(gid))
            {
                this.Message = "No orders found in the document for this object";
                return;
            }

            var objState = Document_OrderManagers.GetOrCreateOrderManager(gid).GetOrderData(order);

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