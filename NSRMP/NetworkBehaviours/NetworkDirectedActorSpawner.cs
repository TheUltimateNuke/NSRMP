using Mirror;

namespace NSRMP.NetworkBehaviours;

public class NetworkDirectedActorSpawner : NetworkBehaviour
{
    private void Awake()
    {
        On.DirectedActorSpawner.CanSpawn += DirectedActorSpawnerOnCanSpawn;
    }

    private bool DirectedActorSpawnerOnCanSpawn(On.DirectedActorSpawner.orig_CanSpawn orig, DirectedActorSpawner self, float? forhour)
    {
        if (Plugin.NetworkManager == null || !Plugin.NetworkManager.isNetworkActive)
            return orig(self, forhour);

        if (!authority)
            return false;

        return orig(self, forhour);
    }
}