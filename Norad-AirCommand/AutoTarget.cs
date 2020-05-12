using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Norad_AirCommand
{
    enum TargetMode
    {
        Closest,
        Furthest,
        Guard,
        GuardToPursue,
        Return,
    }


    class AutoTarget
    {
        /// <summary>
        /// Will automatically select destination if enabled
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// The targeting mode the autopilot is using
        /// </summary>
        public TargetMode Mode;

        public bool GTFOmode;
        public bool ConserveFuelmode;
        private float fuelcounter;
        public float minchasedistance;
        public float maxfueluse;
        private PointF retpoint;
        public PointF GuardPoint;
        public float GuardAreaRadius;
        public bool GuardClosest;
        public bool ReturnToGuard;
        public float targetPassiveSignalStrength;
        public AutoEngageStatus status;


        //select destination using map
        public PointF chooseDestination(PointF pos, MapComponent map, PointF destination, string team, float range, float throttle)
        {
            if (Enabled)
            {
                if (ConserveFuelmode)
                {
                    //check current fuel consumption, if over max, enable retpoint target
                    if (fuelcounter + throttle / 1000 > maxfueluse)
                    {
                        if (!ReturnToGuard)
                            Mode = TargetMode.Return;
                        else
                            Mode = TargetMode.GuardToPursue;
                        fuelcounter = 0; //reset fuel counter
                    }
                    if (Mode != TargetMode.Return)
                    {
                        //update fuel counter
                        fuelcounter += throttle / 1000;
                    }
                }
                switch (Mode)
                {
                    case TargetMode.Closest:
                        //find the closest enemy
                        return find_ClosestTarget(pos, map, destination, team, range);
                    case TargetMode.Furthest:
                        return find_FurthestTarget(pos, map, destination, team, range);
                    case TargetMode.Guard:
                        return guard_Scan(pos, map, team, range, false);
                    case TargetMode.GuardToPursue:
                        return guard_Scan(pos, map, team, range, true);
                    case TargetMode.Return:
                        return retpoint;
                    default:
                        return destination;
                }
            }
            else
            {
                retpoint = pos; //constantly update the return to point, so that once a chase starts it knows where it was before it started
                return destination;
            }
        }


        public float maintainThrottle(PointF pos, PointF destination, float throttle)
        {
            float t = throttle;
            if (Enabled && Mode != TargetMode.Return)
            {
                //check if target is withing chase zone, or if gtfo or fuel conservation is required
                if (pos.getDistance(destination) > minchasedistance)
                {
                    //increae throttle if not max already
                    if (t < 90)
                        t += 10;
                    else
                        t = 100;
                }
                else
                {
                    //check if in gtfo mode
                    if (GTFOmode)
                    {
                        //set target mode to return
                        if (!ReturnToGuard)
                            Mode = TargetMode.Return;
                        else
                            Mode = TargetMode.GuardToPursue;
                        //set throttle to max
                        return 100;
                    }
                    //decrease throttle until hit minspeed
                    if (t > 10)
                        t -= 10;
                    else
                        t = 1;
                }
            }
            return t;
        }

        public PointF find_ClosestTarget(PointF pos, MapComponent map, PointF destination, string team, float range)
        {
            //see if there is an enemy
            //make sure that the enemy is withing radar range
            var querey = map.PassiveRadarMap.FindAll(p => p.Team != team && p.Location.getDistance(pos) <= range/2);
            var target = querey.OrderBy(p => MathExtensions.getLength(pos, p.Location)).Take(1);
            if (target.Count() > 0)
            {
                targetPassiveSignalStrength = target.First().Strength;
                status = AutoEngageStatus.Tracking;
                return target.First().Location;
            }
            else
            {
                status = AutoEngageStatus.Scanning;
                return destination;
            }
        }

        public PointF find_FurthestTarget(PointF pos, MapComponent map, PointF destination, string team, float range)
        {
            //see if there is an enemy
            //make sure that the enemy is withing radar range
            var querey = map.PassiveRadarMap.FindAll(p => p.Team != team && p.Location.getDistance(pos) <= range / 2);
            var target = querey.OrderBy(p => MathExtensions.getLength(pos, p.Location));
            if (target.Count() > 0)
            {
                targetPassiveSignalStrength = target.Last().Strength;
                status = AutoEngageStatus.Tracking;
                return target.Last().Location;
            }
            else
            {
                status = AutoEngageStatus.Scanning;
                return destination;
            }
        }

        public PointF guard_Scan(PointF pos, MapComponent map, string team, float range, bool chase = false)
        {
            //check if any enemy is within in the guard zone
            var querey = map.PassiveRadarMap.FindAll(p => p.Team != team && p.Location.getDistance(GuardPoint) <= GuardAreaRadius && p.Location.getDistance(pos) <= range / 2);
            //find first/last based on request
            var target = querey.OrderBy(p => MathExtensions.getLength(pos, p.Location));
            PointF tp;
            if (target.Count() > 0)
            {
                if (GuardClosest)
                {
                    targetPassiveSignalStrength = target.First().Strength;
                    tp = target.First().Location;
                }
                else
                {
                    targetPassiveSignalStrength = target.Last().Strength;
                    tp = target.Last().Location;
                }
                //set destination to thier position
                //if in guard to pursue set mode to first/last as in request
                if (chase)
                {
                    if (GuardClosest)
                        Mode = TargetMode.Closest;
                    else
                        Mode = TargetMode.Furthest;
                }
                status = AutoEngageStatus.Tracking;
                return tp;
            }
            status = AutoEngageStatus.Scanning;
            return GuardPoint;
        }


        public AutoTarget()
        {
            Enabled = false;
            Mode = TargetMode.Closest;
            GTFOmode = false;
            ConserveFuelmode = false;
            GuardClosest = true;
            ReturnToGuard = false;
            targetPassiveSignalStrength = 0;
            status = AutoEngageStatus.NoTarget;
        }



    }
}
