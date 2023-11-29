using BepInEx;
using Steamworks;
using HarmonyLib;
using System.Reflection;

namespace GetYourOwnPortal;

[BepInPlugin(Plugin.GUID, Plugin.NAME, Plugin.VERSION)]
public class Plugin : BaseUnityPlugin
{
    // Plugin metadata
    public const string NAME = "GetYourOwnPortal";
    public const string AUTHOR = "Kevver";
    public const string GUID = $"{AUTHOR}.{NAME}";
    public const string VERSION = "1.0.0";

    // HarmonyLib handle
    static readonly Harmony harmony = new(NAME);

    private void Awake()
    {
        // Setup loggers
        BepInEx.Logging.Logger.Sources.Add(Portal.Create.Logger);
        BepInEx.Logging.Logger.Sources.Add(Portal.Edit.Logger);
        BepInEx.Logging.Logger.Sources.Add(Portal.Destroy.Logger);

        // Patch assembly with HarmonyLib
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
