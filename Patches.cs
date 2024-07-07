
using EFT.Interactive;
using HarmonyLib;
using System.Reflection;
using SPT.Reflection.Patching;
using UnityEngine;

namespace HazardPatches
{
    public class MinefieldTriggerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Minefield), "IsInTriggerZone");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }

    public class MinefieldCoroutinePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Minefield), "FireCoroutine");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }

    public class MinefieldDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Minefield), "method_3");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }

    public class MinefieldViewTriggerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MinefieldView), "method_0");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class MineDirectionalAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MineDirectional), "Awake");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class MineDirectionalTriggerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MineDirectional), "OnTriggerEnter");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class MineDirectionalTriggerColliderPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MineDirectional), "method_1");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }

    public class MineDirectionalDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MineDirectional), "method_3");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class BarbedWireDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BarbedWire), "ProceedDamage");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class BarbedWireSpeedPenaltyPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BarbedWire), "AddPenalty");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class BarbedWireSpeedPenalty2Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(BarbedWire), "RemovePenalty");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class SniperImitatorAwakePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SniperImitator), "Awake");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class SniperImitatorDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SniperImitator), "method_1");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class SniperImitatorShootPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SniperImitator), "method_0");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class SniperFiringZoneShootPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SniperFiringZone), "Shoot");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class SniperFiringZoneCoroutinePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SniperFiringZone), "FireCoroutine");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class SniperFiringZoneTargetPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SniperFiringZone), "method_3");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class SniperFiringZoneTarget2Patch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(SniperFiringZone), "method_4");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
    public class FlameDamageTriggerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(FlameDamageTrigger), "ProceedDamage");
        }

        [PatchPrefix]
        public static bool PatchPrefix()
        {
            return false;
        }
    }
}