using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
public class PlayerControls : MonoBehaviour
{
    [Header("移动参数")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("组件引用")]
    [SerializeField] private Rigidbody2D rb;
    // [SerializeField] private Collider2D collider2D;
    [SerializeField] private Animator animator;

    private Vector2 _moveInput;
    private Vector2 _smoothMovement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMovement(InputValue value)
    {
        _moveInput = value.Get<Vector2>().normalized;
    }

    private void Update()
    {
        ApplyMovement();
        UpdateAnimation();
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        // normalized确保斜向移动不会过快
        _moveInput = context.ReadValue<Vector2>().normalized;
    }

    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    private void ApplyMovement()
    {
        _smoothMovement = Vector2.MoveTowards(
            _smoothMovement, 
            _moveInput,
            (_moveInput.magnitude > 0 ? acceleration : deceleration) * Time.fixedDeltaTime
            );

         rb.velocity = _smoothMovement * moveSpeed;
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        // bool isMoving = _smoothMovement.magnitude > 0.1f;
        // animator.SetBool(IsMoving, isMoving);

        // if (isMoving)
        // {
        //     // 保持最后移动方向
        //     animator.SetFloat(MoveX, _smoothMovement.x);
        //     animator.SetFloat(MoveY, _smoothMovement.y);
        // }
    }
}