using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Norad_AirCommand
{

    struct Radar
    {
        public float gunRange;
        public float SRInfaredRange;
        public float SRInfaredSightAngle;
        public float SRRadarRange;
        public float SRRadarSightAngle;
        public float MRRadarRange;
        public float MRRadarSightAngle;
        public float LRRadarRange;
        public Radar(float gun)
        {
            gunRange = 30;
            SRInfaredRange = 150;
            SRInfaredSightAngle = 30;
            SRRadarRange = 275;
            SRRadarSightAngle = 30;
            MRRadarRange = 450;
            MRRadarSightAngle = 25;
            LRRadarRange = 1200;
        }

    }

    class Plane : Entity
    {
        /// <summary>
        /// The planes unique callsign
        /// </summary>
        public string Callsign;
        /// <summary>
        /// Indicates whether the plane is currently selected by the user
        /// </summary>
        public bool isSelected;
        /// <summary>
        /// The team of the plane
        /// </summary>
        public string Team;
        /// <summary>
        /// The heading of the aircraft in degrees N is 90
        /// </summary>
        public float Orientation;
        /// <summary>
        /// The current speed of the plane
        /// </summary>
        public float CurrentSpeed;
        /// <summary>
        /// The fastest the plane can go
        /// </summary>
        public float MaxSpeed;
        /// <summary>
        /// The slowest the plane can go
        /// </summary>
        public float MinSpeed;
        /// <summary>
        /// The throttle setting
        /// </summary>
        public float Throttle;
        /// <summary>
        /// The ammount of fuel in the plane
        /// </summary>
        public float Fuel;
        /// <summary>
        /// The maximum amount of fuel the plane can hold
        /// </summary>
        public float MaxFuel;
        /// <summary>
        /// The destination of the aircraft
        /// </summary>
        public PointF Destination;
        /// <summary>
        /// A list of the last 5 points the plane was at, used to draw a trail
        /// </summary>
        public PointF[] TrailPoints;
        /// <summary>
        /// Length of trail to display
        /// </summary>
        public int TrailLength;
        /// <summary>
        /// How many degrees / s the plane can turn
        /// </summary>
        public float TurnSpeed;
        /// <summary>
        /// The color to draw team specific details
        /// </summary>
        public Color TeamColor;
        /// <summary>
        /// The planes passive radar signature
        /// </summary>
        public RadarSignature PassiveSignature;
        /// <summary>
        /// The planes active radar signature
        /// </summary>
        public RadarSignature ActiveSignature;
        /// <summary>
        /// The planes current heat signature
        /// </summary>
        public HeatSignature HeatSignature;
        /// <summary>
        /// The status of the aitargeter
        /// </summary>
        public AutoTarget AITarget;
        /// <summary>
        /// All of the missile weapons on the plane
        /// </summary>
        public List<Missile> Missiles;
        /// <summary>
        /// Handles the ai targeting
        /// </summary>
        public AutoEngage AIEngage;
        /// <summary>
        /// The Radar Characteristics of the Plane
        /// </summary>
        public Radar Radar = new Radar(1);

        /// <summary>
        /// Draws the plane
        /// </summary>
        /// <param name="g">The graphics object to use</param>
        public override void Draw(Graphics g, UniverseDisplaySettings us)
        {
            Pen gp = new Pen(TeamColor);
            //draw plane's image
            g.DrawImage(Properties.Resources.plane.Rotate(Orientation), new RectangleF(Location.CenterPoint(new Size(20,20)), new Size(20,20)));
            //highlight if selected
            if (isSelected)
            {
                if (us.HighightSelectedPlane)
                    g.DrawEllipse(Pens.Yellow, new RectangleF(Location.CenterPoint(new Size(20, 20)), new Size(20, 20)));
                //draw gunsight
                g.DrawLine(gp, Location.CreateHitTriangle(Radar.gunRange, Orientation, 5)[2], Location.CreateHitTriangle(Radar.gunRange, Orientation, 5)[1]);
                //draw radar ranges
                //short ir
                g.DrawPolygon(gp, Location.CreateHitTriangle(Radar.SRInfaredRange, Orientation, Radar.SRInfaredSightAngle));
                //short
                g.DrawPolygon(gp, Location.CreateHitTriangle(Radar.SRRadarRange, Orientation, Radar.SRRadarSightAngle));
                //medium
                g.DrawPolygon(gp, Location.CreateHitTriangle(Radar.MRRadarRange, Orientation, Radar.MRRadarSightAngle));
                //radar zone
                Pen p1 = new Pen(TeamColor);
                p1.DashPattern = new float[] { 5f, 20f, 1f, 20f };
                g.DrawEllipse(p1, new RectangleF(Location.CenterPoint(new SizeF(Radar.LRRadarRange, Radar.LRRadarRange)), new SizeF(Radar.LRRadarRange, Radar.LRRadarRange)));
                //draw projected travel
                p1.DashPattern = new float[] { 5f, 10f };
                g.DrawLine(p1, Location, Destination);
                //draw destination as circle
                g.DrawEllipse(gp, new RectangleF(Destination.CenterPoint(new Size(5, 5)), new Size(5, 5)));
                //draw guardpoint as circle
                if (AITarget.Mode == TargetMode.Guard || AITarget.Mode == TargetMode.GuardToPursue)
                {
                    //draw center point
                    g.DrawEllipse(gp, new RectangleF(AITarget.GuardPoint.CenterPoint(new Size(5, 5)), new Size(5, 5)));
                    //draw radius
                    g.DrawEllipse(p1, new RectangleF(AITarget.GuardPoint.CenterPoint(new SizeF(AITarget.GuardAreaRadius * 2, AITarget.GuardAreaRadius * 2)), new SizeF(AITarget.GuardAreaRadius * 2, AITarget.GuardAreaRadius * 2)));
                }
                //draw trail
                Pen p = new Pen(TeamColor);
                p.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                p.DashPattern = new float[] { 1f, 5f };
                g.DrawLines(gp, TrailPoints);
            }
            else
            {
                //draw gunsight
                if (us.AllGunSight)
                    g.DrawLine(gp, Location.CreateHitTriangle(Radar.gunRange, Orientation, 5)[2], Location.CreateHitTriangle(Radar.gunRange, Orientation, 5)[1]);
                //draw radar ranges
                //short ir
                if (us.AlllIRRadar)
                    g.DrawPolygon(gp, Location.CreateHitTriangle(Radar.SRInfaredRange, Orientation, Radar.SRInfaredSightAngle));
                //short
                if (us.AllSRRadar)
                    g.DrawPolygon(gp, Location.CreateHitTriangle(Radar.SRRadarRange, Orientation, Radar.SRRadarSightAngle));
                //medium
                if (us.AllMRRadar)
                    g.DrawPolygon(gp, Location.CreateHitTriangle(Radar.MRRadarRange, Orientation, Radar.MRRadarSightAngle));
                //radar zone
                Pen p1 = new Pen(TeamColor);
                p1.DashPattern = new float[] { 5f, 20f, 1f, 20f };
                if (us.AllLRRadar)
                    g.DrawEllipse(p1, new RectangleF(Location.CenterPoint(new SizeF(Radar.LRRadarRange, Radar.LRRadarRange)), new SizeF(Radar.LRRadarRange, Radar.LRRadarRange)));
                //draw projected travel
                p1.DashPattern = new float[] { 5f, 10f };
                if (us.AllDestination)
                {
                    g.DrawLine(p1, Location, Destination);
                    //draw destination as circle
                    g.DrawEllipse(gp, new RectangleF(Destination.CenterPoint(new Size(5, 5)), new Size(5, 5)));
                }
                //draw guardpoint as circle
                if (us.AllGuardZone)
                {
                    if (AITarget.Mode == TargetMode.Guard || AITarget.Mode == TargetMode.GuardToPursue)
                    {
                        //draw center point
                        g.DrawEllipse(gp, new RectangleF(AITarget.GuardPoint.CenterPoint(new Size(5, 5)), new Size(5, 5)));
                        //draw radius
                        g.DrawEllipse(p1, new RectangleF(AITarget.GuardPoint.CenterPoint(new SizeF(AITarget.GuardAreaRadius * 2, AITarget.GuardAreaRadius * 2)), new SizeF(AITarget.GuardAreaRadius * 2, AITarget.GuardAreaRadius * 2)));
                    }
                }
                //draw trail
                Pen p = new Pen(TeamColor);
                p.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                p.DashPattern = new float[] { 1f, 5f };
                if (us.AllTrail)
                    g.DrawLines(gp, TrailPoints);
            }
        }

        /// <summary>
        /// Updates the plane
        /// </summary>
        /// <param name="map">The map containg universe data</param>
        public override void Update(ref List<Entity>EntityList, MapComponent map, ref MapComponent tempmap)
        {
            //update destination based on autotarget mode
            update_Destination(map);
            //update autoengage status from autotarget
            AIEngage.Status = AITarget.status;
            //have autopilot check if manuvers are required
            //update heading based on destination
            Orientation = update_Heading();
            //calculate throttle
            Throttle = AITarget.maintainThrottle(Location, Destination, Throttle);
            //calculate the plane's current speed
            calculate_Speed();
            //figure update location based on speed and destination
            Location = Location.CalculateNewPoint(Orientation, CurrentSpeed);
            //update trail
            update_Trail();
            //update map signature
            update_MapSignature(tempmap);
            //update fuel
            calculate_Fuel();
            //manage weapons and engagement
            manage_Weapons(EntityList, map, tempmap);
        }

        public virtual void manage_Weapons(List<Entity> EntityList, MapComponent map, MapComponent tempmap)
        {
            //use autoengage to deal with missiles
            if (AIEngage.Enabled)
                AIEngage.engage_Missiles(Missiles, Radar, Location, Orientation, Destination, EntityList);
            //deal with guns

        }

        public virtual void calculate_Fuel()
        {
            Fuel = Fuel - Throttle / 1000;
        }

        public virtual void calculate_Speed()
        {
            //determine ratio of throttle to range between max and min speed
            float r = (MaxSpeed - MinSpeed) / 100; //r is the scaled throttle range, but fails to take into account minspeed
            //then calculate the speed
            CurrentSpeed = r * Throttle;
            //make sure that min speed is observed
            if (CurrentSpeed < MinSpeed)
                CurrentSpeed = MinSpeed;
        }

        
        public virtual void update_Destination(MapComponent map)
        {
            //first make sure that the plane does not need to take evasive manuvers

            //if in autotarget is enabled 
            Destination = AITarget.chooseDestination(Location, map, Destination, Team, Radar.LRRadarRange, Throttle);
        }

        /// <summary>
        /// Updates the planes map signatures
        /// </summary>
        /// <param name="map">The map to update</param>
        public virtual void update_MapSignature(MapComponent tempmap)
        {
            //update active and passive radars
            ActiveSignature.Location = Location; //eventually update based radar output
            PassiveSignature.Location = Location;
            //update heat signatures
            //HeatSignature.Location = Location;
            //HeatSignature.Temperature = calculate_EngineTemp();

            //add to map
            tempmap.ActiveRadarMap.Add(ActiveSignature);
            tempmap.PassiveRadarMap.Add(PassiveSignature);
            tempmap.HeatMap.Add(new HeatSignature(Location, calculate_EngineTemp(), Team));
        }

        /// <summary>
        /// Updates the plane's heading based on its destination and max turn speed
        /// </summary>
        /// <returns>The plane's new heading</returns>
        public virtual float update_Heading()
        {
            //update the angle between location and destination
            float hdg;
            //figure out which way to turn and by how much you need to
            AngleDifference ad = Orientation.GetAngleDifference(Location.GetAngleF(Destination));
            //see if can make whole turn, else do as much as possible
            if (ad.CCW)
            {
                if (ad.Difference < TurnSpeed)
                    hdg = Orientation.ContinuousAngleAddition(ad.Difference);
                else
                    hdg = Orientation.ContinuousAngleAddition(TurnSpeed);
            }
            else
            {
                if (ad.Difference < TurnSpeed)
                    hdg = Orientation.ContinuousAngleSubtraction(ad.Difference);
                else
                    hdg = Orientation.ContinuousAngleSubtraction(TurnSpeed);
            }
            return hdg;
        }


        public virtual void update_Trail()
        {
            for (int i = 0; i < TrailLength - 1; i++)
            {
                TrailPoints[i] = TrailPoints[i + 1];
            }
            TrailPoints[TrailLength - 1] = Location;
        }

        public void initializeTrail()
        {
            for (int i = 0; i < TrailLength; i++)
            {
                TrailPoints[i] = Location;
            }
        }

        public virtual float calculate_EngineTemp()
        {
            return Throttle * 25; //default
        }

        public Plane(PointF location, float speed, float orientation, PointF destination, Color tColor, string team)
        {
            Location = location;
            CurrentSpeed = speed;
            MaxSpeed = 7;
            MinSpeed = 3;
            Throttle = 100;
            MaxFuel = 1000;
            Fuel = MaxFuel; //eventually have ability to set fuel load 100%, 90% etc.
            Orientation = orientation;
            Destination = destination;
            TurnSpeed = 6.7f; //temporary for now
            TrailLength = 15;
            TrailPoints = new PointF[TrailLength];
            initializeTrail();
            TeamColor = tColor;
            Random r = new Random();
            PassiveSignature = new RadarSignature(Location, 10, r.Next(), team);
            ActiveSignature = new RadarSignature(Location, 10, r.Next(), team);
            HeatSignature = new HeatSignature(Location, calculate_EngineTemp(), team);
            AITarget = new AutoTarget();
            AIEngage = new AutoEngage();
            Missiles = new List<Missile>();
            Team = team;
            isSelected = true;
        }



    }
}
