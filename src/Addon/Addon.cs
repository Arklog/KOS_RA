using kOS;
using kOS.AddOns;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using kOS.Suffixed.Part;
using RealAntennas;
using RealAntennas.Targeting;
using UnityEngine;

namespace KOS_RA.Addon
{
    [kOSAddon("RA")]
    [KOSNomenclature("RAAddon")]
    public class Addon : kOS.Suffixed.Addon
    {
        public Addon(SharedObjects shared) : base(shared)
        {
            InitializeSuffixes();
        }

        private void InitializeSuffixes()
        {
            AddSuffix("HASCONNECTION",
                new OneArgsSuffix<BooleanValue, VesselTarget>(RAHasConnection,
                    "Check if given vessel have a connection"));
            AddSuffix("SETTARGETBODY",
                new TwoArgsSuffix<PartValue, StringValue>(RASetTargetBody, "Set an real antenna target"));
            AddSuffix("SETTARGETVESSEL",
                new TwoArgsSuffix<PartValue, VesselTarget>(RASetTargetVessel, "Set an real antenna target"));
        }

        public override BooleanValue Available()
        {
            return true;
        }

        private BooleanValue RAHasConnection(VesselTarget target)
        {
            return target.Vessel.connection.IsConnected;
        }

        private void RASetTargetBody(PartValue part, StringValue target)
        {
            var module = part.Part.FindModuleImplementing<ModuleRealAntenna>();
            var body = FlightGlobals.GetBodyByName(target);

            if (module != null && body != null)
            {
                var antennaTargetNode = new ConfigNode(AntennaTarget.nodeName);
                antennaTargetNode.AddValue("name", $"{AntennaTarget.TargetMode.BodyLatLonAlt}");
                antennaTargetNode.AddValue("bodyName", body.name);
                antennaTargetNode.AddValue("latLonAlt", new Vector3(0, 0, (float)-body.Radius));
                module.Target = AntennaTarget.LoadFromConfig(antennaTargetNode, module.RAAntenna);
            }
        }

        private void RASetTargetVessel(PartValue part, VesselTarget target)
        {
            var module = part.Part.FindModuleImplementing<ModuleRealAntenna>();

            if (module != null)
            {
                var antennaTargetNode = new ConfigNode(AntennaTarget.nodeName);
                antennaTargetNode.AddValue("name", $"{AntennaTarget.TargetMode.Vessel}");
                antennaTargetNode.AddValue("vesselId", target.Vessel.id);
                module.Target = AntennaTarget.LoadFromConfig(antennaTargetNode, module.RAAntenna);
            }
        }
    }
}