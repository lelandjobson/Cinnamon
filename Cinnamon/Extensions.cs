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
        internal static int ToInt32(this double d) => System.Convert.ToInt32(d);


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

        internal static double Remap(this int value, int fromMin, int fromMax, double toMin = 0, double toMax = 1.0)
        {
            double perc = (value * 1.0 / (fromMax - fromMin));
            return (perc * (toMax - toMin)) + toMin;
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
    }
}
