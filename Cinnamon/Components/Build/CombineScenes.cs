using Cinnamon.Models;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Create
{
    public class CombineScenes : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CombineScenes()
          : base("CombineScenes", "CombineScenes",
            "Combines scenes together into one",
            "Cinnamon", "2_Build")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Scenes", "Scenes", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("CompilationStrategy", "CompStrat", "", GH_ParamAccess.item, 1);

            var p = pManager[1] as Param_Integer;
            p.AddNamedValuesForEnum(typeof(SceneCompilationStrategy));
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Scene", "Scene", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Scene> scenes = new List<Scene>();
            int compilationStrategy = 1;
            if (!DA.GetDataList<Scene>(0, scenes)) { return; }
            DA.GetData<int>(1, ref compilationStrategy);

            // Default is sequence
            SceneCompilationStrategy strat = SceneCompilationStrategy.Sequence;
            strat = (SceneCompilationStrategy)compilationStrategy;

            var scene = Scene.Compile(scenes,strat);

            DA.SetData(0, scene);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.join_scenes;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("FEA2DA7D-A788-4EC3-AA73-9A3435148C63");
    }
} 