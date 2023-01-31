using Rhino.Display;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinnamon
{
    internal class RhinoAppMappings
    {
        //public static Guid Id_ActiveViewportCamera = new Guid("81B69A70-7F24-4D65-9F50-83B14A1570F8");

        public static LayerTable DocumentLayers => Rhino.RhinoDoc.ActiveDoc.Layers;

        public static RhinoObject[] ObjectsOnLayer(Layer l) =>
            Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(l);
        


        public static RhinoViewport ActiveViewport => ActiveView.ActiveViewport;

        public static RhinoView ActiveView
        {
            get
            {
                return Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;
            }
            set
            {
                Rhino.RhinoDoc.ActiveDoc.Views.ActiveView = value;
            }
        }

    }
}
