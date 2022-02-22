using Cinnamon.Models;
using Cinnamon.Models.Effects;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace Cinnamon.Components.Create
{
    public class Animate_MoveObjectOnCurveEffect : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Animate_MoveObjectOnCurveEffect()
          : base("MoveObjectOnCurve", "MoveObjectOnCurve",
            "Moves an object along a curve",
            "Cinnamon", "1_Animate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "Object", "The object.", GH_ParamAccess.item);
            pManager.AddCurveParameter("LocationCurve", "Loc", "The curve which will guide its location.", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Effect", "Effect", "", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
                else if (theObject is RhinoObject ro)
                {
                    objectId = ro.Id.ToString();
                }
                else
                {
                    throw new Exception($"Unhandled type {theObject.GetType()}");
                }
            }
            #endregion


            Curve cur = null;
            if (!DA.GetData<Curve>(1, ref cur)){ return; }
            if(!Guid.TryParse(objectId, out var oidGuid)) { return; }


            var eff = new MoveObjectOnCurveEffect(oidGuid, cur);

            DA.SetData(0, eff);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.eff_05;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("FA72AAAA-B288-4EC3-BB73-1A3435148C61");
    }
} 