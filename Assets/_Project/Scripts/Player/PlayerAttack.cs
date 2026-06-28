using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
            if (IsDialogueOpen())
            {
                return;
            }

            if (WasAttackPressed())
            {
                Attack();
            }
        }

        private bool IsDialogueOpen()
        {
            DialogueManager dialogueManager = DialogueManager.Instance;
            return dialogueManager != null && dialogueManager.IsOpen;
        }

        private bool WasAttackPressed()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            bool keyboardPressed = keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
            bool mousePressed = mouse != null && mouse.leftButton.wasPressedThisFrame;

            if (keyboardPressed || mousePressed)
            {
                return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                return true;
            }
#endif

            return false;
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
