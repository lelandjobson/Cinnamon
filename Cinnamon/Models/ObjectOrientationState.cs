using Rhino.Geometry;
using System;

namespace Cinnamon.Models
{
    public abstract class ObjectOrientationState
    {
        public bool IsValid => Kind != OrientationStateKind.Null;

        public readonly Guid Id;
        public Point3d A { get => a; internal set { a = value; RecomputePlane(); } }
        public Point3d B { get => b; internal set { b = value; RecomputePlane(); } }
        public Point3d C { get => c; internal set { c = value; RecomputePlane(); } }

        private Point3d a;
        private Point3d b;
        private Point3d c;

        public readonly OrientationStateKind Kind = OrientationStateKind.Null;

        public Plane Plane { get; internal set; }

        public ObjectOrientationState(Guid id, params Point3d[] points)
        {
            this.Id = id;
            if (points.Length > 0) { A = points[0]; Kind = OrientationStateKind.OnePoint; }
            if (points.Length > 1) { B = points[1]; Kind = OrientationStateKind.TwoPoint; }
            if (points.Length > 2) { C = points[2]; Kind = OrientationStateKind.ThreePoint; }

            RecomputePlane();
        }

        public abstract void Apply();

        void RecomputePlane()
        {
            switch (Kind)
            {
                case OrientationStateKind.OnePoint:
                    Plane = new Plane(A, Vector3d.XAxis, Vector3d.YAxis);
                    break;
                case OrientationStateKind.TwoPoint:
                    Plane = new Plane(A, B, B.Rotate90Around(A));
                    break;
                case OrientationStateKind.ThreePoint:
                    Plane = new Plane(A, B, C);
                    break;
                default:
                    Plane = Plane.WorldXY;
                    break;
            }
        }

        public static bool TryCreate(Guid id, out ObjectOrientationState state) => ObjectStateExtensions.TryGetOrientationState(id.ToDocumentObject(), out state); 

    }
}