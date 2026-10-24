using UnityEngine;

namespace Zeldinha.Player
{
    /// <summary>
    /// Controlador principal de movimento do jogador (top-down estilo Zelda).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movimento")]
        public float moveSpeed = 5f;
        public float sprintMultiplier = 1.6f;
        public float rotationSpeed = 12f;
        public float jumpForce = 7f;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundDistance = 0.3f;
        public LayerMask groundMask;

        Rigidbody rb;
        PlayerStats stats;
        PlayerCombat combat;

        Vector3 moveInput;
        bool isGrounded;
        bool isSprinting;

        public bool CanMove { get; set; } = true;
        public Vector3 MoveDirection => moveInput.normalized;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            stats = GetComponent<PlayerStats>();
            combat = GetComponent<PlayerCombat>();

            // Trava rotação indesejada
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        void Update()
        {
            if (!CanMove || Systems.GameManager.Instance?.CurrentState != Systems.GameManager.GameState.Playing)
            {
                moveInput = Vector3.zero;
                return;
            }

            // Input
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            moveInput = new Vector3(h, 0f, v).normalized;

            isSprinting = Input.GetKey(KeyCode.LeftShift) && moveInput.magnitude > 0.1f;

            // Jump
            isGrounded = Physics.CheckSphere(groundCheck != null ? groundCheck.position : transform.position, groundDistance, groundMask);
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            // Pausar
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Systems.GameManager.Instance?.TogglePause();
            }
        }

        void FixedUpdate()
        {
            if (!CanMove) return;

            float speed = moveSpeed;
            if (isSprinting && stats.TryUseStamina(12f * Time.fixedDeltaTime))
            {
                speed *= sprintMultiplier;
            }

            Vector3 velocity = moveInput * speed;
            velocity.y = rb.velocity.y;
            rb.velocity = velocity;

            // Rotação suave na direção do movimento
            if (moveInput.magnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveInput);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
            }
        }
    }
}
