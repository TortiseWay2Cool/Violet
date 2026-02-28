using GorillaNetworking;
using Photon.Pun;
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
    }
}
