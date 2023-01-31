using Cinnamon.Models;
using Cinnamon.Models.Effects;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Create
{
    public class Animate_ChangeLayerVisibilityEffect : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Animate_ChangeLayerVisibilityEffect()
          : base("ChangeLayerVisibility", "ChangeLayerVisibility",
            "Changes the visibility of provided layers in the document at an interval of time",
            "Cinnamon", "2_Animate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Layers", "Layers", "The layers to change the state of", GH_ParamAccess.list);
            pManager.AddBooleanParameter("States", "States", "The visibility states of the layers. If you provide fewer states than layers, they will be copied.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("HideOthers", "HideOthers", "If true, hides all of the unprovided layers.", GH_ParamAccess.item, false);

            //pManager.AddNumberParameter("States", "States", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Effect", "Effect", "", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            this.Message = string.Empty;

            List<string> layerNames = new List<string>();
            List<bool> states = new List<bool>();
            bool hideOthers = false;

            if (!DA.GetDataList(0, layerNames)) { return; }
            if (!DA.GetDataList(1, states)) { return; }
            DA.GetData(2, ref hideOthers);

            List<IEffect> effects = new List<IEffect>();

            int shownCount = 0;
            int hiddenCount = 0;

            var effect = new LayerShowHideEffect(layerNames, states);

            shownCount += effect.LayersShownCount;
            hiddenCount += effect.LayersHiddenCount;

            effects.Add(effect);

            if (hideOthers)
            {
                var layersToHide = RhinoAppMappings.DocumentLayers.Where(l => !layerNames.Contains(l.Name));
                var hideEffect = new LayerShowHideEffect(layersToHide.Select(l => l.Name).ToList(), false);
                shownCount += hideEffect.LayersShownCount;
                hiddenCount += hideEffect.LayersHiddenCount;
                effects.Add(hideEffect);
            }

            this.Message = $"Showing {shownCount} , Hiding {hiddenCount}";

            DA.SetDataList(0, effects);


        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.eff_01;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("FB22DAAA-B288-4EC3-BB73-1A3435148C62");
    }
}