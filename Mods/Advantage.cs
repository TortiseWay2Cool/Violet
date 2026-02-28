using ExitGames.Client.Photon;
using Fusion;
using GorillaGameModes;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using Violet.Menu.Utilities;
using Violet.Utilities;
using VioletPaid.Utilities;

namespace Violet.Mods
{
    class Advantage
    {
        #region Serialize


        [HarmonyPatch(typeof(PhotonNetwork), "RunViewUpdate")]
        public class RunViewUpdatePatch // lyfes patch i had a horrible manually setting method before this 
        {
            public static Func<bool> SerilizeData;

            public static bool Prefix()
            {
                if (!PhotonNetwork.InRoom)
                {
                    return true;
                }


                if (SerilizeData == null)
                {
                    return true;
                }


                return SerilizeData.Invoke();
            }
        }


        public static void SerializeUpdate(PhotonView View, RaiseEventOptions Options, int customTimestamp = -1)
        {
            if (!PhotonNetwork.InRoom)
                return;

            var batch = new PhotonNetwork.RaiseEventBatch
            {
                Reliable = false,
                Group = View.Group
            };

            var dataList = PhotonNetwork.OnSerializeWrite(View);

            PhotonNetwork.RaiseEventBatch raiseEventBatch = new PhotonNetwork.RaiseEventBatch();
            IDictionary dictionary = PhotonNetwork.serializeViewBatches;
            var viewBatch = new PhotonNetwork.SerializeViewBatch(batch, 2);

            if (!dictionary.Contains(raiseEventBatch))
                dictionary[raiseEventBatch] = viewBatch;


            viewBatch.Add(dataList);

            viewBatch.ObjectUpdates[0] = PhotonNetwork.ServerTimestamp;

            viewBatch.ObjectUpdates[1] = PhotonNetwork.currentLevelPrefix != 0 ? (byte?)PhotonNetwork.currentLevelPrefix : null;


            PhotonNetwork.RaiseEventInternal((byte)206, viewBatch.ObjectUpdates, Options, SendOptions.SendUnreliable);

            viewBatch.Clear();
        }

        public static float ReturnNotKickableAmount()
        {
            if (PhotonNetwork.InRoom)
            {
                return 0.014f * PhotonNetwork.CurrentRoom.PlayerCount;
            }
            else
            {
                return 0;
            }
        }
        #endregion


        public static void TagGun()
        {
            GunLib.MakeGun(true, () =>
            {
                if (PhotonNetwork.InRoom)
                {
                    if (!Tools.RigIsInfected(GunLib.LockedRig))
                    {
                        VRRig.LocalRig.transform.position = GunLib.LockedRig.transform.position;

                        GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView().RPC("RPC_ReportTag", RpcTarget.MasterClient, new object[] { GunLib.LockedRig.creator.ActorNumber });
                        SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                        {
                            TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber }
                        });
                        VRRig.LocalRig.transform.position = GorillaTagger.Instance.transform.position;
                    }

                }
            });
        }

        public static void TagAll()
        {
            if (PhotonNetwork.InRoom)
            {
                for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
                {
                    Player plr = PhotonNetwork.PlayerListOthers[i];

                    VRRig.LocalRig.transform.position = GunLib.LockedRig.transform.position;

                    GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView().RPC("RPC_ReportTag", RpcTarget.MasterClient, new object[] { GunLib.LockedRig.creator.ActorNumber });
                    SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                    {
                        TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber }
                    });
                    VRRig.LocalRig.transform.position = GorillaTagger.Instance.transform.position;

                }
            }


        }

        public static void TagSelf()
        {
            if (PhotonNetwork.InRoom)
            {
                foreach (Player plr in PhotonNetwork.PlayerListOthers)
                {
                    if (plr != PhotonNetwork.LocalPlayer)
                    {
                        RunViewUpdatePatch.SerilizeData = () =>
                        {
                            VRRig.LocalRig.transform.position = RigManager.GetVRRigFromPlayer(plr).transform.position;
                            GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView().RPC("RPC_ReportTag", RpcTarget.MasterClient, new object[] { PhotonNetwork.LocalPlayer.ActorNumber });
                            SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                            {
                                TargetActors = new int[] { plr.ActorNumber }
                            });
                            return false;
                        };
                    }
                }
            }

        }
    }
}