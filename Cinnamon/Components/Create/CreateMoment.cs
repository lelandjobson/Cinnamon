using Cinnamon.Models;
using Cinnamon.Models.Effects;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
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
            "Cinnamon", "2_Build")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Effects", "Effects", "Effects which will run at this moment", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Style", "Style", "The style of effect interpolation. EaseInOut by default.", GH_ParamAccess.item, 3);
            pManager.AddGenericParameter("TimeRange", "TimeRange", "The range of time where this effect will be run in the timeline", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[0].DataMapping = GH_DataMapping.Flatten;
            var pint = pManager[1] as Param_Integer;
            pint.AddNamedValuesForEnum(typeof(AnimationCurve));
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
            this.Message = "";

            List<IEffect> effects = new List<IEffect>();
            int curve = 0;
            TimelineTime range = TimelineTime.Empty;


            DA.GetDataList<IEffect>(0, effects);
            if (!DA.GetData<int>(1, ref curve)){ }
            if (!DA.GetData<TimelineTime>(2, ref range)){ return;  }

            if(effects.Count == 0) { return; }

            AnimationCurve c = AnimationCurve.Linear;
            c = (AnimationCurve)curve;

            var mom = new Moment(range, c, effects);

            this.Message = $"{mom.Time.Duration} second moment starting at {mom.Time.Start}.";

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