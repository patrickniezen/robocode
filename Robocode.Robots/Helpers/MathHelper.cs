using System;

namespace PN.Helpers
{
    public static class MathHelper
    {
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}