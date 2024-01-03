using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LethalSettings.UI;
using LethalSettings.UI.Components;
using UnityEngine;

namespace LCReducePipeLeakSFX
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class LCReducePipeLeakSFX : BaseUnityPlugin
    {
        private static string ConfigSection = $"{PluginInfo.PLUGIN_GUID}";
        public static ConfigEntry<float> cfg_BrokenValveVolume;
        private ConfigDefinition cfgDef_BrokenValveVolume = new ConfigDefinition(ConfigSection, "BrokenValve");

        

        private void Awake()
        {
            cfg_BrokenValveVolume = Config.Bind(cfgDef_BrokenValveVolume, 0.675f); // Get default value from inspector ingame
            InitializeModSettings();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        }
        

        
        public void InitializeModSettings()
        {
            

            ModMenu.RegisterMod(new ModMenu.ModSettingsConfig
            {
                Name = PluginInfo.PLUGIN_NAME,
                Version = PluginInfo.PLUGIN_VERSION,
                Id = PluginInfo.PLUGIN_GUID,
                Description = "Reduces or amplifies sounds of the environment",
                MenuComponents = new MenuComponent[]
                {
                    // new LabelComponent(){Text = "Specific Volume Adjustments"},
                    new HorizontalComponent()
                    {
                        Children = new MenuComponent[]{
                            new SliderComponent
                            {
                                MinValue = 0,
                                MaxValue = 200,
                                Value = cfg_BrokenValveVolume.Value,
                                Text = "Broken Valve Volume",
                                OnValueChanged = (self, value) =>
                                {
                                    Logger.LogInfo($"New value: {value}");
                                    cfg_BrokenValveVolume.Value = value;
                                    // ValveVolume = value * 0.01f;
                                }
                            }
                        }
                    },
                    new HorizontalComponent()
                    {
                        Children = new MenuComponent[]
                        {
                            new ButtonComponent()
                            {
                                Text = "Save Volume Preferences",
                                OnClick = (self) => SaveConfig()
                            }
                        }
                    }
                }
            });
        }

        public void SaveConfig()
        {
            Config.Save();
        }
    }

    [HarmonyPatch(typeof(SteamValveHazard), "Start")]
    public static class SetSteamValveAudio
    {
        public static bool Prefix(ref SteamValveHazard __instance)
        {
            __instance.valveAudio.volume = LCReducePipeLeakSFX.cfg_BrokenValveVolume.Value * 0.01f;
            return true;
        }
    }
    
    // [HarmonyPatch(typeof(HUDManager), "SetClock")]
    // public static class OnSetClock
    // {
    //     private static string _currentTime;
    //     public static string CurrentTime { 
    //         get
    //         {
    //             return _currentTime;
    //         } 
    //         set
    //         {
    //             _currentTime = value;
    //             Plugin.UpdateKeywords();
    //         }
    //     }
    //     public static void Postfix(ref HUDManager __instance)
    //     {
    //         CurrentTime = __instance.clockNumber.text.Replace('\n', ' ');
    //     }
    // }
}
