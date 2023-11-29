using BepInEx.Logging;
using GetYourOwnPortal;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetYourOwnPortal.Portal;

[HarmonyPatch(typeof(Piece), nameof(Piece.SetCreator), typeof(long))]
static class Create
{
    public static ManualLogSource Logger = new ManualLogSource($"{Plugin.GUID}.Create");
    static HashSet<int> m_setup = new();

    public static void Add(int portalId)
    {
        m_setup.Add(portalId);
    }

    public static bool Has(int portalId)
    {
        return m_setup.Contains(portalId);
    }

    public static void Remove(int portalId)
    {
        m_setup.Remove(portalId);
    }

    static void Postfix(Piece __instance, long uid)
    {
        TeleportWorld port = __instance.GetComponentInChildren<TeleportWorld>();
        string playerName = Player.m_localPlayer.GetPlayerName();
        m_setup.Add(port.GetInstanceID());

        string portalPrefix = Util.GetPortalPrefix(playerName);
        port.SetText(portalPrefix);
    }
}