using Rhino.Display;
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

        public static string ForceView
        {
            get => _forceView;
            set
            {
                if(_forceView != value)
                {
                    _forceView = value;
                }
            }
        }
        private static string _forceView = String.Empty;

        public static RhinoViewport ActiveViewport => ActiveView.ActiveViewport;

        public static RhinoView ActiveView
        {
            get
            {
                if(_forceView != null)
                {
                    return Rhino.RhinoDoc.ActiveDoc.Views.FirstOrDefault(v => v.ActiveViewport.Name.Equals(_forceView)) ??
                       //Rhino.RhinoDoc.ActiveDoc.NamedViews.FirstOrDefault(v => v.Name.Equals(_forceView, StringComparison.OrdinalIgnoreCase)) ??
                           Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;
                }
                else
                {
                    return Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;
                }
            }
        }

    }
}
