using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Norad_AirCommand
{
    class Universe
    {
        //entities to simulate
        public List<Entity> Entities = new List<Entity>();
        public List<Entity> AddEntityList = new List<Entity>();
        //map
        public MapComponent Map = new MapComponent();
        MapComponent tempmap = new MapComponent();
        
        public Plane find_ClosestPlane(PointF pos)
        {
            List<Plane> planes = Entities.OfType<Plane>().ToList();
            var closestPlane = planes.OrderBy(p => MathExtensions.getLength(pos, p.Location)).Take(1);
            return closestPlane.First();
        }

        public void Update()
        {
            foreach (Entity e in Entities)
            {
                e.Update(ref AddEntityList, Map, ref tempmap);
            }
            //cleanse map
            CleneseMap();
        }

        public void Draw(System.Drawing.Graphics g, UniverseDisplaySettings us)
        {
            foreach (Entity e in Entities)
            {
                e.Draw(g, us);
            }
            if (us.DebugHeatSignatures)
            {
                foreach (HeatSignature h in Map.HeatMap)
                {
                    g.DrawEllipse(Pens.Orange, h.Location.X, h.Location.Y, 1, 1);
                }
            }

        }

        public void UpdateandDraw(System.Drawing.Graphics g, UniverseDisplaySettings us)
        {
            foreach (Entity e in Entities)
            {
                e.Update(ref AddEntityList, Map, ref tempmap);
                e.Draw(g, us);
            }
            //clense map
            CleneseMap();
        }
        
        public void CleneseMap()
        {
            Map.ActiveRadarMap.Clear();
            Map.PassiveRadarMap.Clear();
            foreach (HeatSignature h in Map.HeatMap)
            {
                if (h.WillDecay())
                    tempmap.HeatMap.Add(h.DecayedHeadSignature(h));
            }
            Map.HeatMap.Clear();
            Map.KillZoneMap.Clear();
            Map.ActiveRadarMap.AddRange(tempmap.ActiveRadarMap);
            Map.PassiveRadarMap.AddRange(tempmap.PassiveRadarMap);
            Map.HeatMap.AddRange(tempmap.HeatMap);
            Map.KillZoneMap.AddRange(tempmap.KillZoneMap);
            tempmap.ActiveRadarMap.Clear();
            tempmap.PassiveRadarMap.Clear();
            tempmap.HeatMap.Clear();
            tempmap.KillZoneMap.Clear();
            //add entities
            Entities.AddRange(AddEntityList);
            AddEntityList.Clear();
            //kill list
        }


    }
}
