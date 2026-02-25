
using GorillaLocomotion;
using HarmonyLib;
using System;
using UnityEngine;
using Violet.Menu;
using Violet.Utilities;
using VioletPaid.Initialization;
using VioletPaid.Menu;
using VioletPaid.Utilities;

namespace Violet.Initialization
{
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class Initializer : MonoBehaviour
    {

        private static GameObject menuObject;

        private static void Postfix()
        {
            try
            {
                if (menuObject != null) return;
                    menuObject = new GameObject(PluginInfo.menuName);
                    menuObject.AddComponent<Main>();
                    menuObject.AddComponent<Gui>();
                    GameObject.DontDestroyOnLoad(menuObject);
                
            }
            catch (Exception ex) { }
        }
    }
}
