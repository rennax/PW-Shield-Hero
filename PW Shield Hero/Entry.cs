//using Harmony;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CloudheadGames.CHFramework.Platform;
using Il2CppSystem;
using UnityEngine;
using UnhollowerRuntimeLib;

namespace PW_Shield_Hero
{
    public class Entry : MelonMod
    {
        


        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            ClassInjector.RegisterTypeInIl2Cpp<Shield>();
            ClassInjector.RegisterTypeInIl2Cpp<SH_Settings>();
        }

        [HarmonyPatch(typeof(ControllerMonitor), "Start", new System.Type[0] { })]
        public static class ControllerMonitor_Start
        {
            public static void Postfix(ControllerMonitor __instance)
            {
                //var go = GameObject.Find("Staging/Set Dressing/Backlot Gates/PF_Sandwich_Board_UI_StartTransitionToBacklot/Mesh/PF_SongPosterCanvas_UIv2/PF_ImgLinkButton_UIv2/GameEventTrigger");
                //GameEventTrigger eventTrigger = go.GetComponent<GameEventTrigger>();
                //eventTrigger.Event.AddListener(new System.Action());
            }
        }


        [HarmonyPatch(typeof(TrackModUIController), "OnOpenPanel", new System.Type[0] { })]
        public static class TrackModUIController_OnOpenPanel
        {
            static bool initialized = false;
            public static void Postfix(TrackModUIController __instance)
            {
                if (initialized)
                    return;


                GameObject go = new GameObject(); // Empty GO
                go.AddComponent<SH_Settings>();

                initialized = true;
            }
        }


        [HarmonyPatch(typeof(GameplayDatabase), "Init", new System.Type[] { })]
        public static class GameplayDatabase_Init
        {
            public static void Postfix(GameplayDatabase __instance)
            {

                __instance.modifiers[18].showInModifierPane = true;
                var mods = __instance.Modifiers.ToList();
                mods.Add(__instance.modifiers[18]);
                __instance.Modifiers = mods.ToArray();

                //TODO enable modifiers that are otherwise disabled.
            }
        }

        //for testing
        //[HarmonyPatch(typeof(AccessibilityManager), "GetEnabledAccessModsAsList", new System.Type[0] { })]
        //public static class AccessibilityManager_GetEnabledAccessModsAsList
        //{
        //    public static void Postfix(AccessibilityManager __instance, ref List<GameModifierEntry> __result)
        //    {
        //        if (__result != null)
        //        {
        //            MelonLogger.Msg("Logging modifier names");
        //            foreach (var mod in __result)
        //            {
        //                MelonLogger.Msg(mod.name);
        //            }
        //        }
        //        else
        //        {
        //            MelonLogger.Msg("GetEnabledAccessModsAsList: failed to ref mods");
        //        }
        //    }
        //}

        ////for testing
        //[HarmonyPatch(typeof(GameplayDatabase), "CanSetModifier", new System.Type[] { typeof(GameModifier) })]
        //public static class GameplayDatabase_CanSetModifier
        //{
        //    public static void Postfix(GameplayDatabase __instance, GameModifier id, ref bool __result)
        //    {

        //        if (__result)
        //        {
        //            MelonLogger.Msg($"Can set modifier id {id}");
        //        }
        //        else
        //        {
        //            MelonLogger.Msg($"Cannot set modifier id {id}");
        //        }
        //    }
        //}


        //[HarmonyPatch(typeof(TrackModUIController), "Awake", new System.Type[0] { })]
        //public static class TrackModUIController_Start
        //{
        //    public static void Postfix(TrackModUIController __instance)
        //    {
        //        GameModifierEntry mod = new GameModifierEntry();

        //        mod.name = "Shield Hero";
        //        mod.description = "Up your defence and fight back!";
        //        mod.gameplayElements = new Il2CppSystem.Collections.Generic.List<GameplayElementHandler>();
        //        mod.id = 1337;
        //        mod.Index = 1337;
        //        mod.leaderboardID = (LeaderboardModifier)1337;
        //        mod.legacy_id = (GameModifier)1337;
        //        mod.multiplier = 0;
        //        mod.showInModifierPane = true;
        //        mod.showInLeaderboard = false;

        //        __instance.modEntries.AddItem(mod);
        //    }
        //}

        //private static void Init()
        //{
        //    InitShield();
        //}

        //private static void InitShield()
        //{
        //    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    go.name = "shield";
        //    go.layer = 8; //Layer 8 = player
        //    Transform leftHand = GameObject.Find("UnityXR_VRCameraRig(Clone)/TrackingSpace/Left Hand").transform;
        //    if (leftHand == null)
        //    {
        //        MelonLogger.Msg("Did not find UnityXR_VRCameraRig(Clone)/TrackingSpace/Left Hand");
        //        return;
        //    }

        //    go.transform.localScale = new Vector3(0.5f, 0.5f, 0.1f);
        //                go.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Cloudhead/Universal/HighEnd/PW_Guns"));

        //    MelonLogger.Msg("InitShield");
        //    shield = go.AddComponent<Shield>();
        //    shield.parent = leftHand;

        //    Rigidbody rb = go.AddComponent<Rigidbody>();
        //    rb.isKinematic = true;
        //    rb.useGravity = false;

        //}
    }
}
