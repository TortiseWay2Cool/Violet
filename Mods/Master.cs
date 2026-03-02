using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Violet.Menu.Utilities;
using Violet.Utilities;
using static Violet.Mods.Advantage;
using static Violet.Mods.Settings;
namespace Violet.Mods
{
    class Master
    {
        public static void GravityAll()
        {
            if (GravityFactor == 0)
                GreyZoneManager.Instance.greyZoneActive = false;
            else 
                GreyZoneManager.Instance.greyZoneActive = true;
            GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
            GreyZoneManager.Instance.greyZoneActivationTime = (GreyZoneManager.Instance.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
            if (GravityFactor == 67)
            {
                if (GreyZoneManager.Instance.gravityFactorOptionSelection != int.MaxValue)
                    GreyZoneManager.Instance.gravityFactorOptionSelection = int.MaxValue;
                else
                    GreyZoneManager.Instance.gravityFactorOptionSelection = 0;
            }
            else
                GreyZoneManager.Instance.gravityFactorOptionSelection = GravityFactor;
            GreyZoneManager.Instance.ActivateGreyZoneLocal();
        }

        public static void GravityOthers()
        {
            RunViewUpdatePatch.SerilizeData = () =>
            {
                if (GravityFactor == 0)
                    GreyZoneManager.Instance.greyZoneActive = false;
                else
                    GreyZoneManager.Instance.greyZoneActive = true;
                GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
                GreyZoneManager.Instance.greyZoneActivationTime = PhotonNetwork.Time;
                if (GravityFactor == 67)
                {
                    if (GreyZoneManager.Instance.gravityFactorOptionSelection != int.MaxValue)
                        GreyZoneManager.Instance.gravityFactorOptionSelection = int.MaxValue;
                    else
                        GreyZoneManager.Instance.gravityFactorOptionSelection = 0;
                }
                else
                    GreyZoneManager.Instance.gravityFactorOptionSelection = GravityFactor;
                SerializeUpdate(GreyZoneManager.Instance.photonView, new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                });
                return false;
            };
        }

        public static void GravityGun()
        {
            GunLib.MakeGun(true, () =>
            {
                RunViewUpdatePatch.SerilizeData = () =>
                {
                    if (GravityFactor == 0)
                        GreyZoneManager.Instance.greyZoneActive = false;
                    else
                        GreyZoneManager.Instance.greyZoneActive = true;
                    GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
                    GreyZoneManager.Instance.greyZoneActivationTime = PhotonNetwork.Time;
                    if (GravityFactor == 67)
                    {
                        if (GreyZoneManager.Instance.gravityFactorOptionSelection != int.MaxValue)
                            GreyZoneManager.Instance.gravityFactorOptionSelection = int.MaxValue;
                        else
                            GreyZoneManager.Instance.gravityFactorOptionSelection = 0;
                    }
                    else
                        GreyZoneManager.Instance.gravityFactorOptionSelection = GravityFactor;
                    SerializeUpdate(GreyZoneManager.Instance.photonView, new RaiseEventOptions
                    {
                        TargetActors = new int[] { GunLib.LockedRig.creator.ActorNumber },
                    });
                    return false;
                };
            });
        }
        
        public static void GreyScreenAll()
        {
            GreyZoneManager.Instance.greyZoneActive = true;
            GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
            GreyZoneManager.Instance.greyZoneActivationTime = (GreyZoneManager.Instance.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
            GreyZoneManager.Instance.ActivateGreyZoneLocal();
        }

        public static void GreyScreenOthers()
        {
            RunViewUpdatePatch.SerilizeData = () =>
            {
                GreyZoneManager.Instance.greyZoneActive = true;
                GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
                GreyZoneManager.Instance.greyZoneActivationTime = (GreyZoneManager.Instance.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
                SerializeUpdate(GreyZoneManager.Instance.photonView, new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                });
                return false;
            };
        }

        public static void GreyScreenGun()
        {
            GunLib.MakeGun(true, () =>
            {
                RunViewUpdatePatch.SerilizeData = () =>
                {
                    GreyZoneManager.Instance.greyZoneActive = true;
                    GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
                    GreyZoneManager.Instance.greyZoneActivationTime = (GreyZoneManager.Instance.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
                    SerializeUpdate(GreyZoneManager.Instance.photonView, new RaiseEventOptions
                    {
                        TargetActors = new int[] { GunLib.LockedRig.creator.ActorNumber },
                    });
                    return false;
                };
            });
        }

        
    }
}
