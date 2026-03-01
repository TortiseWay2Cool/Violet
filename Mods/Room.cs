using Fusion.Photon.Realtime;
using GorillaGameModes;
using GorillaNetworking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Violet.Mods
{
    class Room
    {
        public static void SetGamemode(GameModeType type)
        {
            GorillaComputer.instance.SetGameModeWithoutButton(type.ToString());
        }
    }
}
