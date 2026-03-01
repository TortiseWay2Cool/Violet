using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Violet.Utilities;

namespace Violet.Mods
{
    class Visuals
    {
        public static void MasterChams()
        {
            if (PhotonNetwork.MasterClient.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                VRRig rig = RigManager.GetVRRigFromPlayer(PhotonNetwork.MasterClient);
                rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                rig.mainSkin.material.color = Color.darkViolet;
            }
            else
            {
                VRRig rig = RigManager.GetOwnVRRig();
                rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                rig.mainSkin.material.color = Color.green;
            }

        }
    }
}
