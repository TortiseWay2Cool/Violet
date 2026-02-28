using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Violet.Menu;
using Violet.Utilities;

namespace VioletPaid.Utilities
{
    internal class Tools
    {
        public static float delay;
        public static float Delays;
        public static void KawaiiRPC()
        {
            Hashtable hashtable = new Hashtable();
            hashtable[0] = GorillaTagger.Instance.myVRRig.ViewID;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(200, hashtable, new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                TargetActors = new int[]
                {
                    PhotonNetwork.LocalPlayer.ActorNumber
                }
            }, SendOptions.SendReliable);
        }

        public static void AutoFlushRPCS()
        {
            if (PhotonNetwork.InRoom)
            {
                if (Time.time > Tools.delay)
                {
                    Tools.delay = Time.time + 0.1f;
                    Tools.KawaiiRPC();
                }
            }
        }

        public static bool Delay(float delayAmount)
        {
            if (Time.time > Tools.Delays)
            {
                Tools.Delays = Time.time + delayAmount;
                return true;
            }
            return false;
        }

        public static bool GetGamemode(string gamemodeName)
        {
            return GorillaGameManager.instance.GameModeName().ToLower().Contains(gamemodeName.ToLower());
        }

        public static bool IsOtherPlayer(VRRig rig) => rig != null && rig != GorillaTagger.Instance.offlineVRRig && !rig.isOfflineVRRig && !rig.isMyPlayer;

        public static bool IAmInfected => GorillaTagger.Instance.offlineVRRig != null && RigIsInfected(GorillaTagger.Instance.offlineVRRig);

        public static bool RigIsInfected(VRRig vrrig)
        {
            string materialName = vrrig.mainSkin.material.name;
            return materialName.Contains("fected") || materialName.Contains("It");
        }

        public static void IsMasterCheck()
        {
            ButtonHandler.ChangeButtonText("Check If Master", PhotonNetwork.IsMasterClient ? "Check If Master [<color=green>Master</color>]" : "Check If Master [<color=red>Not Master</color>]");
        }

        public static bool InLobby() => PhotonNetwork.InLobby;
        public static bool IsMaster() => PhotonNetwork.IsMasterClient;


        public static void ChangeBoardMaterial(string parentPath, int targetIndex, Material newMaterial, ref Material originalMat)
        {
            GameObject parent = GameObject.Find(parentPath);
            int currentIndex = 0;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject childObj = parent.transform.GetChild(i).gameObject;
                if (childObj.name.Contains("UnityTempFile"))
                {
                    currentIndex++;
                    if (currentIndex == targetIndex)
                    {
                        Renderer renderer = childObj.GetComponent<Renderer>();
                        if (originalMat == null)
                            originalMat = renderer.material;

                        renderer.material = newMaterial;
                        break;
                    }
                }
            }
        }

        public static void SendRpc(ReceiverGroup group, PhotonView view, string methodName, params object[] parameters)
        {
            if (view == null || string.IsNullOrEmpty(methodName))
            {
                Debug.LogError("Invalid PhotonView or method name.");
                return;
            }
            Hashtable yuh = new Hashtable();
            yuh[0] = view.ViewID;
            yuh[2] = PhotonNetwork.ServerTimestamp;
            yuh[3] = methodName;
            yuh[4] = parameters;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(200, yuh, new RaiseEventOptions { Receivers = group }, SendOptions.SendReliable);
        }

        public static void SendRpc(Photon.Realtime.Player plr, PhotonView view, string methodName, params object[] parameters)
        {
            if (view == null || string.IsNullOrEmpty(methodName))
            {
                Debug.LogError("Invalid PhotonView or method name.");
                return;
            }
            Hashtable yuh = new Hashtable();
            yuh[0] = view.ViewID;
            yuh[2] = PhotonNetwork.ServerTimestamp;
            yuh[3] = methodName;
            yuh[4] = parameters;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(200, yuh, new RaiseEventOptions { TargetActors = new int[] { plr.ActorNumber } }, SendOptions.SendReliable);
        }

        public static void NetInstantiate(bool Gamemode, PhotonView view, object data, Vector3 Possition, Quaternion Rotation)
        {
            Hashtable yuh = new Hashtable();
            if (Gamemode) { yuh[0] = "GameMode"; }
            else { yuh[0] = "Player Network Controller"; }
            yuh[6] = PhotonNetwork.ServerTimestamp;
            yuh[5] = data;
            yuh[4] = new int[]
            {
                view.ViewID,
            };
            yuh[1] = Possition;
            yuh[2] = Rotation;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(202, yuh, new RaiseEventOptions { Receivers = ReceiverGroup.Others, }, SendOptions.SendReliable);
        }

        public static void NetInstantiate(Photon.Realtime.Player plr, bool Gamemode, PhotonView view, object data, Vector3 Possition, Quaternion Rotation)
        {
            Hashtable yuh = new Hashtable();
            if (Gamemode) { yuh[0] = "GameMode"; }
            else { yuh[0] = "Player Network Controller"; }
            yuh[6] = PhotonNetwork.ServerTimestamp;
            yuh[5] = data;
            yuh[4] = new int[]
            {
                view.ViewID,
                view.ViewID,
                view.ViewID,
                view.ViewID
            };
            yuh[1] = Possition;
            yuh[2] = Rotation;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(202, yuh, new RaiseEventOptions { TargetActors = new int[] { plr.ActorNumber }, }, SendOptions.SendReliable);
        }

        public static void RemoveInstantiate(PhotonView view)
        {
            Hashtable yuh = new Hashtable();
            yuh[0] = view.ViewID;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(204, yuh, new RaiseEventOptions { Receivers = ReceiverGroup.Others, }, SendOptions.SendReliable);
        }

        public static void RemoveInstantiate(Photon.Realtime.Player plr, PhotonView view)
        {
            Hashtable yuh = new Hashtable();
            yuh[0] = view.ViewID;
            PhotonNetwork.NetworkingClient.OpRaiseEvent(204, yuh, new RaiseEventOptions { Receivers = ReceiverGroup.Others, }, SendOptions.SendReliable);
        }

    }
}
