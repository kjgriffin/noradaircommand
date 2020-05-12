using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Norad_AirCommand
{
    static class MathExtensions
    {

        /// <summary>
        /// Calculates a new point based on the current point, distance and angle
        /// </summary>
        /// <param name="pos">The current point</param>
        /// <param name="angle">The angle</param>
        /// <param name="distance">The distance from the original point</param>
        /// <returns>The new point</returns>
        public static PointF CalculateNewPoint(this PointF pos, float angle, float distance)
        {

            //create new local reference point (0,0)
            PointF localpos = new PointF(0, 0);
            //use unit circle and trig to calculate pos using angle and distance
            localpos.X = (float)Math.Cos(angle.DegreestoRad()) * distance;
            localpos.Y = (float)Math.Sin(angle.DegreestoRad())  * distance;
            //add current point.x and current point.y to original pos
            pos.X += localpos.X;
            pos.Y += localpos.Y;
            return pos;
        }
        
        /// <summary>
        /// Return a new point that is centered on half the size
        /// </summary>
        /// <param name="pos">The point to center</param>
        /// <param name="size">The size to center on</param>
        /// <returns>The centered point</returns>
        public static PointF CenterPoint(this PointF pos, SizeF size)
        {
            return (new PointF(pos.X - (size.Width / 2),  pos.Y - (size.Height /2))); 
        }

        /// <summary>
        /// Return an list of points defining an isosceles triangle
        /// </summary>
        /// <param name="pos">The base location</param>
        /// <param name="range">The height of the triangle</param>
        /// <param name="angle">The rotation angle of the triangle</param>
        /// <param name="halfangle">Half the angle between the 2 longer sides</param>
        /// <returns>The isosceles triangle</returns>
        public static PointF[] CreateHitTriangle(this PointF pos, float range, float angle, float halfangle)
        {
            PointF[] pts = new PointF[3];
            pts[0] = pos;
            pts[1] = pos.CalculateNewPoint(angle.ContinuousAngleAddition(halfangle), range);
            pts[2] = pos.CalculateNewPoint(angle.ContinuousAngleSubtraction(halfangle), range);
            return pts;
        }

        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInPolygon(this PointF[] polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        /// <summary>
        /// Makes sure that 0 <= (angle - angle) <= 360
        /// </summary>
        /// <param name="angle">The angle that is being subtracted from</param>
        /// <param name="angle1">The ammount in degrees to subtract from the angle</param>
        /// <returns>The angle</returns>
        public static float ContinuousAngleSubtraction(this float angle, float angle1)
        {
            angle = angle - angle1;
            while (angle < 0)
            {
                angle += 360;
            }
            return angle;
        }

        /// <summary>
        /// Makes sure that 0 <= (angle + angle) <= 360
        /// </summary>
        /// <param name="angle">The angle that is being added to</param>
        /// <param name="angle1">The ammount in degrees to add to the angle</param>
        /// <returns>The angle</returns>
        public static float ContinuousAngleAddition(this float angle, float angle1)
        {
            angle += angle1;
            while (angle > 360)
            {
                angle -= 360;
            }
            return angle;
        }

        /// <summary>
        /// Returns the difference between the angles
        /// </summary>
        /// <param name="angle">The original angle</param>
        /// <param name="angle1">The angle to compare to</param>
        /// <returns> The angledifference</returns>
        public static AngleDifference GetAngleDifference(this float angle, float angle1)
        {
            //translate angles to angle's local coordiante system
            angle1 = angle1.ContinuousAngleSubtraction(angle);
            //see if angle 1 is less than 180 ccw else cw
            if (angle1 < 180)
                return new AngleDifference(angle1, true);
            else
                return new AngleDifference(360 - angle1, false);
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="angle">The angle in degrees</param>
        /// <returns>The angle in radians</returns>
        public static double DegreestoRad(this float angle)
        {
            return angle * Math.PI / 180;
        }

        /// <summary>
        /// Calculates the angle between 2 points
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>The angle in degrees</returns>
        public static double GetAngle(this PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        /// <summary>
        /// Calculates the angle between 2 points
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <returns>The angle in degrees</returns>
        public static float GetAngleF(this PointF p1, PointF p2)
        {
            float xDiff = p2.X - p1.X;
            float yDiff = p2.Y - p1.Y;
            return (float)(Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI);
        }

        /// <summary>
        /// Gets the slope of a straight line
        /// </summary>
        /// <param name="p1">A point on the line</param>
        /// <param name="p2">A point on the line</param>
        /// <returns>The slope of the line</returns>
        public static float GetSlope(PointF p1, PointF p2)
        {
            try
            {
                return (p2.Y - p1.Y) / (p2.X - p1.X);
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Calculates a "distance" (does not take square root) between the 2 points
        /// </summary>
        /// <param name="p">The first point</param>
        /// <param name="p1">The second point</param>
        /// <returns>The "distance"</returns>
        public static double getLength(this PointF p, PointF p1)
        {
            return Math.Pow(p.X - p1.X, 2) + Math.Pow(p.Y - p1.Y, 2);
        }

        /// <summary>
        /// Calculates the distance between 2 points
        /// </summary>
        /// <param name="p">The first point</param>
        /// <param name="p1">The second point</param>
        /// <returns>The distance</returns>
        public static double getDistance(this PointF p, PointF p1)
        {
            return Math.Sqrt( Math.Pow(p.X - p1.X, 2) + Math.Pow(p.Y - p1.Y, 2));
        }

        /// <summary>
        /// Determines whether a point lies inside a circle
        /// </summary>
        /// <param name="pos">The point</param>
        /// <param name="centre">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <returns></returns>
        public static bool IsPointInCircle(this PointF pos, PointF centre, float radius)
        {
            if (pos.getDistance(centre) <= radius)
                return true;
            else
                return false;
        }
       



    }
}
