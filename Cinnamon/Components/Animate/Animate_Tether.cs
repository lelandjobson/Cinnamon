using Cinnamon.Components.Capture.Managers;
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
    public class Animate_Tether : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Animate_Tether()
          : base("Tether", "Tether",
            "Tethers object orientations to a moving object in your moview",
            "Cinnamon", "2_Animate")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Leader", "Leader", "The moving object which to follow", GH_ParamAccess.item);
            pManager.AddGenericParameter("Followers", "Followers", "The GUIDS of follower objects whose orientations match the leader", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Break", "Break", "Breaks all tethers on the leader objecs or only breaks provided follower objects", GH_ParamAccess.item, false);
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
            //this.Message = "WARNING! \n In case of state changes, \n  Use a COPY of your model file, \n not the original.";
            this.Message = "";
            int Capture = -1;

            #region Handle ObjectId / Object Overloading
            object theObject = null;
            List<object> followers = new List<object>();
            DA.GetData(0, ref theObject);
            DA.GetDataList(1, followers);
            Guid leaderId = GetGuid(theObject);
            List<Guid> followerIds = new List<Guid>();
            foreach(var followerObj in followers)
            {
                try
                {
                    followerIds.Add(GetGuid(followerObj));
                }
                catch { continue; }
            }

            bool breakTether = false;
            DA.GetData(2, ref breakTether);
            if (breakTether)
            {
                if (TetherManager.Tethers.ContainsKey(leaderId)) { return; }
                if(followerIds.Count == 0)
                {
                    TetherManager.Tethers.Remove(leaderId);
                }
                else
                {
                    TetherManager.Tethers[leaderId] = TetherManager.Tethers[leaderId].Where(f => !followerIds.Contains(f)).ToList();
                }
            }
            else
            {
                if (TetherManager.Tethers.ContainsKey(leaderId))
                {
                    TetherManager.Tethers[leaderId] = followerIds.ToList();
                }
                else
                {
                    TetherManager.Tethers.Add(leaderId, followerIds.ToList());
                }
                if (followerIds.Count > 0)
                {
                    this.Message = "Ensure that all followers are GUIDs!";
                }
            }


            #endregion
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
        public override Guid ComponentGuid => new Guid("AB22DABC-B288-4EC3-BB73-1A3435211C62");

        private Guid GetGuid(object o)
        {
            string objectId = string.Empty;
            if (o != null)
            {
                if (o is string st)
                {
                    objectId = st;
                }
                else if (o is GH_Guid id)
                {
                    objectId = id.Value.ToString();
                }
                else if (o is Rhino.DocObjects.RhinoObject ro)
                {
                    objectId = ro.Id.ToString();
                }
                else
                {
                    throw new Exception($"Unhandled type {o.GetType()}");
                }
            }
            return Guid.Parse(objectId);
        }
    }
}