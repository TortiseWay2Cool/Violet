using BepInEx;
using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

namespace Violet.Initialization
{

    [BepInPlugin("com.Violet.Violetpaid.org", "Violet Paid", "7.0")]
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class BepInExInitializer : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource LoggerInstance;

        void Awake()
        {
            LoggerInstance = Logger;
            new Harmony("com.Violet.Violetpaid.org").PatchAll();
        }
    }
}
