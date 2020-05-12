using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Norad_AirCommand
{
    class UniverseDisplaySettings
    {
        public bool HighightSelectedPlane;
        public bool AlllIRRadar;
        public bool AllLRRadar;
        public bool AllMRRadar;
        public bool AllSRRadar;
        public bool AllGunSight;
        public bool AllDestination;
        public bool AllTrail;
        public bool AllGuardZone;
        public bool DebugHeatSignatures;

        public UniverseDisplaySettings()
        {
            HighightSelectedPlane = true;
            AllDestination = true;
            AllGunSight = true;
            AllLRRadar = true;
            AllMRRadar = true;
            AllSRRadar = true;
            AllTrail = true;
            AlllIRRadar = true;
            AllGuardZone = true;
            DebugHeatSignatures = false;
        }


    }
}
