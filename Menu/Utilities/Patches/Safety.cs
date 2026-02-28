using HarmonyLib;
using JetBrains.Annotations;
using Liv.Lck.Telemetry;
using PlayFab;
using PlayFab.EventsModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Violet.Menu.Utilities.Patches
{
    internal class Safety
    {
        static void PrefixDisable() { }

        private static bool ShouldSkip() => false;

        [HarmonyPatch]
        public static class Patches
        {
            [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.EnqueueTelemetryEvent))]
            [HarmonyPrefix]
            private static bool EnqueueTelemetryEvent(string eventName, object content, [CanBeNull] string[] customTags = null) => ShouldSkip();

            [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.EnqueueTelemetryEventPlayFab))]
            [HarmonyPrefix]
            private static bool EnqueueTelemetryEventPlayFab(EventContents eventContent) => ShouldSkip();

            [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.FlushPlayFabTelemetry))]
            [HarmonyPrefix]
            private static bool FlushPlayFabTelemetry() => ShouldSkip();

            [HarmonyPatch(typeof(GorillaTelemetry), nameof(GorillaTelemetry.FlushMothershipTelemetry))]
            [HarmonyPrefix]
            private static bool FlushMothershipTelemetry() => ShouldSkip();

            [HarmonyPatch(typeof(PlayFabEventsAPI), nameof(PlayFabEventsAPI.WriteTelemetryEvents))]
            [HarmonyPrefix]
            private static bool WriteTelemetryEvents(
                WriteEventsRequest request,
                System.Action<WriteEventsResponse> resultCallback,
                System.Action<PlayFabError> errorCallback,
                object customData = null,
                Dictionary<string, string> extraHeaders = null) => ShouldSkip();
        }
    }
}
