using UnityEngine;

public class EnemyDropper : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private int nectarAmount = 1;
    [SerializeField] private Vector3 dropOffset = new Vector3(0f, 0.15f, 0f);
    [SerializeField] private bool dropOnlyOnce = true;

    private bool hasDropped;

    public void Drop()
    {
        if (dropOnlyOnce && hasDropped)
        {
            return;
        }

        hasDropped = true;

        if (dropPrefab == null)
        {
            Debug.LogWarning($"Drop prefab is not assigned: {gameObject.name}");
            return;
        }

        GameObject drop = Instantiate(dropPrefab, transform.position + dropOffset, Quaternion.identity);
        PickupItem pickupItem = drop.GetComponent<PickupItem>();

        if (pickupItem != null)
        {
            pickupItem.Configure(nectarAmount, 0.65f, 20f);
        }
    }

    public void Configure(GameObject prefab, int amount)
    {
        dropPrefab = prefab;
        nectarAmount = Mathf.Max(1, amount);
        hasDropped = false;
    }
}
