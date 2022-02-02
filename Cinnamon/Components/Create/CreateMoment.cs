using Cinnamon.Models;
using Cinnamon.Models.Effects;
using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Create
{
    public class CreateMoment : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public CreateMoment()
          : base("CreateMoment", "CreateMoment",
            "Creates a moment from effects",
            "Cinnamon", "1_Effects")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Effects", "Effects", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Style", "Style", "", GH_ParamAccess.item);
            pManager.AddGenericParameter("TimeRange", "TimeRange", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Moment", "Moment", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<IEffect> effects = new List<IEffect>();
            string curve = String.Empty;
            TimelineTime range = TimelineTime.Empty; 

            if (!DA.GetDataList<IEffect>(0, effects)) { return; }
            if (!DA.GetData<string>(1, ref curve)){ }
            if (!DA.GetData<TimelineTime>(2, ref range)){ return;  }

            if(effects.Count == 0) { return; }

            AnimationCurve c = AnimationCurve.Linear;
            if (!string.IsNullOrEmpty(curve)) { AnimationCurve.TryParse(curve, out c); }

            var mom = new Moment(range, c, effects);

            DA.SetData(0, mom);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.moment;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("FA72DA7D-A788-4EC3-BB73-1A3435148C61");
    }
} 