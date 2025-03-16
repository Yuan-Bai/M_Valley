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
    [SerializeField] private Animator[] _animators;

    private Vector2 _moveInput;
    private Vector2 _smoothMovement;
    private bool _isMoving;
    private bool _inputEnable = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _animators = GetComponentsInChildren<Animator>();
    }

    void OnMovement(InputValue value)
    {
        _moveInput = value.Get<Vector2>().normalized;
    }

    private void Update()
    {
        if(_inputEnable)
            ApplyMovement();
        UpdateAnimation();
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
        if (_animators == null) return;

        _isMoving = _moveInput!= Vector2.zero;

        foreach (var animator in _animators)
        {
            animator.SetBool("IsMoving", _isMoving);
            if (_isMoving)
            {
                animator.SetFloat("InputX", _moveInput.x);
                animator.SetFloat("InputY", _moveInput.y);
            }
        }
    }

    public void MoveToPosition(Vector3 positionToGO)
    {
        transform.position = positionToGO;
    }

    public void SetInputEnabled(bool enabled)
    {
        _inputEnable = enabled;
    }
}