using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using HazardPatches;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TYR_DeHazardifier
{
    [BepInPlugin("com.TYR.DeHazardifier", "TYR_DeHazardifier", "1.0.3")]
    public class DeClutter : BaseUnityPlugin
    {
        private static GameWorld gameWorld;
        public static bool MapLoaded() => Singleton<GameWorld>.Instantiated;
        public static List<GameObject> savedDirectionalMinesObjects = new List<GameObject>();
        public static List<GameObject> savedBarbedWireObjects = new List<GameObject>();
        public static ConfigEntry<bool> deHazardifierEnabledConfig;
        public static ConfigEntry<bool> minefieldsEnabledConfig;
        public static ConfigEntry<bool> directionalMinesEnabledConfig;
        public static ConfigEntry<bool> directionalMinesVisualsEnabledConfig;
        public static ConfigEntry<bool> barbedWireEnabledConfig;
        public static ConfigEntry<bool> barbedWireVisualsEnabledConfig;
        public static ConfigEntry<bool> sniperBorderZonesEnabledConfig;
        public static ConfigEntry<bool> fireDamageEnabledConfig;
        public static bool deHazardifiered = false;
        
        private void Awake()
        {
            deHazardifierEnabledConfig = Config.Bind("A - De-Hazardifier Enabler", "A - De-Hazardifier Enabled", true, "Enables the De-Hazardifier.");
            minefieldsEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "A - Minefield Disabler", true, "Disables minefields.");
            directionalMinesEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "B - Claymore Mines Disabler", true, "Disables claymore mines.");
            directionalMinesVisualsEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "C - Claymore Mines Visuals Disabler", true, "Disables visual model of claymore mines.");
            barbedWireEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "D - Barbed Wire Disabler", true, "Disables barbed wire.");
            barbedWireVisualsEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "E - Barbed Wire Visuals Disabler", true, "Disables visual model of barbed wire.");
            sniperBorderZonesEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "F - Sniper Border Zones Disabler", true, "Disables sniper border zones.");
            fireDamageEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "G - Fire Damage Disabler", true, "Disables damage taken from standing in fire.");
            deHazardifierEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            minefieldsEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            directionalMinesEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            directionalMinesVisualsEnabledConfig.SettingChanged += OnApplyDirectionalMinesVisualsSettingChanged;
            barbedWireEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            barbedWireVisualsEnabledConfig.SettingChanged += OnApplyBarbedWireVisualsSettingChanged;
            sniperBorderZonesEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            fireDamageEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            PatchSetter();
        }
        private void OnApplyDirectionalMinesVisualsSettingChanged(object sender, EventArgs e)
        {
            DirectionalMinesVisualsScene();
        }
        private void OnApplyBarbedWireVisualsSettingChanged(object sender, EventArgs e)
        {
            BarbedWireVisualsScene();
        }
        private void BarbedWireVisualsScene()
        {;
            foreach (GameObject obj in savedBarbedWireObjects)
            {
                if (obj != null)
                {
                    if (barbedWireVisualsEnabledConfig.Value)
                    {
                        obj.SetActive(false);
                    }
                    else
                    {
                        obj.SetActive(true);
                    }
                }
            }
        }
        private void OnApplyDeHazardifierSettingChanged(object sender, EventArgs e)
        {
            PatchSetter();
        }
        private void DirectionalMinesVisualsScene()
        {
            foreach (GameObject obj in savedDirectionalMinesObjects)
            {
                if (obj != null)
                {
                    if (directionalMinesVisualsEnabledConfig.Value)
                    {
                        obj.SetActive(false);
                    }
                    else
                    {
                        obj.SetActive(true);
                    }
                }
            }
        }
        private void OnSceneUnloaded(Scene scene)
        {
            savedDirectionalMinesObjects.Clear();
            savedBarbedWireObjects.Clear();
            deHazardifiered = false;
        }
        private void Update()
        {
            if (!MapLoaded() || deHazardifiered || !deHazardifierEnabledConfig.Value)
                return;

            gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.MainPlayer == null)
                return;

            StaticManager.BeginCoroutine(DeHazardifyVisuals());
            DirectionalMinesVisualsScene();
            BarbedWireVisualsScene();
            deHazardifiered = true;
        }
        private IEnumerator DeHazardifyVisuals()
        {
            List<GameObject> allGameObjects = new List<GameObject>();
            GameObject[] rootObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject root in rootObjects)
            {
                bool isMine = root.GetComponent<MineDirectional>() != null;
                string isMineGrouped = root.name.ToLower();
                bool isBarbedWire = root.GetComponent<BarbedWire>() != null;
                if (directionalMinesVisualsEnabledConfig.Value && (isMine || isMineGrouped == "mines"))
                {
                    allGameObjects.Add(root);
                    savedDirectionalMinesObjects.Add(root);
                }
                if (barbedWireVisualsEnabledConfig.Value && isBarbedWire)
                {
                    allGameObjects.Add(root);
                    savedBarbedWireObjects.Add(root);
                }
            }
            yield break;
        }
        public static void PatchSetter()
        {
            if (minefieldsEnabledConfig.Value)
            {
                new MinefieldTriggerPatch().Enable();
                new MinefieldCoroutinePatch().Enable();
                new MinefieldDamagePatch().Enable();
                new MinefieldViewTriggerPatch().Enable();
            }
            else
            {
                new MinefieldTriggerPatch().Disable();
                new MinefieldCoroutinePatch().Disable();
                new MinefieldDamagePatch().Disable();
                new MinefieldViewTriggerPatch().Disable();
            }
            if (directionalMinesEnabledConfig.Value)
            {
                new MineDirectionalAwakePatch().Enable();
                new MineDirectionalTriggerPatch().Enable();
                new MineDirectionalTriggerColliderPatch().Enable();
                new MineDirectionalDamagePatch().Enable();
            }
            else
            {
                new MineDirectionalAwakePatch().Disable();
                new MineDirectionalTriggerPatch().Disable();
                new MineDirectionalTriggerColliderPatch().Disable();
                new MineDirectionalDamagePatch().Disable();
            }
            if (barbedWireEnabledConfig.Value)
            {
                new BarbedWireDamagePatch().Enable();
                new BarbedWireSpeedPenaltyPatch().Enable();
                new BarbedWireSpeedPenalty2Patch().Enable();
            }
            else
            {
                new BarbedWireDamagePatch().Disable();
                new BarbedWireSpeedPenaltyPatch().Disable();
                new BarbedWireSpeedPenalty2Patch().Disable();
            }
            if (sniperBorderZonesEnabledConfig.Value)
            {
                new SniperImitatorAwakePatch().Enable();
                new SniperImitatorDamagePatch().Enable();
                new SniperImitatorShootPatch().Enable();
                new SniperFiringZoneShootPatch().Enable();
                new SniperFiringZoneCoroutinePatch().Enable();
                new SniperFiringZoneTargetPatch().Enable();
                new SniperFiringZoneTarget2Patch().Enable();
            }
            else
            {
                new SniperImitatorAwakePatch().Disable();
                new SniperImitatorDamagePatch().Disable();
                new SniperImitatorShootPatch().Disable();
                new SniperFiringZoneShootPatch().Disable();
                new SniperFiringZoneCoroutinePatch().Disable();
                new SniperFiringZoneTargetPatch().Disable();
                new SniperFiringZoneTarget2Patch().Disable();
            }
            if (fireDamageEnabledConfig.Value)
            {
                new FlameDamageTriggerPatch().Enable();
            }
            else
            {
                new FlameDamageTriggerPatch().Disable();
            }
        }
    }
}
