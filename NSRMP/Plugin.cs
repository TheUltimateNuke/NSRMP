using System.Reflection;
using BepInEx;
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
        Assembly.GetExecutingAssembly().GetType("Mirror.GeneratedNetworkCode").GetMethod("InitReadWriters")?.Invoke(null, null);
        
        CommonEvents.OnGameContextAwake += OnGameContextAwake;
    }

    private void OnGameContextAwake()
    {
        SingletonContainer = new GameObject("NSRMP_SingletonContainer");
        DontDestroyOnLoad(SingletonContainer);

        SingletonContainer.AddComponent<KcpTransport>();
        SingletonContainer.AddComponent<NetworkManager>();
        SingletonContainer.AddComponent<NetworkManagerHUD>();
    }
}

public static class CommonEvents
{
    public static Action? OnGameContextAwake;
}