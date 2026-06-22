using UnityEngine;

namespace MipurinAdventure.Enemy
{
    public class SimpleEnemy : MonoBehaviour
    {
        [SerializeField] private int hp = 3;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private Transform target;

        private void Update()
        {
            if (target == null) return;

            Vector2 dir = (target.position - transform.position).normalized;
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }

        public void TakeDamage(int amount)
        {
            hp -= amount;
            if (hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
