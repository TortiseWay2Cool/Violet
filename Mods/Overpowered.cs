using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Violet.Utilities;
using VioletPaid.Utilities;
using static Violet.Mods.Advantage;
namespace Violet.Mods
{
    class Overpowered
    {
        public static void ScitzoGun()
        {
            GunTemplate.StartBothGuns(() =>
            {
                if (PhotonNetwork.InRoom)
                {
                    if (GunTemplate.lockedPlayer != null)
                    {
                        RunViewUpdatePatch.SerilizeData = () =>
                        {
                            VRRig.LocalRig.transform.position = new Vector3(0,999,0);

                            SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                            {
                                TargetActors = new int[] { GunTemplate.lockedPlayer.creator.ActorNumber},
                            });

                            return false;
                        };
                    }
                }
            }, true);
            if (GunTemplate.lockedPlayer == null)
            {
                RunViewUpdatePatch.SerilizeData = () =>
                {
                    VRRig.LocalRig.transform.position = GorillaTagger.Instance.transform.position;
                    SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.All,
                    });
                    return false;
                };
            }
        }

        public static void ReverseScitzoGun()
        {
            GunTemplate.StartBothGuns(() =>
            {
                if (PhotonNetwork.InRoom)
                {
                    foreach (Player plr in PhotonNetwork.PlayerListOthers)
                    {
                        if (plr != RigManager.GetPlayerFromVRRig(GunTemplate.lockedPlayer))
                        {
                            RunViewUpdatePatch.SerilizeData = () =>
                            {
                                VRRig.LocalRig.transform.position = new Vector3(0, 999, 0);
                                SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                                {
                                    TargetActors = new int[] { plr.ActorNumber },
                                });

                                return false;
                            };
                        }
                    }
                }
            }, true);
            if (GunTemplate.lockedPlayer == null)
            {
                RunViewUpdatePatch.SerilizeData = () =>
                {
                    VRRig.LocalRig.transform.position = GorillaTagger.Instance.transform.position;
                    SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                    {
                        Receivers = ReceiverGroup.All,
                    });
                    return false;
                };
            }
        }


        public static void Lag(int Index)  
        {
            if (Index == 0)
            {
                Hashtable c = new Hashtable();
                c[0] = "GameMode";
                c[5] = new int[] { UnityEngine.Random.Range(int.MinValue, int.MaxValue) };
                c[6] = PhotonNetwork.ServerTimestamp;
                c[7] = 2;
                if (Time.time > Variables.Delay)
                {
                    Variables.Delay = Time.time + 1.2f;
                    for (int i = 0; i < 520; i++)
                    {
                        PhotonNetwork.CurrentRoom.LoadBalancingClient.OpRaiseEvent(202, c, new RaiseEventOptions()
                        {
                            Receivers = ReceiverGroup.Others,
                            CachingOption = EventCaching.AddToRoomCacheGlobal
                        }, SendOptions.SendUnreliable);

                    }
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOutgoingCommands();
                    PhotonNetwork.SendAllOutgoingCommands();
                }
            }
            
        }
    }
}
