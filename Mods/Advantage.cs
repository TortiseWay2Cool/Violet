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

        public static void ResetPlayer()
        {
            RunViewUpdatePatch.SerilizeData = () =>
            {
                return true;
            };
        }
        #endregion


        public static void TagGun()
        {
            GunLib.MakeGun(true, () =>
            {
                if (PhotonNetwork.InRoom)
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        GorillaTagManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                        if (PhotonNetwork.CurrentRoom.PlayerCount < 4)
                        {
                            component.ChangeCurrentIt(GunLib.LockedRig.creator, true);
                        }
                        else
                        {
                            component.AddInfectedPlayer(GunLib.LockedRig.creator, true);
                        }
                    }
                    else
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
                }
                
            });
        }

        public static void TagAll()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    foreach (Player plr in PhotonNetwork.PlayerList)
                    {
                        GorillaTagManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                        if (PhotonNetwork.CurrentRoom.PlayerCount < 4)
                        {
                            component.ChangeCurrentIt(plr, true);
                        }
                        else
                        {
                            component.AddInfectedPlayer(plr, true);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
                    {
                        Player plr = PhotonNetwork.PlayerListOthers[i];

                        VRRig.LocalRig.transform.position = RigManager.GetVRRigFromPlayer(plr).transform.position;

                        GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetPhotonView().RPC("RPC_ReportTag", RpcTarget.MasterClient, new object[] { plr.ActorNumber });
                        SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                        {
                            TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber }
                        });
                        VRRig.LocalRig.transform.position = GorillaTagger.Instance.transform.position;

                    }
                }
            }
        }

        public static void TagSelf()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GorillaTagManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                    if (PhotonNetwork.CurrentRoom.PlayerCount < 4)
                    {
                        component.ChangeCurrentIt(PhotonNetwork.LocalPlayer, true);
                    }
                    else
                    {
                        component.AddInfectedPlayer(PhotonNetwork.LocalPlayer, true);
                    }
                }
                else
                {
                    Player[] playerListOthers = PhotonNetwork.PlayerListOthers;
                    for (int i = 0; i < playerListOthers.Length; i++)
                    {
                        Player plr = playerListOthers[i];
                        if (plr != PhotonNetwork.LocalPlayer)
                        {
                            Advantage.RunViewUpdatePatch.SerilizeData = delegate ()
                            {
                                VRRig.LocalRig.transform.position = RigManager.GetVRRigFromPlayer(plr).transform.position;
                                PunExtensions.GetPhotonView(GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)")).RPC("RPC_ReportTag", RpcTarget.MasterClient, new object[]
                                {
                                    PhotonNetwork.LocalPlayer.ActorNumber
                                });
                                Advantage.SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                                {
                                    TargetActors = new int[]
                                    {
                                        plr.ActorNumber
                                    }
                                });
                                return false;
                            };
                        }
                    }
                }
            }
        }

        public static void AntiTag()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GorillaTagManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                    if (PhotonNetwork.CurrentRoom.PlayerCount < 4)
                    {
                        component.currentIt = null;
                    }
                    else
                    {
                        component.currentInfected.Remove(PhotonNetwork.LocalPlayer);
                    }
                }
                else
                {
                    for (int i = 0; i < PhotonNetwork.PlayerListOthers.Length; i++)
                    {
                        if (PhotonNetwork.PlayerListOthers[i] != PhotonNetwork.LocalPlayer)
                        {
                            if (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, RigManager.GetVRRigFromPlayer(PhotonNetwork.PlayerListOthers[i]).headMesh.transform.position) <= 3f)
                            {
                                if (!Tools.IAmInfected && Tools.RigIsInfected(RigManager.GetVRRigFromPlayer(PhotonNetwork.PlayerListOthers[i])))
                                {
                                    Advantage.RunViewUpdatePatch.SerilizeData = delegate ()
                                    {
                                        VRRig.LocalRig.transform.position = GorillaTagger.Instance.transform.position + new Vector3(0f, -8f, 0f);
                                        Advantage.SerializeUpdate(GorillaTagger.Instance.myVRRig.punView, new RaiseEventOptions
                                        {
                                            TargetActors = new int[]
                                            {
                                                PhotonNetwork.PlayerListOthers[i].ActorNumber
                                            }
                                        });
                                        return false;
                                    };
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void TagBot()
        {
            if (PhotonNetwork.InRoom)
            {
                if (Tools.IAmInfected)
                {
                    Advantage.TagAll();
                }
                else
                {
                    Advantage.TagSelf();
                }
            }
        }

        public static void MatGun()
        {
            GunLib.MakeGun(true, () =>
            {
                Advantage.MatPlayer(RigManager.GetPlayerFromVRRig(GunLib.LockedRig));
            });
        }

        public static void MatAll()
        {
            if (PhotonNetwork.InRoom)
            {
                foreach (Player plr in PhotonNetwork.PlayerList)
                {
                    Advantage.MatPlayer(plr);
                }
            }
        }

        public static void MatSelf()
        {
            if (PhotonNetwork.InRoom)
            {
                Advantage.MatPlayer(PhotonNetwork.LocalPlayer);
            }
        }

        public static void MatPlayer(Player plr)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (Time.time > Variables.Delay)
                {
                    Variables.Delay = Time.time + 0.05f;
                    GorillaTagManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                    if (component.currentInfected.Contains(plr))
                    {
                        component.currentInfected.Remove(plr);
                    }
                    else
                    {
                        component.AddInfectedPlayer(plr, false);
                    }
                }
            }
        }

        public static void RestartGameMode()
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GorillaTagManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>();
                    component.currentIt = null;
                    component.currentInfected.Clear();
                }
            }
        }

        public static void SetInfectionThreshold(int Threshold)
        {
            if (PhotonNetwork.InRoom)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    GorillaGameManager.instance.gameObject.GetComponent<GorillaTagManager>().infectedModeThreshold = Threshold;
                }
            }
        }

        public static void BrawlKillAll()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    GorillaGameManager.instance.HitPlayer(vrrig.creator);
                }
            }
        }

        public static void BrawlKillGun()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GunLib.MakeGun(true, () =>
                {
                    GorillaGameManager.instance.HitPlayer(GunLib.LockedRig.creator);
                });
            }
        }

        public static void BrawlInfLives(bool enable)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>().playerLives[PhotonNetwork.LocalPlayer.ActorNumber] = 9999;
            }
        }

        public static void BrawlGiveInfLives()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GunLib.MakeGun(true, () =>
                {
                    GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>().playerLives[GunLib.LockedRig.creator.ActorNumber] = 9999;
                });
            }
        }

        public static void BrawlReviveAll()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaPaintbrawlManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
                foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
                {
                    if (component.playerLives[vrrig.creator.ActorNumber] > 0)
                    {
                        component.playerLives[vrrig.creator.ActorNumber] = 1;
                    }
                }
            }
        }

        public static void BrawlReviveGun()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaPaintbrawlManager gpm = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
                GunLib.MakeGun(true, () =>
                {
                    gpm.playerLives[GunLib.LockedRig.creator.ActorNumber] = 1;
                });
            }
        }

        public static void BrawlRestart()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaPaintbrawlManager component = GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>();
                component.BattleEnd();
                component.StartBattle();
            }
        }

        public static void BrawlTeamBattle()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GorillaGameManager.instance.gameObject.GetComponent<GorillaPaintbrawlManager>().teamBattle = true;
            }
        }

        public static void SlowPlayer(object[] stuff)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (stuff[0] is Player)
                {
                    RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, (NetPlayer)stuff[0]);
                }
                else
                {
                    RoomSystem.SendStatusEffectAll(RoomSystem.StatusEffects.SetSlowedTime);
                }
            }
        }

        public static void SlowGun()
        {
            GunLib.MakeGun(true, ()=>
            {
                Advantage.SlowPlayer(new object[]
                {
                    GunLib.LockedRig.creator
                });
            });
        }

        public static void SlowAll()
        {
            Advantage.SlowPlayer(new object[0]);
        }

        public static void VibratePlayer(object[] stuff)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (stuff[0] is Player)
                {
                    RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, (NetPlayer)stuff[0]);
                }
                else
                {
                    RoomSystem.SendStatusEffectAll(RoomSystem.StatusEffects.JoinedTaggedTime);
                }
            }
        }

        public static void VibrateGun()
        {
            GunLib.MakeGun(true, delegate
            {
                Advantage.VibratePlayer(new object[]
                {
                    GunLib.LockedRig.creator
                });
            });
        }

        public static void VibrateAll()
        {
            Advantage.VibratePlayer(new object[0]);
        }
    }
}