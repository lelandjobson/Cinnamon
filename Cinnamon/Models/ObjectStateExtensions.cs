using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using System;
using System.Collections.Generic;

namespace Cinnamon.Models
{
    public static class ObjectStateExtensions
    {
        public static Point3d Rotate90Around(this Point3d toRotate, Point3d axis)
        {
            var copy = toRotate;
            copy.Transform(Transform.Rotation(Math.PI / 2, axis));
            return copy;
        }

        public static bool TryGetOrientationState(this RhinoObject ro, out ObjectOrientationState orientation)
        {
            // TODO - FINISH CASE LABELS FOR AS MANY OBJECT TYPES AS POSSIBLE.
            // TODO - Implement new layer storage mechanism
            orientation = null;
            switch (ro.Geometry.ObjectType)
            {
                case ObjectType.Point:
                    if (ro.Geometry is Point p)
                    {
                        orientation = new SinglePointObjectOrientationState(ro.Id, p.Location);
                        return true;
                    } 
                    break;
                case ObjectType.Curve:
                    if (ro.Geometry is Curve c)
                    {
                        orientation = new ThreePointObjectOrientationState(ro.Id, GetOrientationPoints(c).ToArray());
                        return true;
                    }
                    break;
                case ObjectType.None:
                    return false;
                case ObjectType.PointSet:
                    if (ro.Geometry is PointCloud pcs)
                    {
                       
                    }
                    break;
                case ObjectType.Surface:
                    if (ro.Geometry is Surface s)
                    {
                        orientation = new ThreePointObjectOrientationState(ro.Id, GetOrientationPoints(s).ToArray());
                        return true;
                    }
                    break;
                case ObjectType.Brep:
                    if (ro.Geometry is Brep b)
                    {
                        orientation = new ThreePointObjectOrientationState(ro.Id, GetOrientationPoints(b.Curves3D.ToList()).ToArray());
                        return true;
                    }
                    break;
                case ObjectType.Mesh:
                    break;
                case ObjectType.Light:
                    break;
                case ObjectType.Annotation:
                    break;
                case ObjectType.InstanceDefinition:
                    break;
                case ObjectType.InstanceReference:
                    break;
                case ObjectType.TextDot:
                    break;
                case ObjectType.Grip:
                    break;
                case ObjectType.Detail:
                    break;
                case ObjectType.Hatch:
                    break;
                case ObjectType.MorphControl:
                    break;
                case ObjectType.SubD:
                    break;
                case ObjectType.BrepLoop:
                    break;
                case ObjectType.PolysrfFilter:
                    break;
                case ObjectType.EdgeFilter:
                    break;
                case ObjectType.PolyedgeFilter:
                    break;
                case ObjectType.MeshVertex:
                    break;
                case ObjectType.MeshEdge:
                    break;
                case ObjectType.MeshFace:
                    break;
                case ObjectType.Cage:
                    break;
                case ObjectType.Phantom:
                    break;
                case ObjectType.ClipPlane:
                    if(ro.Geometry is ClippingPlaneSurface clip)
                    {

                    }
                    break;
                case ObjectType.Extrusion:
                    if(ro.Geometry is Extrusion e)
                    {
                        orientation = new ThreePointObjectOrientationState(ro.Id, GetOrientationPoints(e).ToArray());
                        return true;
                    }
                    break;
                case ObjectType.AnyObject:
                    return false;
            }
            return false;
        }

        public static List<Curve> ToList(this BrepCurveList curveList)
        {
            List<Curve> output = new List<Curve>();
            for(int i = 0; i < curveList.Count; i++)
            {
                if(curveList[i] == null) { continue; }
                output.Add(curveList[i]);
            }
            return output;
        }

        const double VectorAngleTolerance = Math.PI/36;

        public static Point3d[] ToArray(this (Point3d,Point3d,Point3d) tuple)
        {
            return new Point3d[] { tuple.Item1, tuple.Item2, tuple.Item3 };
        }

        public static (Point3d a, Point3d b, Point3d c) GetOrientationPoints(this Curve singleCurve)
        {
            // Use z rotation to get third point
            var pts = GetKeyPoints(singleCurve);
            Point3d c = pts.a;
            Transform.Rotation(Math.PI / 2, Vector3d.ZAxis, c);
            return (pts.a, pts.b, c);
        }

        public static (Point3d a, Point3d b,Point3d c) GetOrientationPoints(this Surface s)
        {
            var uDomain = s.Domain(0);
            var vDomain = s.Domain(1);
            Plane p = Plane.WorldXY;
            if (s.FrameAt(uDomain.Min, vDomain.Min, out p) ||
                s.FrameAt(uDomain.Min, vDomain.Max, out p) ||
                s.FrameAt(uDomain.Max, vDomain.Max, out p) ||
                s.FrameAt(uDomain.Max, vDomain.Min, out p)
                )
            {
                return GetOrientationPoints(p);
            }
            return
                (s.PointAt(0, 0), s.PointAt(1, 0), s.PointAt(0, 1));
        }

        public static (Point3d a, Point3d b, Point3d c) GetOrientationPoints(this Plane p)
        {
            Point3d a = p.Origin;
            Point3d b = p.Origin + (p.XAxis * 100);
            Point3d c = p.Origin + (p.YAxis * 100);
            return (a,b,c);
        }

        public static (Point3d a,Point3d b,Point3d c) GetOrientationPoints(this List<Curve> curveSelection)
        {
            if(curveSelection.Count == 1)
            {
                return GetOrientationPoints(curveSelection[0]);
            }
            // Get two curves which are different enough from eachother
            Curve startCurve = curveSelection[0];
            var startCurvePts = startCurve.GetKeyPoints();
            Vector3d startCurveVec = curveSelection[0].GetKeyVector();
            Curve endCurve = curveSelection[1];
            for(int i =1; i < curveSelection.Count; i++)
            {
                var pts = curveSelection[i].GetKeyPoints();
                // Don't use points too close to the start curve's points
                if(pts.b.AlmostEquals(startCurvePts.a) || pts.b.AlmostEquals(startCurvePts.a)) { continue; }

                var vec = curveSelection[i].GetKeyVector();
                if(Vector3d.VectorAngle(startCurveVec, vec) < VectorAngleTolerance) { continue; }

                endCurve = curveSelection[i];
                break;
            }

            var endCurvePts = endCurve.GetKeyPoints();
            return (startCurvePts.a, startCurvePts.b, endCurvePts.b);
        }

        public static Vector3d GetKeyVector(this Curve c)
        {
            var pts = GetKeyPoints(c);
            return pts.b - pts.a;
        }

        public static (Point3d a, Point3d b) GetKeyPoints(this Curve c)
        {
            Point3d a = c.PointAtStart;
            Point3d b = c.PointAtEnd;
            if (c.IsClosed)
            {
                b = c.PointAtNormalizedLength(0.5);
            }
            return (a, b);
        }

        const double PointLocationTolerance = 0.001;
        public static bool AlmostEquals(this Point3d a, Point3d b)
        {
            if (a.X-b.X <= PointLocationTolerance && 
                a.Y-b.Y <= PointLocationTolerance && 
                a.Z-b.Z <= PointLocationTolerance)
            {
                return true;
            }
            return false;
        }
    }
}