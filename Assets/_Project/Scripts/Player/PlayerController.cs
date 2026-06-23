using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // PlayerAttack.cs が参照する最後の移動方向
    // 正面キャラなので、初期値は下方向＝正面扱いにしておく
    public Vector2 LastMoveDirection { get; private set; } = Vector2.down;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = ReadMoveInput();

        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput.Normalize();
        }

        // 入力がある時だけ最後の向きを更新
        if (moveInput.sqrMagnitude > 0.01f)
        {
            LastMoveDirection = moveInput.normalized;
        }
    }

    private void FixedUpdate()
    {
        Vector2 delta = moveInput * moveSpeed * Time.fixedDeltaTime;

        if (rb != null)
        {
            rb.MovePosition(rb.position + delta);
        }
        else
        {
            transform.position += new Vector3(delta.x, delta.y, 0f);
        }
    }

    private Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                input.x -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                input.x += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                input.y -= 1f;
            }

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                input.y += 1f;
            }
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        input.x += Input.GetAxisRaw("Horizontal");
        input.y += Input.GetAxisRaw("Vertical");
#endif

        return input;
    }
}