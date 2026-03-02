using GorillaTag;
using Pathfinding.RVO;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using Violet.Menu;
using Violet.Utilities;
using static Valve.VR.InteractionSystem.Sample.CustomSkeletonHelper;
using static Violet.Utilities.Variables;
using static Violet.Menu.Main;
using static Violet.Utilities.ColorLib;
using static Violet.Menu.ButtonHandler;
using static Violet.Menu.Optimizations;
namespace Violet.Mods
{
    class Settings : MonoBehaviour
    {
        public void Awake()
        {
            ChangeGravityType();
        }

        #region Menu Settings

        public static void ChangeTheme()
        {
            Theme++;
            if (Theme > 5) Theme = 0;

            switch (Theme)
            {
                case 0:
                    SetDefaultTheme();
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Base]");
                    break;
                case 1:
                    MenuColor = SkyBlue;
                    ButtonColorOff = RoyalBlue;
                    ButtonColorOn = DodgerBlue;
                    outColor = DarkDodgerBlue;
                    DisconnectColor = Crimson;
                    AddonsColor = WineRed;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Blue]");
                    break;
                case 2:
                    MenuColor = FireBrick;
                    ButtonColorOff = WineRed;
                    ButtonColorOn = IndianRed;
                    outColor = IndianRed;
                    DisconnectColor = Crimson;
                    AddonsColor = WineRed;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Red]");
                    break;
                case 3:
                    MenuColor = new Color32(171, 129, 182, 255);
                    ButtonColorOff = Plum;
                    ButtonColorOn = MediumOrchid;
                    outColor = DarkSlateBlue;
                    DisconnectColor = ButtonColorOff;
                    AddonsColor = outColor;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Lavender]");
                    break;
                case 4:
                    MenuColor = MediumAquamarine;
                    ButtonColorOff = MediumSeaGreen;
                    ButtonColorOn = SeaGreen;
                    DisconnectColor = ButtonColorOff;
                    outColor = Lime;
                    AddonsColor = outColor;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Green]");
                    break;
                case 5:
                    MenuColor = Black;
                    ButtonColorOff = DarkerGrey;
                    ButtonColorOn = WineRed;
                    DisconnectColor = WineRed;
                    outColor = DarkDodgerBlue;
                    AddonsColor = Maroon;
                    ButtonHandler.ChangeButtonText("Current Menu Theme", "Current Menu Theme [Dark]");
                    break;
            }
            RefreshMenu();
        }

        private static void SetDefaultTheme()
        {
            MenuColor = Black;
            ButtonColorOff = ColorLib.Violet;
            ButtonColorOn = Indigo;
            DisconnectColor = violet;
            outColor = ColorLib.Violet;
            AddonsColor = WineRed;
            NotificationLib.SendNotification("<color=white>[</color><color=blue>Theme</color><color=white>] Violet/Default</color>");
            RefreshMenu();
        }

        public static void ChangeOutlineColor()
        {
            Theme++;
            if (Theme > 8) Theme = 1;

            switch (Theme)
            {
                case 1:
                    RainbowOutline = false;
                    outColor = Navy;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Blue]");
                    break;
                case 2:
                    RainbowOutline = false;
                    outColor = WineRed;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Red]");
                    break;
                case 3:
                    RainbowOutline = false;
                    outColor = Green;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Green]");
                    break;
                case 4:
                    RainbowOutline = false;
                    outColor = Black;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Dark]");
                    break;
                case 5:
                    RainbowOutline = false;
                    outColor = Purple;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Purple]");
                    break;
                case 6:
                    RainbowOutline = false;
                    outColor = Orange;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Orange]");
                    break;
                case 7:
                    RainbowOutline = false;
                    outColor = Yellow;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Yellow]");
                    break;
                case 8:
                    RainbowOutline = false;
                    outColor = Pink;
                    ButtonHandler.ChangeButtonText("Current Menu Outline", "Current Menu Outline [Pink]");
                    break;
            }
        }

        public static void ChangeSound()
        {
            SoundIndex++;
            if (SoundIndex > 6) SoundIndex = 1;

            ActualSound = SoundIndex switch
            {
                1 => 66,
                2 => 8,
                3 => 203,
                4 => 50,
                5 => 67,
                6 => 114,
                _ => 66
            };
        }

        public static void EnableGravity(bool enable)
        {
            gravity = enable;
        }
        #endregion

        #region Master Settings
        public static int GravityIndex = 1;
        public static int GravityFactor = 0;
        public static void ChangeGravityType()
        {
            GravityIndex++;
            if (GravityIndex > 3) GravityIndex = 1;
            switch (GravityIndex)
            {
                case 1:
                    GravityFactor = 0;
                    ButtonHandler.ChangeButtonText("Gravity Type", "Gravity Type [Gravity]");
                    break;
                case 2:
                    GravityFactor = 67;
                    ButtonHandler.ChangeButtonText("Gravity Type", "Gravity Type [Low]");
                    break;
                case 3:
                    GravityFactor = int.MaxValue;
                    ButtonHandler.ChangeButtonText("Gravity Type", "Gravity Type [Zero]");
                    break;
            }
        }

        #endregion
    }
}
