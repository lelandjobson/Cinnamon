using Cinnamon.Components.Capture;
using Cinnamon.Models;
using Cinnamon.Models.Effects;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Components.Create
{
    public class AnimateCaptures : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public AnimateCaptures()
          : base("AnimateCaptures ", "AnimateCaptures ",
            "Creates a movement from Captures",
            "Cinnamon", "1_Animate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Object", "Object", "The object. If blank, uses the camera instead.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Captures", "Captures", "Captures to utilize in the animation.", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Interpolate", "Interpolate", "If true, uses .", GH_ParamAccess.item, false);

            pManager[0].Optional = true;
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
            Guid objectIdGuid;
            List<int> Captures = new List<int>();
            bool interp = false;

            #region Handle ObjectId / Object Overloading
            object theObject = null;
            DA.GetData(0, ref theObject);
            string objectId = string.Empty;
            if(theObject != null)
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

            if (!DA.GetDataList(1, Captures)) { return; }
            DA.GetData(2, ref interp);
            if(Captures.Count == 0) { return; }

            List<IEffect> effectsOutput = new List<IEffect>();
            if (string.IsNullOrEmpty(objectId))
            {
                // Camera motion effect
                Curve loc;
                Curve target;
                List<CameraState> states = Captures.Select(o => CaptureManager_Camera.GetCaptureData(o)).ToList();
                if(states.Count < 2) { throw new Exception("2 or more captures are required to produce an effect."); }
                if (!interp)
                {
                    loc = new PolylineCurve(states.Select(s => s.PositionState));
                    target = new PolylineCurve(states.Select(s => s.TargetPositionState));
                    // polylin
                }
                else
                {
                    loc = Curve.CreateInterpolatedCurve(states.Select(s => s.PositionState),3);
                    target = Curve.CreateInterpolatedCurve(states.Select(s => s.TargetPositionState), 3);
                }
                effectsOutput.Add(new MoveCameraOnCurveEffect(loc, target));
                effectsOutput.Add(new FocalLengthEffect(states.Select(s => s.FocalLengthState)));
            }
            else
            {
                if (!Guid.TryParse(objectId, out objectIdGuid)) { return; }
                // Object motion effect
                Curve movement;
                var omg = Document_CaptureManagers.GetOrCreateCaptureManager(objectIdGuid);
                List<ObjectState> states = Captures.Select(o => omg.GetCaptureData(o)).ToList();
                if (states.Count < 2) { throw new Exception("2 or more captures are required to produce an effect."); }
                if (!interp)
                {
                    movement = new PolylineCurve(states.Select(s => s.PositionState));
                }
                else
                {
                    movement = Curve.CreateInterpolatedCurve(states.Select(s => s.PositionState), 3);
                }
                effectsOutput.Add(new MoveObjectWithCapturesEffect(objectIdGuid, movement));
            }

            DA.SetDataList(0, effectsOutput);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.eff_04;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("FDDDDA7D-A788-4EC3-BB73-1A3435148C61");
    }
} 