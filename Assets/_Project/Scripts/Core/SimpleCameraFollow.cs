using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float followSpeed = 8f;
    [SerializeField] private bool snapOnStart = true;

    [Header("Limits")]
    [SerializeField] private bool useLimits;
    [SerializeField] private Vector2 minPosition = new Vector2(-20f, -12f);
    [SerializeField] private Vector2 maxPosition = new Vector2(20f, 12f);

    public Transform Target => target;

    private void Start()
    {
        if (snapOnStart)
        {
            SnapToTarget();
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition = ClampPosition(desiredPosition);

        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, t);
    }

    public void Configure(Transform followTarget, Vector3 followOffset, float speed, bool enableLimits, Vector2 min, Vector2 max)
    {
        target = followTarget;
        offset = followOffset;
        followSpeed = Mathf.Max(0.01f, speed);
        useLimits = enableLimits;
        minPosition = min;
        maxPosition = max;
    }

    public void SetTarget(Transform followTarget)
    {
        target = followTarget;
    }

    public void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        transform.position = ClampPosition(target.position + offset);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        if (!useLimits)
        {
            return position;
        }

        position.x = Mathf.Clamp(position.x, minPosition.x, maxPosition.x);
        position.y = Mathf.Clamp(position.y, minPosition.y, maxPosition.y);
        return position;
    }
}
