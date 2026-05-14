using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class BasicCharacterController : MonoBehaviour
{
    public InputActionAsset ActionAsset;
    public float Speed = 4.0f;
    public float RunSpeed = 8.0f;
    public float FallingSpeed = 15.0f;
    public float MaxFallingSpeed = 20.0f;
    public float JumpForce = 6.0f;
    public float TurnSpeed = 10.0f;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private InputAction m_SprintAction;
    private CharacterController m_Controller;
    private Animator m_Animator;

    private int m_SpeedAnimatorHash = Animator.StringToHash("Speed");
    private int m_GroundedAnimatorHash = Animator.StringToHash("Grounded");
    private int m_SprintAnimatorHash = Animator.StringToHash("Sprint");
    private int m_VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");

    private Vector3 m_TargetDirection;
    private Vector3 m_CurrentVelocity;

    void OnEnable()
    {
        // Buscamos las acciones en el Action Asset configurado
        m_MoveAction = ActionAsset.FindAction("Move");
        m_JumpAction = ActionAsset.FindAction("Jump");
        m_SprintAction = ActionAsset.FindAction("Sprint");

        m_Controller = GetComponent<CharacterController>();
        m_Animator = GetComponentInChildren<Animator>();

        m_TargetDirection = m_Controller.transform.forward;

        m_MoveAction.Enable();
        m_JumpAction.Enable();
        m_SprintAction.Enable();
    }

    void Update()
    {
        var sprinting = m_SprintAction.IsPressed();
        m_Animator.SetBool(m_SprintAnimatorHash, sprinting);

        if (m_CurrentVelocity.y < -MaxFallingSpeed)
            m_CurrentVelocity.y = -MaxFallingSpeed;

        // Movimiento relativo a donde mira la cámara principal
        var forward = Camera.main.transform.forward;
        forward.y = 0;
        if (forward.sqrMagnitude == 0.0f)
        {
            forward = Camera.main.transform.up;
            forward.y = 0;
        }
        forward.Normalize();

        var right = Camera.main.transform.right;

        var moveInput = m_MoveAction.ReadValue<Vector2>();
        var move = moveInput.y * forward + moveInput.x * right;
        move.Normalize();
        m_Animator.SetFloat(m_SpeedAnimatorHash, move.magnitude);

        m_CurrentVelocity.x = 0;
        m_CurrentVelocity.z = 0;

        var horizontalMove = move * (sprinting ? RunSpeed : Speed);
        if (horizontalMove.sqrMagnitude > 0.01f)
            m_TargetDirection = move;

        float actualSpeed = Mathf.Clamp01(1.0f - Vector3.Angle(horizontalMove, transform.forward) / 90.0f);

        transform.forward = Vector3.RotateTowards(transform.forward, m_TargetDirection, Time.deltaTime * TurnSpeed, 0.0f);

        m_CurrentVelocity += transform.forward * horizontalMove.magnitude * actualSpeed;

        // Salto y Gravedad
        if (m_JumpAction.WasPressedThisFrame() && m_Controller.isGrounded)
        {
            m_CurrentVelocity.y = JumpForce;
        }
        else
        {
            m_CurrentVelocity.y -= FallingSpeed * Time.deltaTime;
        }

        var c = m_Controller.Move(m_CurrentVelocity * Time.deltaTime);
        if ((c & CollisionFlags.Below) != 0)
        {
            m_CurrentVelocity.y = -2.0f;
        }

        // Enviamos los datos al Animator que me mostraste en la foto
        m_Animator.SetBool(m_GroundedAnimatorHash, m_Controller.isGrounded);
        m_Animator.SetFloat(m_VerticalSpeedHash, m_CurrentVelocity.y);
    }
}