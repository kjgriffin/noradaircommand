using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Norad_AirCommand
{

    enum MissileType
    {
        srir,
        srr,
        mrr,
    }

    class Missile : Entity
    {
        public MissileType Type;
        public float Damage;
        public float lockID;

        public float Orientation;
        public float speed;
        public PointF targetpos;
        public bool Active;


        public Missile(MissileType type)
        {
            Type = type;
            Damage = 10;
            Active = true;
        }

        public override void Draw(Graphics g, UniverseDisplaySettings us)
        {
            g.DrawImage(Properties.Resources.missile.Rotate(Orientation), new RectangleF(Location.CenterPoint(new Size(20, 00)), new Size(20, 20)));

        }

        public override void Update(ref List<Entity> EntityList, MapComponent map, ref MapComponent tempmap)
        {
            base.Update(ref EntityList, map, ref tempmap);
        }



    }
}
