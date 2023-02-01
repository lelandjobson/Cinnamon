using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Models
{
    public class OrientableMovementCurve
    {
        public bool IsValid => A.IsValid && (B == null || B.IsValid) && (C == null || C.IsValid);

        public Curve A { get; private set; }

        public Curve B { get; private set; }

        public Curve C { get; private set; }

        public OrientableMovementCurve(params Curve[] movementCurves)
        {
            if(movementCurves.Length > 0)
            {
                A = movementCurves[0];
            }
            if(movementCurves.Length > 1)
            {
                B = movementCurves[1];
            }
            if(movementCurves.Length > 2)
            {
                C = movementCurves[2];
            }
        }

        public OrientableMovementCurve(List<Curve> movementCurves)
        {
            if(movementCurves[0] != null) { A = movementCurves[0];}
            if(movementCurves[1] != null) { B = movementCurves[1];}
            if(movementCurves[2] != null) { C = movementCurves[2]; }
        }

        public OrientableMovementCurve(List<ObjectOrientationState> states, bool interpolatedCurve = false, int degree = 3)
        {
            switch (states[0].Kind)
            {

                case OrientationStateKind.OnePoint:
                    A = interpolatedCurve ? Curve.CreateInterpolatedCurve(states.Select(s => s.A), degree) : new PolylineCurve(states.Select(s => s.A));
                    break;

                case OrientationStateKind.ThreePoint:
                    A = interpolatedCurve ? Curve.CreateInterpolatedCurve(states.Select(s => s.A), degree) : new PolylineCurve(states.Select(s => s.A));
                    B = interpolatedCurve ? Curve.CreateInterpolatedCurve(states.Select(s => s.B), degree) : new PolylineCurve(states.Select(s => s.B));
                    C = interpolatedCurve ? Curve.CreateInterpolatedCurve(states.Select(s => s.C), degree) : new PolylineCurve(states.Select(s => s.C));
                    break;
            }
        }

        public IEnumerable<Curve> Iterate()
        {
            if(A != null) { yield return A; }   
            if(B != null) { yield return B; }
            if(C != null) { yield return C; }
            yield break;
        }
    }
}