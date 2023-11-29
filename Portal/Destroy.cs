using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetYourOwnPortal.Portal;

[HarmonyPatch(typeof(Destructible), nameof(Destructible.Destroy), typeof(UnityEngine.Vector3), typeof(UnityEngine.Vector3))]
static class Destroy
{
    public static ManualLogSource Logger = new ManualLogSource($"{Plugin.GUID}.Destroy");

    static bool Prefix(Destructible __instance, UnityEngine.Vector3 hitPoint, UnityEngine.Vector3 hitDir)
    {
        TeleportWorld portal = __instance.GetComponentInChildren<TeleportWorld>();
        if (portal == null)
            return true;

        string text = portal.GetText();
        if (text.IndexOf(PortalInfo.DELIM) == -1)
            return true;

        PortalInfo info = Util.GetPortalInfo(text);
        string localPlayerName = Player.m_localPlayer.GetPlayerName();
        bool isAdmin = ZNet.instance.IsAdmin(ZNet.GetPublicIP());

        // If the user isn't an admin and they don't own the portal, error out.
        if (!isAdmin && localPlayerName != info.playerName)
        {
            Player.m_localPlayer.Message(
                MessageHud.MessageType.Center,
                $"<color=red>You don't own this portal, {info.playerName} does");
            return false;
        }

        return true;
    }
}
