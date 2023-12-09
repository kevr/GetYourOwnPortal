using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GetYourOwnPortal.Portal;

[HarmonyPatch(typeof(Player), nameof(Player.RemovePiece))]
static class Destroy
{
    public static ManualLogSource Logger = new ManualLogSource($"{Plugin.GUID}.Destroy");

    static bool Prefix(Player __instance)
    {
        Logger.LogInfo("Destroying a piece...");

        Piece piece;
        if (!Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out var hitInfo, 50f, __instance.m_removeRayMask) && Vector3.Distance(hitInfo.point, __instance.m_eye.position) < __instance.m_maxPlaceDistance)
        {
            Logger.LogError("Raycast failed");
            return true;
        }

        piece = hitInfo.collider.GetComponentInParent<Piece>();
        if (piece == null && (bool)hitInfo.collider.GetComponent<Heightmap>())
        {
            piece = TerrainModifier.FindClosestModifierPieceInRange(hitInfo.point, 2.5f);
        }

        if (piece == null || !(bool)piece)
            return true;

        Logger.LogInfo($"Piece: '{piece.name}'");
        if (!piece.name.StartsWith("portal_wood"))
            return true;

        Logger.LogInfo("Destroying portal...");

        TeleportWorld portal = piece.GetComponentInChildren<TeleportWorld>();
        Logger.LogInfo("Portal exists...");

        string text = portal.GetText();
        if (text.IndexOf(PortalInfo.DELIM) == -1)
            return true;

        PortalInfo info = Util.GetPortalInfo(text);
        string localPlayerName = Player.m_localPlayer.GetPlayerName();

        // If the user isn't an admin and they don't own the portal, error out.
        if (!Plugin.PlayerIsAdmin() && localPlayerName != info.playerName)
        {
            Player.m_localPlayer.Message(
                MessageHud.MessageType.Center,
                $"<color=red>You don't own this portal, {info.playerName} does");
            return false;
        }

        return true;
    }
}
