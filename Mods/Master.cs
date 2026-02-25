using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Violet.Utilities;
using static Violet.Mods.Advantage;

namespace Violet.Mods
{
    class Master
    {
        public static void ZeroGravityAll(bool Enabled)
        {
            GreyZoneManager.Instance.greyZoneActive = Enabled;
            GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
            GreyZoneManager.Instance.greyZoneActivationTime = (GreyZoneManager.Instance.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
            GreyZoneManager.Instance.gravityFactorOptionSelection = int.MaxValue;
            GreyZoneManager.Instance.ActivateGreyZoneLocal();
        }

        public static void ZeroGravityOthers()
        {
            RunViewUpdatePatch.SerilizeData = () =>
            {
                GreyZoneManager.Instance.greyZoneActive = true;
                GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
                GreyZoneManager.Instance.greyZoneActivationTime = PhotonNetwork.Time;
                GreyZoneManager.Instance.gravityFactorOptionSelection = int.MaxValue;
                SerializeUpdate(GreyZoneManager.Instance.photonView, new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                });
                return false;
            };
        }

        public static void ZeroGravityGun()
        {
            GunTemplate.StartBothGuns(() =>
            {
                RunViewUpdatePatch.SerilizeData = () =>
                {
                    GreyZoneManager.Instance.greyZoneActive = true;
                    GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
                    GreyZoneManager.Instance.greyZoneActivationTime = PhotonNetwork.Time;
                    GreyZoneManager.Instance.gravityFactorOptionSelection = int.MaxValue;
                    SerializeUpdate(GreyZoneManager.Instance.photonView, new RaiseEventOptions
                    {
                        TargetActors = new int[] { GunTemplate.lockedPlayer.creator.ActorNumber },
                    });
                    return false;
                };
            }, true);
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
            GunTemplate.StartBothGuns(() =>
            {
                RunViewUpdatePatch.SerilizeData = () =>
                {
                    GreyZoneManager.Instance.greyZoneActive = true;
                    GreyZoneManager.Instance.photonConnectedDuringActivation = PhotonNetwork.InRoom;
                    GreyZoneManager.Instance.greyZoneActivationTime = (GreyZoneManager.Instance.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
                    SerializeUpdate(GreyZoneManager.Instance.photonView, new RaiseEventOptions
                    {
                        TargetActors = new int[] { GunTemplate.lockedPlayer.creator.ActorNumber },
                    });
                    return false;
                };
            }, true);
        }

        
    }
}
