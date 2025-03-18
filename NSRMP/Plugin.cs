using System.Reflection;
using BepInEx;
using HarmonyLib;
using kcp2k;
using Mirror;
using UnityEngine;

namespace NSRMP;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static GameObject? SingletonContainer { get; private set;  }
    
    private void Awake()
    {
        Assembly.GetExecutingAssembly().GetType("Mirror.GeneratedNetworkCode")?.GetMethod("InitReadWriters")?.Invoke(null, null);

        On.GameContext.Awake += OnGameContextAwake;
        Debug.Log("NSRMP plugin loaded OK!");
    }

    private void OnGameContextAwake(On.GameContext.orig_Awake orig, GameContext self)
    {
        SingletonContainer = new GameObject("NSRMP_SingletonContainer");
        DontDestroyOnLoad(SingletonContainer);

        SingletonContainer.AddComponent<KcpTransport>();
        var networkManager = SingletonContainer.AddComponent<NetworkManager>();
        networkManager.dontDestroyOnLoad = true;
        SingletonContainer.AddComponent<NetworkManagerHUD>();
    }
}