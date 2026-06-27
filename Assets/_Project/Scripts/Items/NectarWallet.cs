using UnityEngine;

public class NectarWallet : MonoBehaviour
{
    [SerializeField] private int nectar;

    public int Nectar => nectar;

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
    }
}
