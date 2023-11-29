using BepInEx.Logging;
using GetYourOwnPortal;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GetYourOwnPortal.Portal;

[HarmonyPatch(typeof(TextInput), nameof(TextInput.RequestText), typeof(TextReceiver), typeof(string), typeof(int))]
static class TextInputRequestText
{
    static bool Prefix(TextInput __instance, TextReceiver sign, string topic, int charLimit)
    {
        if (topic != "$piece_portal_tag")
            return true;

        TeleportWorld portal = sign as TeleportWorld;
        string text = portal.GetText();

        if (text.IndexOf(PortalInfo.DELIM) == -1)
            return true;

        PortalInfo info = Util.GetPortalInfo(text);

        TextInput.instance.m_queuedSign = sign;
        TextInput.instance.Show(topic, info.text, charLimit);

        return false;
    }
}

[HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.Interact), typeof(Humanoid), typeof(bool), typeof(bool))]
static class TeleportWorldInteract
{
    static bool Prefix(TeleportWorld __instance, Humanoid human, bool hold, bool alt)
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            PortalInfo info = Util.GetPortalInfo(__instance.GetText());
            Create.Add(__instance.GetInstanceID());
            __instance.SetText(info.text);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(TeleportWorld), nameof(TeleportWorld.SetText), typeof(string))]
static class Edit
{
    public static ManualLogSource Logger = new ManualLogSource($"{Plugin.GUID}.Edit");

    static bool Prefix(TeleportWorld __instance, string text)
    {
        int portalId = __instance.GetInstanceID();
        Logger.LogInfo($"Editing a portal with id: {portalId}");

        if (Create.Has(portalId))
        {
            Logger.LogInfo("Portal is being created, passing through...");
            Create.Remove(portalId);
            return true;
        }

        // If the mod hasn't taken this portal, no-op this override
        string currentText = __instance.GetText();
        if (currentText.IndexOf(PortalInfo.DELIM) == -1)
            return true;

        PortalInfo info = Util.GetPortalInfo(currentText);
        string portalPrefix = Util.GetPortalPrefix(info.playerName);
        Logger.LogInfo($"PortalInfo ( player: {info.playerName}, oldText: '{info.text}', newText: '{text}' )");

        // If the text is user input
        if (text.IndexOf(portalPrefix) == -1)
        {
            if (text.LastIndexOf(PortalInfo.DELIM) != -1)
            {
                Player.m_localPlayer.Message(
                    MessageHud.MessageType.Center,
                    $"<color=red>'{PortalInfo.DELIM}' characters are not allowed in portal names");
                return false;
            }

            __instance.SetText($"{portalPrefix}{text}");
            return false;
        }
        else if (text.LastIndexOf(PortalInfo.DELIM) != text.IndexOf(PortalInfo.DELIM))
        {
            Player.m_localPlayer.Message(
                MessageHud.MessageType.Center,
                $"<color=red>'{PortalInfo.DELIM}' characters are not allowed in portal names");
            return false;
        }

        return true;
    }
}
