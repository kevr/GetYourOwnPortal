using BepInEx;
using Steamworks;
using HarmonyLib;
using System.Reflection;
using BepInEx.Logging;

namespace GetYourOwnPortal;

[BepInPlugin(Plugin.GUID, Plugin.NAME, Plugin.VERSION)]
public class Plugin : BaseUnityPlugin
{
    static ManualLogSource _Logger = new ManualLogSource(GUID);

    // Plugin metadata
    public const string NAME = "GetYourOwnPortal";
    public const string AUTHOR = "Kevver";
    public const string GUID = $"{AUTHOR}.{NAME}";
    public const string VERSION = "1.0.2";

    // HarmonyLib handle
    static readonly Harmony harmony = new(NAME);

    public static ServerSync.ConfigSync configSync = new ServerSync.ConfigSync(GUID)
    {
        DisplayName = NAME,
        CurrentVersion = VERSION,
        MinimumRequiredVersion = VERSION
    };

    public static bool PlayerIsAdmin()
    {
        bool isAdmin = configSync.IsAdmin || (ZNet.instance && ZNet.instance.IsServer());
        _Logger.LogInfo($"IsAdmin: {isAdmin}");
        return isAdmin;
    }

    private void Awake()
    {
        // Setup loggers
        BepInEx.Logging.Logger.Sources.Add(Plugin._Logger);
        BepInEx.Logging.Logger.Sources.Add(Portal.Create.Logger);
        BepInEx.Logging.Logger.Sources.Add(Portal.Edit.Logger);
        BepInEx.Logging.Logger.Sources.Add(Portal.Destroy.Logger);

        // Patch assembly with HarmonyLib
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
