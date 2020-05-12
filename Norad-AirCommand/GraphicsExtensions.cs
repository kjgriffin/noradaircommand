using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Norad_AirCommand
{
    static class GraphicsExtensions
    {

        /// <summary>
        /// Rotates a bitmap to the specific angle
        /// </summary>
        /// <param name="b">The bitmap to rotate</param>
        /// <param name="angle">The angle to rotate to</param>
        /// <returns>The rotated bitmap</returns>
        public static Bitmap Rotate (this Bitmap b, float angle)
        {
            angle += 90; //account for conversion from cartesian coordinates to CAST
            Bitmap returnbitmap = new Bitmap(b.Width, b.Height);
            Graphics g = Graphics.FromImage(returnbitmap);
            g.TranslateTransform(b.Width / 2, b.Height / 2);
            g.RotateTransform(angle);
            g.TranslateTransform(-b.Width / 2, -b.Height / 2);
            g.DrawImage(b, new Point(0, 0));
            return returnbitmap;
        }



    }
}
