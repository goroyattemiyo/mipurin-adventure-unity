using UnityEngine;

public class MipurinDebugHud : MonoBehaviour
{
    [SerializeField] private MipurinHealth playerHealth;
    [SerializeField] private MipurinEnemy enemy;

    private void Update()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<MipurinHealth>();
        }

        if (enemy == null)
        {
            enemy = FindObjectOfType<MipurinEnemy>();
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(12, 12, 460, 24), "Move: WASD / Arrow keys    Attack: Space / Left Click");

        if (playerHealth != null)
        {
            GUI.Label(new Rect(12, 36, 260, 24), $"Mipurin HP: {playerHealth.CurrentHp}/{playerHealth.MaxHp}");
            GUI.Label(new Rect(12, 60, 260, 24), $"Mipurin State: {playerHealth.StateLabel}");
        }

        if (enemy != null)
        {
            GUI.Label(new Rect(12, 84, 220, 24), $"Enemy HP: {enemy.CurrentHp}/{enemy.MaxHp}");
        }
        else
        {
            GUI.Label(new Rect(12, 84, 260, 24), "Enemy: defeated or not found");
        }
    }

    public void Configure(MipurinHealth health, MipurinEnemy targetEnemy)
    {
        playerHealth = health;
        enemy = targetEnemy;
    }
}
