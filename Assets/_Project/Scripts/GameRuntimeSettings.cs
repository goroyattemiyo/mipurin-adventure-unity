using UnityEngine;

public static class GameRuntimeSettings
{
    private const int TargetFrameRate = 60;
    private const float TargetFixedDeltaTime = 1f / 60f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Apply()
    {
        Application.targetFrameRate = TargetFrameRate;
        QualitySettings.vSyncCount = 0;
        Time.fixedDeltaTime = TargetFixedDeltaTime;
    }
}
