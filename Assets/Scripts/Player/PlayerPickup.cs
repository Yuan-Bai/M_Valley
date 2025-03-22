using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class PlayerPickup : MonoBehaviour
{
    [Header("设置")]
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private float pickupRadius = 2f;
    [SerializeField] private float attractSpeed = 5f;
    [SerializeField] private float minPickupDistance = 0.3f;

    [Header("事件")]
    [SerializeField] private ItemEventChannel _pickupEventChannel;
    
    private Collider2D pickupCollider;
    private readonly HashSet<WorldItem> _processingItems = new();

    void Awake()
    {
        pickupCollider = GetComponent<CircleCollider2D>();
        pickupCollider.isTrigger = true;
        ConfigureCollider();
    }

    private void ConfigureCollider()
    {
        if (pickupCollider is CircleCollider2D circle)
        {
            circle.radius = pickupRadius;
            circle.offset = new Vector2(0, 1);
        }
        else
        {
            Debug.LogError("请使用CircleCollider2D以获得最佳检测效果");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsItemValid(other)) return;
        
        var item = other.GetComponent<WorldItem>();
        if (item != null && !_processingItems.Contains(item) && !item.isThrowing)
        {
            _processingItems.Add(item);
            StartCoroutine(ProcessItemAttraction(item));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<WorldItem>(out WorldItem item))
            item.SetColliderAvailable(true);
    }

    private IEnumerator ProcessItemAttraction(WorldItem item)
    {
        // 禁用物品的碰撞体防止重复触发
        item.SetColliderAvailable(false);
        
        // 吸引阶段
        yield return StartCoroutine(AttractItem(item));

        int quantity = 0;
        _pickupEventChannel.RaiseEvent(new ItemPickupData{
            ItemID = item.ItemID,
            Quantity = item.quantity
        }, remaining=>{quantity = remaining;});

        if (quantity == 0)
        {
            ItemPool.Instance.ReturnItem(item);
        }
        item.quantity = quantity;
        _processingItems.Remove(item);
    }

    private IEnumerator AttractItem(WorldItem item)
    {
        Transform itemTransform = item.transform;
        
        while (Vector2.Distance(itemTransform.position, transform.position) > minPickupDistance)
        {
            Vector2 direction = (transform.position - itemTransform.position).normalized;
            item.SetVelocity(direction * attractSpeed);
            yield return new WaitForFixedUpdate();
        }
        item.SetVelocity(Vector2.zero);
    }

    private bool IsItemValid(Collider2D other)
    {
        return ((1 << other.gameObject.layer) & itemLayer) != 0;
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position+new Vector3(0, 1, 0), pickupRadius);
    }
    #endif
}

