using Cinnamon.Components.CameraTools;
using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Object_Rec
{
    public class NextOrder_Object : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public NextOrder_Object()
          : base("NextOrder", "NextOrder",
            "Provides the next order",
            "Cinnamon", "0B_Obj-Rec")
        {
        }

        private static int _order = 0;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Id", "Id", "", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Reset", "Reset", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("NextOrder", "NextOrder", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string id = string.Empty;
            if(!DA.GetData(0,ref id)) { return; }
            if(!Guid.TryParse(id, out Guid gId)) { return; }
            var omg = Document_OrderManagers.GetOrCreateOrderManager(gId);
            _order = omg.Next;
            this.Message = $"{_order}";
            DA.SetData(0, _order);
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