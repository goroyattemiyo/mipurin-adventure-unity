using UnityEngine;

namespace MipurinAdventure.Player
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Transform attackOrigin;
        [SerializeField] private float attackRange = 0.8f;
        [SerializeField] private int damage = 1;
        [SerializeField] private LayerMask targetLayer;

        private PlayerController controller;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Attack();
            }
        }

        private void Attack()
        {
            Vector2 dir = controller != null ? controller.LastMoveDirection : Vector2.down;
            Vector2 origin = attackOrigin != null ? (Vector2)attackOrigin.position : (Vector2)transform.position;
            Vector2 center = origin + dir.normalized * attackRange;

            Collider2D hit = Physics2D.OverlapCircle(center, attackRange, targetLayer);
            if (hit != null)
            {
                hit.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
