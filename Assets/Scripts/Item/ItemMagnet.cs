// using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
// public class ItemMagnet : MonoBehaviour
// {
//     // 配置参数
//     [SerializeField] private float attractRadius = 3f;      // 吸引半径
//     [SerializeField] private float minPickupDistance = 0.5f;// 最小拾取距离
//     [SerializeField] private float baseSpeed = 2f;          // 基础移动速度
//     [SerializeField] private float acceleration = 1.2f;     // 移动加速度
//     [SerializeField] private float rotationSpeed = 180f;    // 旋转速度

//     // 组件引用
//     private Rigidbody2D _rb;
//     private Transform _player;
//     private bool _isAttracted;

//     void Awake()
//     {
//         _rb = GetComponent<Rigidbody2D>();
//         _rb.gravityScale = 0; // 禁用重力
//     }


//     void StartAttraction()
//     {
//         _isAttracted = true;
//         // 禁用碰撞检测（避免移动时卡住）
//         GetComponent<Collider2D>().enabled = false; 
//         // 触发视觉效果
//         PlayAttractEffect(); 
//     }

//     void UpdateMovement(float currentDistance)
//     {
//         Vector2 direction = (_player.position - transform.position).normalized;
//         float speed = baseSpeed + (attractRadius - currentDistance) * acceleration;
        
//         _rb.velocity = direction * speed;
//     }

//     void UpdateRotation()
//     {
//         float angle = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
//         Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
//         transform.rotation = Quaternion.Slerp(
//             transform.rotation, 
//             targetRotation, 
//             rotationSpeed * Time.deltaTime
//         );
//     }

//     void CompletePickup()
//     {
//         if (InventoryController.Instance.TryAddItem(GetComponent<WorldItem>().ItemID))
//         {
//             ItemPool.Instance.ReturnItem(this);
//         }
//         PlayPickupEffect();
//     }

//     // 可视化调试
//     #if UNITY_EDITOR
//     void OnDrawGizmosSelected()
//     {
//         Handles.color = Color.yellow;
//         Handles.DrawWireDisc(transform.position, Vector3.forward, attractRadius);
//         Handles.color = Color.red;
//         Handles.DrawWireDisc(transform.position, Vector3.forward, minPickupDistance);
//     }
//     #endif
// }
