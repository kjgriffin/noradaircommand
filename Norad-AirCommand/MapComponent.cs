using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Norad_AirCommand
{
    class MapComponent
    {
        //list of heat signatures
        public List<HeatSignature> HeatMap = new List<HeatSignature>();
        //list of passive and active radar signatures
        public List<RadarSignature> PassiveRadarMap = new List<RadarSignature>();
        public List<RadarSignature> ActiveRadarMap = new List<RadarSignature>();
        //list of kill locations
        public List<KillZone> KillZoneMap = new List<KillZone>();

        public MapComponent()
        {

        }

    }

    class HeatSignature
    {
        //actual temperature degrees celcius
        public float Temperature;
        //trackingtempid (unique)
        public float trackingID;
        public string Team;
        public PointF Location;

        public HeatSignature(PointF pos, float temp, string team)
        {
            Temperature = temp;
            Random r = new Random();
            trackingID = r.Next();
            Team = team;
            Location = pos;
        }

        public bool WillDecay()
        {
            if (Temperature - 50 > 1500)
                return true;
            else return false;
        }

        public HeatSignature DecayedHeadSignature(HeatSignature h)
        {
            h.Temperature -= 50;
            return h;
        }

    }

    class RadarSignature
    {
        public float Strength;
        public float trackingID;
        public string Team;
        public PointF Location;

        public RadarSignature(PointF pos, float strength, float id, string team)
        {
            Strength = strength;
            Team = team;
            trackingID = id;
            Location = pos;
        }

    }
    
    class KillZone
    {

        public PointF Epicenter;
        public float Radius;
        public float Damage;


        public KillZone(PointF ec, float radius, float damage)
        {
            Damage = damage;
            Radius = radius;
            Epicenter = ec;
        }

        public bool ContainsPoint(PointF pos)
        {
            return pos.IsPointInCircle(Epicenter, Radius);
        }

        public float calculate_Damage(PointF pos)
        {
            //damage decreses by 1/2 per m radius unit from epicenter
            if (ContainsPoint(pos))
            {
                float distance = (float)pos.getDistance(Epicenter);
                if (distance > 0)
                {
                    return Damage / (2 * distance);
                 }
                else
                    return Damage;
            }
            else
                return 0;
        }

    }



}
