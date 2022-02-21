using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinnamon.Models
{
    public static class AnimationCurveExtensions
    {
        static List<double> _easeIn = new List<double>()
        {
            0,
            0.000016,
            0.000064,
            0.000146,
            0.000261,
            0.000412,
            0.000599,
            0.000823,
            0.001086,
            0.001387,
            0.00173,
            0.002113,
            0.00254,
            0.003011,
            0.003527,
            0.00409,
            0.004702,
            0.005363,
            0.006077,
            0.006843,
            0.007664,
            0.008542,
            0.009478,
            0.010475,
            0.011535,
            0.01266,
            0.013852,
            0.015113,
            0.016447,
            0.017855,
            0.019341,
            0.020907,
            0.022556,
            0.024292,
            0.026119,
            0.028039,
            0.030057,
            0.032176,
            0.034401,
            0.036736,
            0.039187,
            0.041758,
            0.044454,
            0.047281,
            0.050246,
            0.053354,
            0.056613,
            0.060031,
            0.063614,
            0.067371,
            0.071311,
            0.075445,
            0.079782,
            0.084333,
            0.08911,
            0.094127,
            0.099397,
            0.104935,
            0.110757,
            0.116881,
            0.123327,
            0.130114,
            0.137266,
            0.144806,
            0.152762,
            0.161163,
            0.17004,
            0.179429,
            0.189367,
            0.199896,
            0.211061,
            0.222912,
            0.235503,
            0.248892,
            0.263143,
            0.278325,
            0.294511,
            0.311779,
            0.330209,
            0.349885,
            0.370889,
            0.393303,
            0.417197,
            0.442632,
            0.46965,
            0.498264,
            0.528458,
            0.560177,
            0.593323,
            0.627759,
            0.66331,
            0.699776,
            0.736938,
            0.774577,
            0.812478,
            0.850448,
            0.888315,
            0.925935,
            0.963194,
            1,
        };

        static List<double> _easeOut => __easeOut ?? (__easeOut = _easeIn.OrderBy(c => -c).ToList());
        static List<double> __easeOut;

        static List<double> _easeInOut = new List<double>()
        {
            0,
            0.000088,
            0.000357,
            0.000812,
            0.00146,
            0.002307,
            0.003362,
            0.004631,
            0.006123,
            0.007845,
            0.009807,
            0.012018,
            0.014489,
            0.017229,
            0.020249,
            0.023562,
            0.027181,
            0.031117,
            0.035387,
            0.040004,
            0.044984,
            0.050345,
            0.056105,
            0.062283,
            0.068899,
            0.075976,
            0.083536,
            0.091605,
            0.100208,
            0.109373,
            0.11913,
            0.12951,
            0.140544,
            0.152267,
            0.164713,
            0.177918,
            0.191919,
            0.206751,
            0.222449,
            0.239043,
            0.256563,
            0.275028,
            0.294453,
            0.314838,
            0.336168,
            0.358411,
            0.381512,
            0.405393,
            0.429947,
            0.455044,
            0.480528,
            0.506228,
            0.531958,
            0.557533,
            0.58277,
            0.607506,
            0.631596,
            0.65492,
            0.677385,
            0.698925,
            0.719498,
            0.73908,
            0.757668,
            0.775271,
            0.791907,
            0.807605,
            0.822396,
            0.836315,
            0.849401,
            0.861691,
            0.873223,
            0.884034,
            0.894161,
            0.903638,
            0.912497,
            0.920771,
            0.928489,
            0.935678,
            0.942366,
            0.948577,
            0.954335,
            0.95966,
            0.964574,
            0.969096,
            0.973244,
            0.977035,
            0.980484,
            0.983607,
            0.986417,
            0.988929,
            0.991153,
            0.993103,
            0.994788,
            0.99622,
            0.997409,
            0.998362,
            0.99909,
            0.9996,
            0.999901,
            1
        };


        public static double GetNormalizedValue(this AnimationCurve curve, double value)
        {
            if(value <= 0) { return 0; }
            if(value >= 1) { return 1; }
            switch (curve)
            {
                case AnimationCurve.Linear:
                //case AnimationCurve.Null:
                    return value;
                case AnimationCurve.EaseIn:
                    return GetInterp(value, _easeIn);
                case AnimationCurve.EaseInOut:
                    return GetInterp(value, _easeInOut);
                case AnimationCurve.EaseOut:
                    return GetInterp(value, _easeOut);
                default:
                    throw new Exception("Curve not recognized.");
            }
        }

        public static double GetNormalizedValue(this AnimationCurve curve, double value, double min, double max)
        {
            return ((max - min) * GetNormalizedValue(curve, value)) + min;
        }

        static double GetInterp(double input, List<double> outputRange)
        {
            double mult = input * (outputRange.Count - 1);
            int flor = Convert.ToInt32(Math.Floor(mult));
            int ceil = Convert.ToInt32(Math.Ceiling(mult));
            if(flor == ceil) { return outputRange[flor]; }
            return ((outputRange[ceil] - outputRange[flor]) * input) + outputRange[flor];
        }
    }
}