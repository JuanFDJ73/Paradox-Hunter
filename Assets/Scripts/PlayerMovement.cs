using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private bool jumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // INPUT SYSTEM
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            jumpPressed = true;
    }

    private void FixedUpdate()
    {
        // Movimiento Horizontal
        float moveX = moveInput.x;
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        // Animación: isRunning
        bool isRunning = moveX != 0;
        animator.SetBool("isRunning", isRunning);

        // Flip del personaje
        if (moveX != 0)
        {
            transform.localScale = new Vector3(moveX > 0 ? 1 : -1, 1, 1);
        }

        // Salto
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
        }
    }
}