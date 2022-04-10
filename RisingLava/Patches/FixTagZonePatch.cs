﻿using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

namespace RisingLava.Patches
{
    [HarmonyPatch(typeof(GorillaTagManager))]
    [HarmonyPatch("ReportTag", MethodType.Normal)]
    internal class FixTagZonePatch
    {
        internal static void Postfix(GorillaTagManager __instance, Player taggedPlayer, Player taggingPlayer)
        {
            Debug.Log("TAGGING: " + taggedPlayer.NickName);
            if (__instance.photonView.IsMine && taggedPlayer == taggingPlayer)
            {
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                WebFlags flags = new WebFlags(1);
                raiseEventOptions.Flags = flags;

                if (__instance.isCurrentlyTag)
                {
                    if (__instance.currentIt != taggedPlayer)
                    {
                        __instance.ChangeCurrentIt(taggedPlayer);
                        __instance.lastTag = UnityEngine.Time.time;

                        object[] eventContent = new object[]
                        {
                            taggingPlayer.UserId,
                            taggedPlayer.UserId
                        };

                        PhotonNetwork.RaiseEvent(1, eventContent, raiseEventOptions, SendOptions.SendReliable);
                    }
                }
                else
                {
                    if (!__instance.currentInfected.Contains(taggedPlayer))
                    {
                        __instance.AddInfectedPlayer(taggedPlayer);
                        object[] eventContent2 = new object[]
                        {
                            taggingPlayer.UserId,
                            taggedPlayer.UserId,
                            __instance.currentInfected.Count
                        };
                        PhotonNetwork.RaiseEvent(2, eventContent2, raiseEventOptions, SendOptions.SendReliable);
                    }
                }
            }
        }
    }
}
