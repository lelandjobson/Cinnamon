//using Cinnamon.Models;
//using Cinnamon.Models.Effects;
//using Grasshopper;
//using Grasshopper.Kernel;
//using Rhino.DocObjects;
//using Rhino.Geometry;
//using System;
//using System.Collections.Generic;

//namespace Cinnamon.Components.Create
//{
//    public class Animate_ChangeLayerTransparencyEffect : GH_Component
//    {
//        /// <summary>
//        /// Each implementation of GH_Component must provide a public 
//        /// constructor without any arguments.
//        /// Category represents the Tab in which the component will appear, 
//        /// Subcategory the panel. If you use non-existing tab or panel names, 
//        /// new tabs/panels will automatically be created.
//        /// </summary>
//        public Animate_ChangeLayerTransparencyEffect()
//          : base("ChangeLayerTransparency", "ChangeLayerTransparency",
//            "Changes the transparency of the materials on objects on layers",
//            "Cinnamon", "1_Effects")
//        {
//        }

//        /// <summary>
//        /// Registers all the input parameters for this component.
//        /// </summary>
//        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
//        {
//            pManager.AddTextParameter("Layers", "Layers", "", GH_ParamAccess.list);
//            pManager.AddNumberParameter("Start", "Start", "", GH_ParamAccess.item);
//            pManager.AddNumberParameter("End", "End", "", GH_ParamAccess.item);

//            //pManager.AddNumberParameter("States", "States", "", GH_ParamAccess.list);
//        }

//        /// <summary>
//        /// Registers all the output parameters for this component.
//        /// </summary>
//        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
//        {
//            pManager.AddGenericParameter("Effect", "Effect", "", GH_ParamAccess.item);
//        }

//        /// <summary>
//        /// This is the method that actually does the work.
//        /// </summary>
//        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
//        /// to store data in output parameters.</param>
//        protected override void SolveInstance(IGH_DataAccess DA)
//        {
//            List<string> layerNames = new List<string>();
//            //List<bool> layerStates = new List<bool>();
//            double start = 0;
//            double end = 0;

//            if (!DA.GetDataList(0, layerNames)) { return; }
//            if(!DA.GetData(1, ref start)) { return; }
//            if(!DA.GetData(2, ref end)) { return; }

//            var transEffect = new LayerFadeEffect(layerNames, start, end);

//            DA.SetData(0, transEffect);


//            //if(!DA.GetDataList(1, layerStates)) { return; }

//            // LJ
//            // TODO
//            // Consider warnings system which passes through

//            // effects setting.
//            //List<string> warnings = new List<string>();

//            //if(warnings.Count > 0)
//            //{
//            //    this.Message = String.Join("\n", warnings);
//            //}
//            //else
//            //{
//            //    this.Message = String.Empty;
//            //}
//        }

//        /// <summary>
//        /// Provides an Icon for every component that will be visible in the User Interface.
//        /// Icons need to be 24x24 pixels.
//        /// You can add image files to your project resources and access them like this:
//        /// return Resources.IconForThisComponent;
//        /// </summary>
//        protected override System.Drawing.Bitmap Icon => Properties.Resources.eff_01; 

//        /// <summary>
//        /// Each component must have a unique Guid to identify it. 
//        /// It is vital this Guid doesn't change otherwise old ghx files 
//        /// that use the old ID will partially fail during loading.
//        /// </summary>
//        public override Guid ComponentGuid => new Guid("FB22DAAA-B288-4EC3-BB73-1A3435148C61");
//    }
//} 