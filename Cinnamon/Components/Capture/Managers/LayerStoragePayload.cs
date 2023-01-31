using Rhino.Geometry;
using System.Collections.Generic;


namespace Cinnamon.Components.Capture.Managers
{
    public class LayerStoragePayload
    {
        public readonly Dictionary<string, Point3d> Points;

        public LayerStoragePayload(Dictionary<string, Point3d> points)
        {
            Points = points;
        }
    }
}
