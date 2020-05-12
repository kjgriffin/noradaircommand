using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Norad_AirCommand
{
    class Entity
    {

        internal PointF Location;

        public virtual void Update(ref List<Entity>EntityList, MapComponent map, ref MapComponent tempmap)
        {

        }

        public virtual void Draw(Graphics g, UniverseDisplaySettings us)
        {

        }

        public Entity()
        {

        }
    }
}
