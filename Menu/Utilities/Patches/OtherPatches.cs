using GorillaLocomotion;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Violet.Utilities.Patches
{
    public class OtherPatches
    {
        public static bool enabled = true;
        [HarmonyPatch(typeof(LegalAgreements), "Update")]
        public class Update
        {
            private static bool Prefix(LegalAgreements __instance)
            {
                if (enabled)
                {
                    ControllerInputPoller.instance.leftControllerPrimary2DAxis.y = -1f;
                    __instance.scrollSpeed = 10f;
                    __instance._maxScrollSpeed = 10f;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ModIOTermsOfUse_v1), "PostUpdate")]
        public class PostUpdateModIO
        {
            private static bool Prefix(ModIOTermsOfUse_v1 __instance)
            {
                if (enabled)
                {
                    __instance.TurnPage(999);
                    ControllerInputPoller.instance.leftControllerPrimary2DAxis.y = -1f;
                    __instance.holdTime = 0.1f;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(AgeSlider), "PostUpdate")]
        public class PostUpdateAgeSlider
        {
            private static bool Prefix(AgeSlider __instance)
            {
                if (enabled)
                {
                    __instance._currentAge = 21;
                    __instance.holdTime = 0.1f;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PrivateUIRoom), "StartOverlay")]
        public class StartOverlay
        {
            private static bool Prefix()
            {
                return !enabled;
            }
        }

        [HarmonyPatch(typeof(KIDManager), "UseKID")]
        public class UseKID
        {
            private static bool Prefix(ref Task<bool> __result)
            {
                if (!enabled)
                {
                    return true;
                }
                __result = Task.FromResult<bool>(false);
                return false;
            }
        }


        [HarmonyPatch(typeof(GorillaLocomotion.GTPlayer), "AntiTeleportTechnology", MethodType.Normal)]
        public class NoAntiTP
        {
            static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabClientAPI), "AttributeInstall")]
        internal class NoAttributeInstall
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabHttp), "InitializeScreenTimeTracker")]
        internal class NoInitializeScreenTimeTracker
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabDeviceUtil), "GetAdvertIdFromUnity")]
        internal class NoGetAdvertIdFromUnity
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabDeviceUtil), "DoAttributeInstall")]
        internal class NoDoAttributeInstall
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportDeviceInfo")]
        internal class NoDeviceInfo2
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabClientAPI), "ReportDeviceInfo")]
        internal class NoDeviceInfo1
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabDeviceUtil), "SendDeviceInfoToPlayFab")]
        internal class NoDeviceInfo
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabClientInstanceAPI), "ReportPlayer", MethodType.Normal)]
        public class NoReportPlayer
        {
            static bool Prefix(ReportPlayerClientRequest request, Action<ReportPlayerClientResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayFabClientAPI), "ReportPlayer", MethodType.Normal)]
        public class PlayFabReportPatch2
        {
            private static bool Prefix(ReportPlayerClientRequest request, Action<ReportPlayerClientResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
            {
                return false;
            }
        }


        [HarmonyPatch(typeof(MonkeAgent), "SendReport")]
        public static class NoSendReport
        {
            private static bool Prefix(string susReason, string susId, string susNick)
            {
                if (susId == PhotonNetwork.LocalPlayer.UserId)
                {
                    Debug.Log("Reported By Anti Cheat : " + susReason);
                    NotificationLib.SendNotification("<color=blue>Anti-Cheat</color> : Reason: " + susReason);
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "CheckReports", MethodType.Enumerator)]
        public class ReportCheck : MonoBehaviourPunCallbacks
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "LogErrorCount")]
        public class LogErrorCount : MonoBehaviourPunCallbacks
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(MonkeAgent), "DispatchReport")]
        public class Dispatch : MonoBehaviourPunCallbacks
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin))]
        [HarmonyPatch("GracePeriod", MethodType.Enumerator)]
        public class NoGracePeriod
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin))]
        [HarmonyPatch("LateUpdate", MethodType.Normal)]
        public class NoGracePeriod4
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2))]
        [HarmonyPatch("GracePeriod", MethodType.Enumerator)]
        public class NoGracePeriod3
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2))]
        [HarmonyPatch("LateUpdate", MethodType.Normal)]
        public class NoGracePeriod2
        {
            public static bool Prefix()
            {
                return false;
            }
        }
        [HarmonyPatch(typeof(GTPlayer), "ApplyKnockback")]
        public class KnockbackPatch
        {
            public static bool enabled = false;

            public static bool Prefix(Vector3 direction, float speed)
            {
                if (enabled)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
