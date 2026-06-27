using UnityEngine;

public class CharacterTestDisplayStabilizer : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private bool stabilizeCamera = true;
    [SerializeField] private float orthographicSize = 3.2f;

    [Header("Timing")]
    [SerializeField] private bool stabilizeTiming = true;
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private float fixedDeltaTime = 1f / 60f;

    private Camera targetCamera;

    private void Awake()
    {
        ApplyTiming();
        ApplyCamera();
    }

    private void LateUpdate()
    {
        ApplyCamera();
    }

    private void ApplyTiming()
    {
        if (!stabilizeTiming)
        {
            return;
        }

        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = 0;
        Time.fixedDeltaTime = fixedDeltaTime;
    }

    private void ApplyCamera()
    {
        if (!stabilizeCamera)
        {
            return;
        }

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            return;
        }

        targetCamera.orthographic = true;
        targetCamera.orthographicSize = orthographicSize;
    }
}
