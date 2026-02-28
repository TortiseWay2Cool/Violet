using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using static Violet.Utilities.ColorLib;
using UnityEngine.Animations.Rigging;
using Violet.Menu;
using TMPro;

namespace Violet.Utilities
{
    public class Variables
    {
        public static GameObject menuObj;
        public static GameObject background;
        public static GameObject Catagorybackground;
        public static GameObject canvasObj;
        public static GameObject clickerObj;
        public static GameObject PageButtons;
        public static GameObject catButtons;
        public static GameObject BackCat;
        public static GameObject FWDCat;
        public static GameObject CurrantCatTXT;
        public static GameObject disconnectButton;
        public static GameObject ModButton;
        public static TextMeshPro title;
        public static float Delay;



        public static int Theme = 0;
        public static Color MenuColor = Black;
        public static Color MenuColorT = MenuColor;
        public static Color ButtonColorOff = ColorLib.Violet;
        public static Color ButtonColorOn = Indigo;
        public static Color DisconnectColor = ColorLib.Violet;
        public static Color outColor = ColorLib.Violet;
        public static Color disOut = WineRed;
        public static bool RainbowOutline = true;
        public static bool gravity;
        public static bool bark;
        public static bool trail = true;
        public static int ActualSound = 66;
        public static int SoundIndex = 1;


        public static Category currentPage = Category.Home;
        public static int currentCategoryPage = 0;
        public static int ButtonsPerPage = 8;
        public static bool toggledisconnectButton = false;
        public static bool rightHandedMenu = false;
        public static bool catsSwitch = true;
        public static bool toggleNotifications = true;
        public static bool PCMenuOpen = false;
        public static bool SideCatagorys = true;
        public static bool PlayerTab = true;
        public static bool Rigidbody = false;
        public static KeyCode PCMenuKey = KeyCode.F;
        public static bool openMenu;
        public static bool menuOpen = false;
        public static bool InMenuCondition;
        public static float lastFPSTime = 0f;
        public static int actorNumber;
        public static int fps;
        public static bool InPcCondition;
        

        public static GorillaLocomotion.GTPlayer playerInstance;
        public static GorillaTagger taggerInstance;
        public static ControllerInputPoller pollerInstance;
        public static VRRig vrrig = null;
        public static Material vrrigMaterial;


        public static GameObject thirdPersonCamera;
        public static GameObject shoulderCamera;
        public static GameObject TransformCam;
        public static bool didThirdPerson = false;
        public static GameObject cm;


        public static Rigidbody currentMenuRigidbody;
        public static Vector3 previousVelocity = Vector3.zero;

        public const float velocityThreshold = 0.05f;
        public static int Rotation = 1;

        
        public static void Placeholder() { }
        public static readonly Dictionary<string, string> gameModePaths = new Dictionary<string, string>
        {
            { "forest", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Forest, Tree Exit" },
            { "city", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City Front" },
            { "canyons", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Canyon" },
            { "mountains", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Mountain For Computer" },
            { "beach", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Beach from Forest" },
            { "sky", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Clouds" },
            { "basement", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Basement For Computer" },
            { "metro", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Metropolis from Computer" },
            { "arcade", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - City frm Arcade" },
            { "rotating", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Rotating Map" },
            { "bayou", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - BayouComputer2" },
            { "caves", "Environment Objects/TriggerZones_Prefab/JoinRoomTriggers_Prefab/JoinPublicRoom - Cave" }
        };

        public static string DetectCurrentMap()
        {
            foreach (var entry in gameModePaths)
            {
                if (GameObject.Find(entry.Value) != null) return entry.Key;
            }
            return null;
        }

        public static string GetPathForGameMode(string gameMode)
        {
            gameModePaths.TryGetValue(gameMode.ToLower(), out string path);
            return path;
        }

        public static void IsModded()
        {
            if (!PhotonNetwork.IsConnected)
            {
                NotificationLib.SendNotification("<color=blue>Room</color> : You are not connected to a room.");
                return;
            }

            string message = GetHTMode().Contains("MODDED") ? "<color=blue>Room</color> : You are in a modded lobby." : "<color=blue>Room</color> : You are not in a modded lobby.";
            NotificationLib.SendNotification(message);
        }

        public static string GetHTMode()
        {
            if (PhotonNetwork.CurrentRoom == null || !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gameMode"))
            {
                return "ERROR";
            }
            return PhotonNetwork.CurrentRoom.CustomProperties["gameMode"].ToString();
        }

    }
}
