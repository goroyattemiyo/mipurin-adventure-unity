using UnityEngine;

public class NectarWallet : MonoBehaviour
{
    [SerializeField] private int nectar;

    public int Nectar => nectar;

    private void Awake()
    {
        EnsurePowerUpController();
    }

    public void AddNectar(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        nectar += amount;
        Debug.Log($"Nectar +{amount}. Total: {nectar}");
    }

    public void ResetNectar()
    {
        nectar = 0;

        NectarPowerUpController powerUp = GetComponent<NectarPowerUpController>();
        if (powerUp != null)
        {
            powerUp.ResetPower();
        }
    }

    private void EnsurePowerUpController()
    {
        NectarPowerUpController powerUp = GetComponent<NectarPowerUpController>();
        if (powerUp == null)
        {
            gameObject.AddComponent<NectarPowerUpController>();
        }
    }
}
