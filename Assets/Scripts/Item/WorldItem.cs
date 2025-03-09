using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer), typeof(Rigidbody2D))]
[DisallowMultipleComponent]
public class WorldItem : MonoBehaviour
{
    [SerializeField] private int itemID = -1;
    [SerializeField] private float colliderScaleFactor = 0.95f;
    [SerializeField] private ItemDatabase_SO itemDatabase;
    
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
        Initialize();
    }

    private void Initialize()
    {
        itemModel = itemDatabase.GetItemByID(itemID);
        if (itemModel == null)
        {
            Debug.LogWarning($"itemModel加载失败: {itemID}");
            return;
        }
        spriteRenderer.sprite = itemModel.itemOnWorldIcon == null ? itemModel.itemIcon : itemModel.itemOnWorldIcon;
        UpdateCollider();
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

    private void ValidateSortingLayer()
    {
        if(SortingLayer.layers.All(l => l.name != "Instance"))
        {
            Debug.LogError("缺失SortingLayer: Instance", this);
        }
    }

    private void UpdateCollider()
    {
        if(spriteRenderer.sprite == null) return;

        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        Vector2 scaledSize = spriteBounds.size * colliderScaleFactor;
        
        coll.size = scaledSize;
        coll.offset = spriteBounds.center;
    }

    public void SetItemID(int newID)
    {
        if(isInitializing)
        {
            Debug.LogWarning("初始化过程中禁止修改ID");
            return;
        }
        
        itemID = newID;
        Initialize();
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
