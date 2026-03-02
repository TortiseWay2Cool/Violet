using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Violet.Menu.Utilities;
using Violet.Utilities;
using Random = UnityEngine.Random;

namespace Violet.Mods
{
    class Players
    {
        public static void StumpKickAll()
        {
            GorillaComputer.instance.OnGroupJoinButtonPress(0, GorillaComputer.instance.friendJoinCollider);
            GorillaTagger.Instance.offlineVRRig.StartCoroutine(Rejoin());
        }

        public static void StumpKickGun()
        {
            GunLib.MakeGun(true,() =>
            {
                if (NetworkSystem.Instance.SessionIsPrivate)
                {
                    LastRoom = PhotonNetwork.CurrentRoom.Name;
                    PhotonNetworkController.Instance.shuffler = Random.Range(0, 99).ToString().PadLeft(2, '0') + Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                    PhotonNetworkController.Instance.keyStr = Random.Range(0, 99999999).ToString().PadLeft(8, '0');
                    GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Add(GunLib.LockedRig.creator.UserId);
                    RoomSystem.SendEvent(4, new object[]
                    {
                        PhotonNetworkController.Instance.shuffler,
                        PhotonNetworkController.Instance.keyStr
                    }, GunLib.LockedRig.creator, true);

                    PhotonNetwork.SendAllOutgoingCommands();
                    PhotonNetworkController.Instance.AttemptToJoinPublicRoom(GorillaComputer.instance.GetSelectedMapJoinTrigger(), JoinType.JoinWithNearby, null, false);
                    GorillaTagger.Instance.offlineVRRig.StartCoroutine(Rejoin());
                }
            });
        }

        public static IEnumerator Rejoin()
        {
            yield return new WaitForSeconds(4f);
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(LastRoom, GorillaNetworking.JoinType.Solo);

        }

        public static string LastRoom;
        public static VRRig FakeRigger;
        public static void FakeRig() // Lwk dont work
        {
            if (GorillaTagger.Instance.offlineVRRig.enabled == false)
            {
                if (FakeRigger == null)
                    FakeRigger = GameObject.Instantiate(GorillaTagger.Instance.offlineVRRig.gameObject, GorillaTagger.Instance.offlineVRRig.transform.position, GorillaTagger.Instance.offlineVRRig.transform.rotation).GetComponent<VRRig>();
                FakeRigger.mainSkin.material = new Material(Shader.Find("GUI/Text Shader"));
                FakeRigger.mainSkin.material.color = Color.darkViolet;
                FakeRigger.headConstraint.transform.position = GorillaTagger.Instance.headCollider.transform.position;
                FakeRigger.headConstraint.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;

                FakeRigger.leftHandTransform.position = GorillaTagger.Instance.leftHandTransform.position;
                FakeRigger.leftHandTransform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;

                FakeRigger.rightHandTransform.position = GorillaTagger.Instance.rightHandTransform.position;
                FakeRigger.rightHandTransform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;

                FakeRigger.transform.position = GorillaTagger.Instance.transform.position;
                FakeRigger.transform.rotation = GorillaTagger.Instance.transform.rotation;
            }
            else
            {
                if (FakeRigger != null)
                    GameObject.Destroy(FakeRigger.gameObject);
            }
        }

        public static void Invis(bool enabled)
        {
            GorillaTagger.Instance.offlineVRRig.enabled = enabled;
            //FakeRig();
            if (!enabled)
                GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(0, -999, 0);
        }

        public static void Ghost(bool enabled)
        {
            GorillaTagger.Instance.offlineVRRig.enabled = enabled;
        }

        public static void RPInvis(bool enabled)
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = enabled;
                //FakeRig();
                if (!enabled)
                    GorillaTagger.Instance.offlineVRRig.transform.position = new Vector3(0, -999, 0);
            }
        }

        public static void LPGhost(bool enabled)
        {
            if (ControllerInputPoller.instance.leftControllerPrimaryButton)
                GorillaTagger.Instance.offlineVRRig.enabled = enabled;
        }

        public static void ScareGun()
        {
            GunLib.MakeGun(true, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = GunLib.LockedRig.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
            }, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            });

        }

        public static void ScareClosest()
        {
            VRRig rig = RigManager.GetClosestVRRig();
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
        }

        public static void CopyMovementGun()
        {
            GunLib.MakeGun(true, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = GunLib.LockedRig.transform.position;
                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = GunLib.LockedRig.rightHand.rigTarget.transform.position;
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = GunLib.LockedRig.leftHand.rigTarget.transform.position;
            }, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            });
        }

        public static void CopyMovementClosest()
        {
            VRRig rig = RigManager.GetClosestVRRig();
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            GorillaTagger.Instance.offlineVRRig.transform.position = rig.transform.position;
            GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position = rig.rightHand.rigTarget.transform.position;
            GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position = rig.leftHand.rigTarget.transform.position;
        }

        public static void FollowGun()
        {
            GunLib.MakeGun(true, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                VRRig.LocalRig.transform.position = GunLib.LockedRig.transform.position;
            }, () =>
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            });
        }

        public static void FollowClosest()
        {
            VRRig rig = RigManager.GetClosestVRRig();
            GorillaTagger.Instance.offlineVRRig.enabled = false;
            VRRig.LocalRig.transform.position = rig.transform.position;
        }



        public static void FollowAll()
        {
            foreach (Player plr in PhotonNetwork.PlayerListOthers)
            {
                Advantage.RunViewUpdatePatch.SerilizeData = () =>
                {
                    VRRig.LocalRig.transform.position = RigManager.GetVRRigFromPlayer(plr).transform.position;
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

        public static void CopyMovementAll()
        {
            foreach (Player plr in PhotonNetwork.PlayerListOthers)
            {
                Advantage.RunViewUpdatePatch.SerilizeData = () =>
                {
                    VRRig.LocalRig.transform.position = RigManager.GetVRRigFromPlayer(plr).transform.position;
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = RigManager.GetVRRigFromPlayer(plr).rightHand.rigTarget.transform.position;
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = RigManager.GetVRRigFromPlayer(plr).leftHand.rigTarget.transform.position;
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

        public static void ScareAll()
        {
            foreach (Player plr in PhotonNetwork.PlayerListOthers)
            {
                Advantage.RunViewUpdatePatch.SerilizeData = () =>
                {
                    VRRig.LocalRig.transform.position = RigManager.GetVRRigFromPlayer(plr).transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
                    VRRig.LocalRig.rightHand.rigTarget.transform.position = RigManager.GetVRRigFromPlayer(plr).transform.position;
                    VRRig.LocalRig.leftHand.rigTarget.transform.position = RigManager.GetVRRigFromPlayer(plr).transform.position;

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
