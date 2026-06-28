using UnityEngine;

public class VillageInventory : MonoBehaviour
{
    private static VillageInventory instance;

    [SerializeField] private int smallHoneyBottleCount;
    [SerializeField] private int pollenCandyCount;
    [SerializeField] private int waxShieldCount;

    public static VillageInventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<VillageInventory>();
            }

            if (instance == null)
            {
                GameObject inventoryObject = new GameObject("VillageInventory");
                instance = inventoryObject.AddComponent<VillageInventory>();
            }

            return instance;
        }
    }

    public int SmallHoneyBottleCount => smallHoneyBottleCount;
    public int PollenCandyCount => pollenCandyCount;
    public int WaxShieldCount => waxShieldCount;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddSmallHoneyBottle()
    {
        smallHoneyBottleCount++;
    }

    public void AddPollenCandy()
    {
        pollenCandyCount++;
    }

    public void AddWaxShield()
    {
        waxShieldCount++;
    }

    public void ResetForTest()
    {
        smallHoneyBottleCount = 0;
        pollenCandyCount = 0;
        waxShieldCount = 0;
    }
}
