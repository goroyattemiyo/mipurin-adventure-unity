using UnityEngine;

public class HoneyWallet : MonoBehaviour
{
    private static HoneyWallet instance;

    [SerializeField] private int honey = 30;

    public static HoneyWallet Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HoneyWallet>();
            }

            if (instance == null)
            {
                GameObject walletObject = new GameObject("HoneyWallet");
                instance = walletObject.AddComponent<HoneyWallet>();
            }

            return instance;
        }
    }

    public int Honey => honey;

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

    public void AddHoney(int amount)
    {
        honey = Mathf.Max(0, honey + Mathf.Max(0, amount));
    }

    public bool CanSpend(int amount)
    {
        return amount >= 0 && honey >= amount;
    }

    public bool TrySpend(int amount)
    {
        amount = Mathf.Max(0, amount);
        if (!CanSpend(amount))
        {
            return false;
        }

        honey -= amount;
        return true;
    }

    public void ResetForTest(int value = 30)
    {
        honey = Mathf.Max(0, value);
    }
}
