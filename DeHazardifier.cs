using BepInEx;
using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using HazardPatches;
using System;
using System.Collections;
using System.Collections.Generic;
using DrakiaXYZ.VersionChecker;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TYR_DeHazardifier
{
    [BepInPlugin("com.TYR.DeHazardifier", "TYR_DeHazardifier", "1.0.6")]
    [BepInDependency("com.SPT.custom", "3.11.0")]
    public class DeClutter : BaseUnityPlugin
    {
        public const int TarkovVersion = 35392;
        
        private static GameWorld gameWorld;
        private static bool MapLoaded() => Singleton<GameWorld>.Instantiated;
        private static List<GameObject> _savedDirectionalMinesObjects = [];
        private static List<GameObject> _savedBarbedWireObjects = [];
        private static ConfigEntry<bool> _deHazardifierEnabledConfig;
        private static ConfigEntry<bool> _minefieldsEnabledConfig;
        private static ConfigEntry<bool> _directionalMinesEnabledConfig;
        private static ConfigEntry<bool> _directionalMinesVisualsEnabledConfig;
        private static ConfigEntry<bool> _barbedWireEnabledConfig;
        private static ConfigEntry<bool> _barbedWireVisualsEnabledConfig;
        private static ConfigEntry<bool> _sniperBorderZonesEnabledConfig;
        private static ConfigEntry<bool> _fireDamageEnabledConfig;
        private static bool _deHazardifiered;
        
        private void Awake()
        {
            if (!VersionChecker.CheckEftVersion(Logger, Info, Config))
            {
                throw new Exception("Invalid EFT Version");
            }
            
            _deHazardifierEnabledConfig = Config.Bind("A - De-Hazardifier Enabler", "A - De-Hazardifier Enabled", true, "Enables the De-Hazardifier.");
            _minefieldsEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "A - Minefield Disabler", true, "Disables minefields.");
            _directionalMinesEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "B - Claymore Mines Disabler", true, "Disables claymore mines.");
            _directionalMinesVisualsEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "C - Claymore Mines Visuals Disabler", true, "Disables visual model of claymore mines.");
            _barbedWireEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "D - Barbed Wire Disabler", true, "Disables barbed wire.");
            _barbedWireVisualsEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "E - Barbed Wire Visuals Disabler", true, "Disables visual model of barbed wire.");
            _sniperBorderZonesEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "F - Sniper Border Zones Disabler", true, "Disables sniper border zones.");
            _fireDamageEnabledConfig = Config.Bind("A - De-Hazardifier Settings", "G - Fire Damage Disabler", true, "Disables damage taken from standing in fire.");
            _deHazardifierEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            _minefieldsEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            _directionalMinesEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            _directionalMinesVisualsEnabledConfig.SettingChanged += OnApplyDirectionalMinesVisualsSettingChanged;
            _barbedWireEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            _barbedWireVisualsEnabledConfig.SettingChanged += OnApplyBarbedWireVisualsSettingChanged;
            _sniperBorderZonesEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
            _fireDamageEnabledConfig.SettingChanged += OnApplyDeHazardifierSettingChanged;
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
        {
            foreach (var obj in _savedBarbedWireObjects)
            {
                if (obj is null) continue;
                
                obj.SetActive(_barbedWireVisualsEnabledConfig.Value);
            }
        }
        
        private void OnApplyDeHazardifierSettingChanged(object sender, EventArgs e)
        {
            PatchSetter();
        }
        
        private void DirectionalMinesVisualsScene()
        {
            foreach (var obj in _savedDirectionalMinesObjects)
            {
                if (obj is null) continue;
                    
                obj.SetActive(_directionalMinesVisualsEnabledConfig.Value);
            }
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            _savedDirectionalMinesObjects.Clear();
            _savedBarbedWireObjects.Clear();
            _deHazardifiered = false;
        }
        
        private void Update()
        {
            if (!MapLoaded() || _deHazardifiered || !_deHazardifierEnabledConfig.Value)
                return;

            gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld is null || gameWorld.MainPlayer is null)
                return;

            StaticManager.BeginCoroutine(DeHazardifyVisuals());
            DirectionalMinesVisualsScene();
            BarbedWireVisualsScene();
            _deHazardifiered = true;
        }
        private IEnumerator DeHazardifyVisuals()
        {
            var allGameObjects = new List<GameObject>();
            var rootObjects = FindObjectsOfType<GameObject>();

            foreach (var root in rootObjects)
            {
                var isMine = root.GetComponent<MineDirectional>() is not null;
                var isMineGrouped = root.name.ToLower();
                var isBarbedWire = root.GetComponent<BarbedWire>() is not null;
                
                if (_directionalMinesVisualsEnabledConfig.Value && (isMine || isMineGrouped == "mines"))
                {
                    allGameObjects.Add(root);
                    _savedDirectionalMinesObjects.Add(root);
                }
                
                if (_barbedWireVisualsEnabledConfig.Value && isBarbedWire)
                {
                    allGameObjects.Add(root);
                    _savedBarbedWireObjects.Add(root);
                }
            }
            yield break;
        }
        
        private static void PatchSetter()
        {
            if (_minefieldsEnabledConfig.Value)
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
            
            if (_directionalMinesEnabledConfig.Value)
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
            
            if (_barbedWireEnabledConfig.Value)
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
            
            if (_sniperBorderZonesEnabledConfig.Value)
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
            
            if (_fireDamageEnabledConfig.Value)
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
