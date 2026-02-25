using BepInEx;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace Violet.Utilities
{
    public class RigManager
    {
        public static VRRig GetVRRigFromPlayer(NetPlayer p)
        {
            return GorillaGameManager.instance.FindPlayerVRRig(p);
        }
        public static Player NetPlayerToPlayer(NetPlayer p)
        {
            return p.GetPlayerRef();
        }
        public static VRRig GetRandomVRRig(bool includeSelf)
        {
            VRRig random = GorillaParent.instance.vrrigs[UnityEngine.Random.Range(0, GorillaParent.instance.vrrigs.Count - 1)];
            if (includeSelf)
            {
                return random;
            }
            else
            {
                if (random != GorillaTagger.Instance.offlineVRRig)
                {
                    return random;
                }
                else
                {
                    return GetRandomVRRig(includeSelf);
                }
            }
        }

        public static NetworkView GetNetworkViewFromVRRig(VRRig p)
        {
            return (NetworkView)Traverse.Create(p).Field("netView").GetValue();
        }

        public static int[] GetActorNumbersFromPhotonViews()
        {
            List<int> actorNumbers = new List<int>();

            foreach (var kvp in PhotonNetwork.photonViewList)
            {
                var view = kvp.Value;
                if (view != null && view.Owner != null)
                {
                    int actorNumber = view.Owner.ActorNumber;
                    if (actorNumber != -1 && !actorNumbers.Contains(actorNumber)) 
                    {
                        actorNumbers.Add(actorNumber);
                    }
                }
            }

            return actorNumbers.ToArray();
        }

        public static PhotonView GetPhotonViewFromVRRig(VRRig p)
        {
            NetworkView view = Traverse.Create(p).Field("netView").GetValue() as NetworkView;
            return RigManager.NetView2PhotonView(view);
        }
        public static PhotonView NetView2PhotonView(NetworkView view)
        {
            PhotonView result;
            if (view == null)
            {
                Debug.Log("null netview passed to converter");
                result = null;
            }
            else
            {
                result = view.GetView;
            }
            return result;

        }
        public static VRRig GetOwnVRRig()
        {
            return GorillaTagger.Instance.offlineVRRig;
        }
        public static NetPlayer GetNetPlayerFromVRRig(VRRig p)
        {
            return RigManager.ToNetPlayer(RigManager.GetPhotonViewFromVRRig(p).Owner);
        }
        public static NetPlayer ToNetPlayer(Player player)
        {
            foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
            {
                if (netPlayer.GetPlayerRef() == player)
                {
                    return netPlayer;
                }
            }
            return null;
        }
        public static VRRig GetClosestVRRig()
        {
            float num = float.MaxValue;
            VRRig outRig = null;
            foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
            {
                if (Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position) < num)
                {
                    num = Vector3.Distance(GorillaTagger.Instance.bodyCollider.transform.position, vrrig.transform.position);
                    outRig = vrrig;
                }
            }
            return outRig;
        }



        public static Photon.Realtime.Player GetRandomPlayer(bool includeSelf)
        {
            if (includeSelf)
            {
                return PhotonNetwork.PlayerList[UnityEngine.Random.Range(0, PhotonNetwork.PlayerList.Length - 1)];
            }
            else
            {
                return PhotonNetwork.PlayerListOthers[UnityEngine.Random.Range(0, PhotonNetwork.PlayerListOthers.Length - 1)];
            }
        }

        public static Photon.Realtime.Player GetPlayerFromVRRig(VRRig p)
        {
            return GetPhotonViewFromVRRig(p).Owner;
        }

        public static Photon.Realtime.Player GetPlayerFromID(string id)
        {
            Photon.Realtime.Player found = null;
            foreach (Photon.Realtime.Player target in PhotonNetwork.PlayerList)
            {
                if (target.UserId == id)
                {
                    found = target;
                    break;
                }
            }
            return found;
        }
    }
}