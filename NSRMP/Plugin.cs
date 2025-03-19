using System.Reflection;
using BepInEx;
using HarmonyLib;
using kcp2k;
using Mirror;
using MonomiPark.SlimeRancher.Regions;
using On.MonomiPark.SlimeRancher.DataModel;
using UnityEngine;
using XGamingRuntime;

namespace NSRMP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static GameObject? SingletonContainer { get; private set;  }
    public static NetworkManager? NetworkManager { get; private set;  }
    
    private void Awake()
    {
        Assembly.GetExecutingAssembly().GetType("Mirror.GeneratedNetworkCode")?.GetMethod("InitReadWriters")?.Invoke(null, null);

        SingletonContainer = new GameObject("NSRMP_SingletonContainer");
        DontDestroyOnLoad(SingletonContainer);

        SingletonContainer.AddComponent<KcpTransport>();
        var networkManager = SingletonContainer.AddComponent<NetworkManager>();
        networkManager.dontDestroyOnLoad = true;
        networkManager.onlineScene = "worldGenerated";
        SingletonContainer.AddComponent<NetworkManagerHUD>();
        NetworkManager = networkManager;
        Debug.Log("NSRMP plugin loaded OK! huzzah");
        
        On.MonomiPark.SlimeRancher.DataModel.GameModel.RegisterActor += GameModelOnRegisterActor;
    }

    private void GameModelOnRegisterActor(GameModel.orig_RegisterActor orig, MonomiPark.SlimeRancher.DataModel.GameModel self, long actorid, GameObject gameobj, RegionRegistry.RegionSetId regionsetid, bool nonactorok, bool skipnotify)
    {
        
    }
}