using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer), typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class WorldItem : MonoBehaviour
{
    [SerializeField] private int itemID = -1;
    [SerializeField] private float colliderScaleFactor = 0.95f;
    
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private ItemModel itemModel;
    private bool isInitializing;

    public int ItemID => itemID;
    public int quantity = 1;

    void Awake()
    {
        InitializeComponents();
        ValidateSortingLayer();
    }

    void Start()
    {
        if(itemID == -1)
        {
            Debug.LogError("未配置有效ItemID", this);
            return;
        }
        StartCoroutine(InitializeRoutine());
    }

    private void InitializeComponents()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer.sortingLayerName = "Instance";
        spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        coll.isTrigger = true;
        rb.gravityScale = 0;
    }

    private IEnumerator InitializeRoutine()
    {
        isInitializing = true;
        
        yield return EnsureInventoryController();
        yield return LoadItemData();
        yield return LoadSpriteAsync();
        UpdateCollider();

        isInitializing = false;
    }

    private IEnumerator EnsureInventoryController()
    {
        while (!InventoryController.IsInitialized)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator LoadItemData()
    {
        bool success = false;
        InventoryController.Instance.GetItemModelAsync(itemID, model =>
        {
            itemModel = model;
            success = model != null;
        });

        yield return new WaitUntil(() => success);
    }

    private IEnumerator LoadSpriteAsync()
    {
        Sprite sprite = itemModel.itemOnWorldIcon == null ? itemModel.itemIcon : itemModel.itemOnWorldIcon;
        yield return null;
        if(sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"Sprite加载失败: {itemID}");
        }
    }

    private void UpdateCollider()
    {
        if(spriteRenderer.sprite == null) return;

        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        Vector2 scaledSize = spriteBounds.size * colliderScaleFactor;
        
        coll.size = scaledSize;
        coll.offset = spriteBounds.center;
        
        #if UNITY_EDITOR
        DebugDrawCollider();
        #endif
    }

    #if UNITY_EDITOR
    private void DebugDrawCollider()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3)coll.offset, Color.green, 1f);
        Debug.DrawRay(transform.position + (Vector3)coll.offset, 
                     new Vector3(coll.size.x/2, coll.size.y/2, 0), 
                     Color.red, 1f);
    }
    #endif

    private void ValidateSortingLayer()
    {
        if(SortingLayer.layers.All(l => l.name != "Instance"))
        {
            Debug.LogError("缺失SortingLayer: Instance", this);
        }
    }

    public void SetItemID(int newID)
    {
        if(isInitializing)
        {
            Debug.LogWarning("初始化过程中禁止修改ID");
            return;
        }
        
        itemID = newID;
        StartCoroutine(InitializeRoutine());
    }

    public void SetColliderAvailable(bool isAble)
    {
        coll.enabled = isAble;
    }

    public void SetVelocity(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
}
