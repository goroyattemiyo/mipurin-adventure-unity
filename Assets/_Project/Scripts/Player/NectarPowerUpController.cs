using UnityEngine;

public class NectarPowerUpController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NectarWallet wallet;
    [SerializeField] private MipurinAttack attack;

    [Header("Power Levels")]
    [SerializeField] private int level1Nectar = 5;
    [SerializeField] private int level2Nectar = 10;

    private int currentPowerLevel;

    public int CurrentPowerLevel => currentPowerLevel;
    public int Level1Nectar => level1Nectar;
    public int Level2Nectar => level2Nectar;

    private void Awake()
    {
        AutoFindReferences();
        ApplyPowerLevel(0, false);
    }

    private void Update()
    {
        AutoFindReferences();

        if (wallet == null || attack == null)
        {
            return;
        }

        int targetLevel = GetTargetPowerLevel(wallet.Nectar);
        if (targetLevel != currentPowerLevel)
        {
            ApplyPowerLevel(targetLevel, true);
        }
    }

    public void ResetPower()
    {
        ApplyPowerLevel(0, false);
    }

    private void AutoFindReferences()
    {
        if (wallet == null)
        {
            wallet = GetComponent<NectarWallet>();
        }

        if (attack == null)
        {
            attack = GetComponent<MipurinAttack>();
        }
    }

    private int GetTargetPowerLevel(int nectar)
    {
        if (nectar >= level2Nectar)
        {
            return 2;
        }

        if (nectar >= level1Nectar)
        {
            return 1;
        }

        return 0;
    }

    private void ApplyPowerLevel(int level, bool showMessage)
    {
        currentPowerLevel = Mathf.Clamp(level, 0, 2);

        if (attack != null)
        {
            attack.ApplyPowerLevel(currentPowerLevel);
        }

        if (showMessage && currentPowerLevel > 0)
        {
            string label = currentPowerLevel == 1 ? "POWER UP Lv.1" : "POWER UP Lv.2";
            PickupFloatingText.Show(transform.position + new Vector3(0f, 0.6f, 0f), label);
            Debug.Log(label);
        }
    }
}
