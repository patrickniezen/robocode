using System;

namespace PN.Helpers
{
    public static class MathHelper
    {
        public static double CalculateDistanceBetweenCoordinates(double x1, double y1, double x2, double y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;

            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        }
    }
}