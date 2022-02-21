using Grasshopper.Kernel.Parameters;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinnamon
{
    internal static class Extensions
    {
        internal static Point3d GetCriticalPoint(this RhinoObject ro)
        {
            if(ro is ClippingPlaneObject co)
            {
                return co.ToBBPoint();
            }
            else if(ro.Geometry is Point p) { return p.Location; }
            else if (ro.Geometry != null)
            {
                return ro.ToBBPoint();
            }
            else
            {
                throw new Exception("provided element has no geometry");
            }
        }

        internal static Point3d ToBBPoint(this ClippingPlaneObject co) => co.Geometry?.GetBoundingBox(true).Center ?? Point3d.Unset;

        internal static RhinoObject ToDocumentObject(this Guid id) => Rhino.RhinoDoc.ActiveDoc.Objects.Find(id);

        internal static Point3d ToBBPoint(this RhinoObject ro) => ro.Geometry?.GetBoundingBox(true).Center ?? Point3d.Unset;

        internal static Vector3d GetCaptureTransform(this RhinoObject ro, Point3d toPoint) => toPoint - ro.ToBBPoint();

        internal static int ToInt32(this double d) => System.Convert.ToInt32(d);

        internal static double GetTransparency(this Layer l)
        {
            if (l == null) { return double.NaN; }
            var objs = RhinoAppMappings.ObjectsOnLayer(l);
            if(objs.Length == 0) { return double.NaN;}
            foreach(var o in objs)
            {
                var mat = o.GetMaterial(false);
                if(mat != null) { return mat.Transparency; }
            }
            return double.NaN;
        }

        internal static void SetTransparency(this Layer l, double trans)
        {
            if(l == null) { return; }
            var objs = RhinoAppMappings.ObjectsOnLayer(l);
            HashSet<Material> mats = 
                objs.Select(o => o.GetMaterial(false))
                    .Where(m => m != null && m != default(Material) && !m.IsDocumentControlled)
                    .ToHashSet();
            foreach(var m in mats)
            {
                m.Transparency = trans;
            }
        }

        internal static void AddNamedValuesForEnum(this Param_Integer pint, Type enumType)
        {
            var names = Enum.GetNames(enumType);
            for(int i = 0; i < names.Length; i++)
            {
                pint.AddNamedValue(names[i], i);
            }
        }

        internal static Vector3d VectorTo(this Point3d start, Point3d end)
        {
            return new Vector3d(end.X - start.X, end.Y - start.Y, end.Z - start.Z); 
        }

        internal static void Fill<T>(this T[] arr, T value = null) where T : class
        {
            for(int i =0; i < arr.Length; i++)
            {
                arr[i] = null;
            }
        }

        internal static double Remap(this int from, int fromMin, int fromMax, double toMin, double toMax)
        {
            var fromAbs = from - fromMin;
            var fromMaxAbs = fromMax - fromMin;

            var normal = (fromAbs * 1.0) / (fromMaxAbs * 1.0);

            var toMaxAbs = toMax - toMin;
            var toAbs = toMaxAbs * normal;

            var to = toAbs + toMin;

            return to;
        }

        /// <summary>
        /// Given a percentage, gets the value percentage of a range
        /// </summary>
        /// <param name="p"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        internal static double PercToValue(this double p, double start, double end)
        {
            return ((end - start) * p) + start;
        }

        internal static Point3d PercToValue(this double p, Point3d start, Point3d end)
        {
            return new Point3d(
                p.PercToValue(start.X, end.X),
                p.PercToValue(start.Y, end.Y),
                p.PercToValue(start.Z, end.Z)
                );
        }
    }
}
