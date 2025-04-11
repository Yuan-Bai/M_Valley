using System.Collections;
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
    [SerializeField] private Rigidbody2D _rb;
    // [SerializeField] private Collider2D collider2D;
    [SerializeField] private Animator[] _animators;

    [Header("事件通道")]
    [SerializeField] private PlayerEventChannel _playerEventChannel;
    [SerializeField] private GridEventChannel _gridEventChannel;

    private Vector2 _moveInput;
    private Vector2 _smoothMovement;
    private bool _isMoving;
    private bool _inputEnable = true;
    private bool _useTool;
    private float _mouseX;
    private float _mouseY;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animators = GetComponentsInChildren<Animator>();
    }

    void OnEnable()
    {
        _playerEventChannel.OnToolAnimation += HandleToolAnimation;
    }

    void OnDisable()
    {
        _playerEventChannel.OnToolAnimation -= HandleToolAnimation;
    }

    #region 事件

    void OnMovement(InputValue value)
    {
        _moveInput = value.Get<Vector2>().normalized;
    }

    private void HandleToolAnimation(ItemType itemType, Vector2 pos)
    {
        if (_useTool)
            return;

        if (itemType != ItemType.Seed && itemType != ItemType.Commodity && itemType != ItemType.Furniture)
        {
            _mouseX = pos.x - transform.position.x;
            _mouseY = pos.y - transform.position.y;
            if (Mathf.Abs(_mouseX) > Mathf.Abs(_mouseY))
                _mouseY = 0;
            else
                _mouseX = 0;
            
            StartCoroutine(UseToolRoutine(itemType, pos));
        }
        else
        {
            _gridEventChannel.RaiseTileUpdate(itemType, pos);
        }
    }

    #endregion

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
         _rb.velocity = _smoothMovement * moveSpeed;
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

    private IEnumerator UseToolRoutine(ItemType itemType, Vector2 pos)
    {
        _useTool = true;
        _inputEnable = false;
        yield return null;
        foreach (var animator in _animators)
        {
            animator.SetTrigger("UseTool");
            animator.SetFloat("MouseX", _mouseX);
            animator.SetFloat("MouseY", _mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        _gridEventChannel.RaiseTileUpdate(itemType, pos);
        yield return new WaitForSeconds(0.35f);
        _useTool = false;
        _inputEnable = true;
    }

    public void MoveToPosition(Vector3 positionToGO)
    {
        transform.position = positionToGO;
    }

    public void SetInputEnabled(bool enabled)
    {
        _inputEnable = enabled;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rb.velocity = velocity;
    }
}