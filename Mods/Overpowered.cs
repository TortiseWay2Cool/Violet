using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Violet.Menu.Utilities;
using Violet.Utilities;
using VioletPaid.Utilities;
using static Violet.Mods.Advantage;
namespace Violet.Mods
{
    class Overpowered
    {
        public static void ScitzoGun()
        {
            GunLib.MakeGun(true, () =>
            {
                if (PhotonNetwork.InRoom)
                {
                    if (GunLib.LockedRig != null)
                    {
                        RunViewUpdatePatch.SerilizeData = () =>
                        {
                            VRRig.LocalRig.transform.position = new Vector3(0, 999, 0);

                            SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                            {
                                TargetActors = new int[] { GunLib.LockedRig.creator.ActorNumber },
                            });

                            return false;
                        };
                    }
                }
            });
            if (GunLib.LockedRig == null)
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
            GunLib.MakeGun(true, () =>
            {
                if (PhotonNetwork.InRoom)
                {
                    foreach (Player plr in PhotonNetwork.PlayerListOthers)
                    {
                        if (plr != RigManager.GetPlayerFromVRRig(GunLib.LockedRig))
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
            });
            if (GunLib.LockedRig == null)
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
        public static void KickMaster()
        {
            Hashtable hashtable = new Hashtable();
            hashtable[0] = "GameMode";
            hashtable[4] = new int[]
            {
                PhotonNetwork.AllocateViewID(0)
            };
            hashtable[6] = PhotonNetwork.ServerTimestamp;
            hashtable[7] = PhotonNetwork.AllocateViewID(0);

            for (int i = 0; i < 3; i++)
            {
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpRaiseEvent(202, hashtable, new RaiseEventOptions
                {
                    d
                    TargetActors = new int[]
                    {
                        PhotonNetwork.MasterClient.ActorNumber
                    },
                    CachingOption = 0
                }, SendOptions.SendUnreliable);
            }

            PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOutgoingCommands();
            PhotonNetwork.SendAllOutgoingCommands();
            Tools.AutoFlushRPCS();
        }

        public static void KickMasterGun()
        {
            GunLib.MakeGun(true, () =>
            {
                if (PhotonNetwork.InRoom)
                {

                    Hashtable hashtable = new Hashtable();
                    hashtable[0] = "GameMode";
                    hashtable[4] = new int[]
                    {
                        PhotonNetwork.AllocateViewID(0)
                    };
                    hashtable[6] = PhotonNetwork.ServerTimestamp;
                    hashtable[7] = PhotonNetwork.AllocateViewID(0);

                    for (int i = 0; i < 3; i++)
                    {
                        PhotonNetwork.CurrentRoom.LoadBalancingClient.OpRaiseEvent(202, hashtable, new RaiseEventOptions
                        {
                            TargetActors = new int[]
                            {
                        PhotonNetwork.MasterClient.ActorNumber
                            },
                            CachingOption = 0
                        }, SendOptions.SendUnreliable);
                    }

                    PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOutgoingCommands();
                    PhotonNetwork.SendAllOutgoingCommands();
                    Tools.AutoFlushRPCS();
                }
            });
        }

        public static void SlowSetMaster()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Overpowered.KickMaster();
            }
        }

        public static void Lag2(int options)
        {
            Hashtable table = new Hashtable();
            table[0] = "GameMode";
            table[5] = new int[]
            {
                UnityEngine.Random.Range(int.MinValue, int.MaxValue)
            };
            table[6] = PhotonNetwork.ServerTimestamp;
            table[7] = 2;

            if (options == 0)
            {
                if (Tools.Delay(1.2f))
                {
                    for (int i = 0; i < 520; i++)
                    {
                        PhotonNetwork.CurrentRoom.LoadBalancingClient.OpRaiseEvent(202, table, new RaiseEventOptions
                        {
                            Receivers = 0
                        }, SendOptions.SendUnreliable);
                    }
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOutgoingCommands();
                    PhotonNetwork.SendAllOutgoingCommands();
                }
                Tools.AutoFlushRPCS();
            }
            else if (options == 1)
            {
                GunLib.MakeGun(true, () =>
                {
                    if (PhotonNetwork.InRoom)
                    {
                        if (Tools.Delay(1.2f))
                        {
                            for (int j = 0; j < 520; j++)
                            {
                                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpRaiseEvent(202, table, new RaiseEventOptions
                                {
                                    TargetActors = new int[]
                                    {
                                        GunLib.LockedRig.creator.ActorNumber
                                    }
                                }, SendOptions.SendUnreliable);
                            }
                            PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOutgoingCommands();
                            PhotonNetwork.SendAllOutgoingCommands();
                        }
                        Tools.AutoFlushRPCS();
                    }
                });
            }
        }

        public static void DestroyGun()
        {
            GunLib.MakeGun(true, () =>
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetwork.OpRemoveCompleteCacheOfPlayer(GunLib.LockedRig.creator.ActorNumber);
                }
            });
        }

        public static void DestroyAll()
        {
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(player.ActorNumber);
            }
        }
    }
}
