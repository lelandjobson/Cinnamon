﻿using System;
using System.Collections.Generic;

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

        static List<double> _easeOut = new List<double>()
        {
            0,
            0.27104,
            0.368603,
            0.437109,
            0.490757,
            0.535023,
            0.572704,
            0.605455,
            0.634351,
            0.660134,
            0.683343,
            0.704384,
            0.723568,
            0.741146,
            0.757317,
            0.772245,
            0.786068,
            0.798901,
            0.810841,
            0.821975,
            0.832373,
            0.842101,
            0.851215,
            0.859763,
            0.86779,
            0.875335,
            0.882433,
            0.889116,
            0.895414,
            0.901351,
            0.906951,
            0.912237,
            0.917228,
            0.921942,
            0.926396,
            0.930606,
            0.934585,
            0.938348,
            0.941906,
            0.945271,
            0.948454,
            0.951464,
            0.954311,
            0.957004,
            0.95955,
            0.961959,
            0.964235,
            0.966388,
            0.968423,
            0.970345,
            0.972161,
            0.973877,
            0.975497,
            0.977026,
            0.978469,
            0.97983,
            0.981113,
            0.982323,
            0.983463,
            0.984536,
            0.985546,
            0.986496,
            0.98739,
            0.988231,
            0.98902,
            0.989761,
            0.990457,
            0.991109,
            0.991721,
            0.992295,
            0.992832,
            0.993335,
            0.993807,
            0.994247,
            0.99466,
            0.995046,
            0.995407,
            0.995745,
            0.996062,
            0.996358,
            0.996636,
            0.996896,
            0.997141,
            0.997372,
            0.997589,
            0.997794,
            0.997988,
            0.998172,
            0.998348,
            0.998517,
            0.998679,
            0.998836,
            0.998988,
            0.999136,
            0.999282,
            0.999426,
            0.999569,
            0.999712,
            0.999855,
            1,

        };

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
                case AnimationCurve.Null:
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

        static double GetInterp(double input, List<double> output)
        {
            double mult = input * 100;
            int flor = Convert.ToInt32(Math.Ceiling(mult));
            int ceil = Convert.ToInt32(Math.Floor(mult));
            if(flor == ceil) { return output[flor]; }
            return ((output[ceil] - output[flor]) * (mult % 1)) + output[flor];
        }
    }
}