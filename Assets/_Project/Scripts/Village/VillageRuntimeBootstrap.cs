using UnityEngine;
using UnityEngine.SceneManagement;

public static class VillageRuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureForScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureForScene(scene);
    }

    private static void EnsureForScene(Scene scene)
    {
        HoneyWallet wallet = HoneyWallet.Instance;
        VillageInventory inventory = VillageInventory.Instance;

        if (scene.name != "VillageTest")
        {
            return;
        }

        _ = wallet;
        _ = inventory;
        _ = VillageShopManager.Instance;
        _ = VillageHudManager.Instance;
    }
}
