using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Norad_AirCommand
{
    enum AutoEngageStatus
    {
        NoTarget,
        Scanning,
        Tracking,
        Resolving,
        Locked,
    }

    enum AutoEngageMode
    {
        AutoSelect,
        Infared,
        Radar,
    }

    class AutoEngage
    {

        public bool Enabled;
        public AutoEngageStatus Status;
        public AutoEngageMode Mode;
        public bool PreferShortRange;
        public bool SRIRlock;
        public bool SRRlock;
        public bool MRRlock;

        public AutoEngage()
        {
            Enabled = false;
            Status = AutoEngageStatus.NoTarget;
            Mode = AutoEngageMode.AutoSelect;
            PreferShortRange = true;
        }

        public void engage_Missiles(List<Missile> missiles, Radar radar, PointF location, float orientation, PointF targetpos, List<Entity>EntityList)
        {
            //uses the current targetpos and apropriatly selects a weapon and fires it based on prefrences then adds the current missile as an entity to the map
            switch (Mode)
            {
                case AutoEngageMode.AutoSelect:
                    findclosestzone(missiles, radar, location, targetpos, orientation, EntityList);
                    break;
                case AutoEngageMode.Infared:
                    break;
                case AutoEngageMode.Radar:
                    break;
                default:
                    break;
            }
        }


        public void findclosestzone(List<Missile> missiles, Radar r, PointF location, PointF targetpos, float orientation, List<Entity> EntityList)
        {
            //takes a look at the pos and current location to determine if it is in the radar's zone
            //start with the infared, then to radar
            //if inside that zone then locked = true
            //check short range infared radar
            if (location.CreateHitTriangle(r.SRInfaredRange, orientation, r.SRInfaredSightAngle / 2).IsPointInPolygon(targetpos))
            {
                //is inside short range infared radar
                if (missiles.Any(p => p.Type == MissileType.srir & p.Active == true)) //ensure that the plane has a weapon to use
                {
                    Status = AutoEngageStatus.Locked;
                    //loose missile
                    Missile m = missiles.First(p => p.Type == MissileType.srir);
                    missiles.Remove(m);
                    //give it the data it needs to keep tracking
                    m.Orientation = orientation;
                    m.Location = location;
                    m.lockID = 0;
                    EntityList.Add(m);
                }
                SRIRlock = true;
            }
            else
            {
                SRIRlock = false;
            }

            if (location.CreateHitTriangle(r.SRRadarRange, orientation, r.SRRadarSightAngle / 2).IsPointInPolygon(targetpos))
            {
                //is inside short range infared radar
                if (missiles.Any(p => p.Type == MissileType.srr & p.Active == true)) //ensure that the plane has a weapon to use
                {
                    Status = AutoEngageStatus.Locked;
                    //loose missile
                    //give it the data it needs to keep tracking

                }
                SRRlock = true;
            }
            else
            {
                SRRlock = false;
            }

            if (location.CreateHitTriangle(r.MRRadarRange, orientation, r.MRRadarSightAngle / 2).IsPointInPolygon(targetpos))
            {
                //is inside short range infared radar
                if (missiles.Any(p => p.Type == MissileType.mrr & p.Active == true)) //ensure that the plane has a weapon to use
                {
                    Status = AutoEngageStatus.Locked;
                    //loose missile
                    //give it the data it needs to keep tracking

                }
                MRRlock = true;
            }
            else
            {
                MRRlock = false;
            }


        }
        


    }

}
