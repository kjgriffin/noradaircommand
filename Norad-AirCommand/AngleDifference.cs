using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Norad_AirCommand
{

    enum RotationDirection
    {
        Clockwise,
        CounterClockwise,
    }

    /// <summary>
    /// A structure that represents the angular distance between 2 vectors, and the direction of rotation that is shorter, with refrence to vector a
    /// </summary>
    struct AngleDifference
    {
        /// <summary>
        /// The difference between the ange
        /// </summary>
        public float Difference;
        /// <summary>
        /// True means rotation direction is CounterClockWise
        /// </summary>
        public bool CCW;
        /// <summary>
        /// The direction of roation
        /// </summary>
        public RotationDirection RD;

        public AngleDifference(float value, bool ccw)
        {
            CCW = ccw;
            if (ccw)
                RD = RotationDirection.CounterClockwise;
            else
                RD = RotationDirection.Clockwise;
            Difference = value;
        }

    }
}
