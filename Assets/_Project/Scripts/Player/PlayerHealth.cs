using UnityEngine;

namespace MipurinAdventure.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int maxHp = 5;
        private int currentHp;

        public int CurrentHp => currentHp;
        public int MaxHp => maxHp;
        public bool IsDefeated => currentHp <= 0;

        private void Awake()
        {
            currentHp = maxHp;
        }

        public void TakeDamage(int amount)
        {
            if (IsDefeated) return;

            currentHp = Mathf.Max(0, currentHp - amount);

            if (currentHp <= 0)
            {
                Debug.Log("Mipurin defeated.");
            }
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            currentHp = Mathf.Min(maxHp, currentHp + amount);
        }
    }
}
