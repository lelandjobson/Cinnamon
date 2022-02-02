﻿using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.CameraTools
{

    public class SaveCameraLocation : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public SaveCameraLocation()
          : base("SaveCameraLocation", "SaveCameraLocation",
            "Captures the current camera location and saves it to an order.",
            "Cinnamon", "0_CameraTools")
        {
        }

        private static int _nextOrderUp => OrderManager.Next;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddIntegerParameter("Order", "Order", "", GH_ParamAccess.item,_nextOrderUp);
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
            int order = -1;
            bool run = false;
            DA.GetData(0, ref order);
            DA.GetData(1, ref run);

            if (!run) { return; }

            OrderManager.CreateNewOrder(order);
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
        protected override System.Drawing.Bitmap Icon => Properties.Resources.rec_01;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("FE72DA7D-A788-4AA3-BB73-9B3435148A23");
    }
}