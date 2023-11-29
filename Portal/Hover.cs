using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace GetYourOwnPortal.Portal;

[HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.GetHoverName))]
static class GetHoverName
{
    static void Postfix(TeleportWorld __instance, ref string __result)
    {
        string text = __instance.GetText();
        if (text.IndexOf(PortalInfo.DELIM) == -1)
            return;

        PortalInfo info = Util.GetPortalInfo(text);
        __result = info.text;
    }
}

// TODO: Transfer portal metadata to client on connection!
[HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.GetHoverText))]
static class GetHoverText
{
    static void Postfix(TeleportWorld __instance, ref string __result)
    {
        string text = __instance.GetText();
        if (text.IndexOf(PortalInfo.DELIM) == -1)
            return;

        PortalInfo info = Util.GetPortalInfo(text);
        string portalPrefix = Util.GetPortalPrefix(info.playerName);

        __result = __result.Replace($"\"{portalPrefix}", "\"");

        StringBuilder sb = new();
        if (info.playerName == Player.m_localPlayer.GetPlayerName())
        {
            sb.Append(Environment.NewLine);
            sb.Append("[<color=yellow>LAlt+E<color=white>] Disown");
        }

        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append($"Owner: <color=orange>{info.playerName}");

        __result += sb.ToString();
    }
}