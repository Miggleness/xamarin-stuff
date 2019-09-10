using System;
namespace MarvelCards
{
    public static class Helpers
    {
        public static double BoundedMinMax(double value, double min, double max)
        {
            return (value < min)
                ? min
                : (value > max)
                    ? max
                    : value;
        }
        
    }
}
