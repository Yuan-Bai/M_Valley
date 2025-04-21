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
    private BoxCollider2D _coll;
    private Rigidbody2D _rb;
    private ItemModel itemModel;
    private bool isInitializing;

    public bool isThrowing;
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
        _coll = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        spriteRenderer.sortingLayerName = "Instance";
        spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        _coll.isTrigger = true;
        _rb.gravityScale = 0;
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
        
        _coll.size = scaledSize;
        _coll.offset = spriteBounds.center;
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
        _coll.enabled = isAble;
    }

    public IEnumerator SetColliderDisableForSeconds(float seconds)
    {
        _coll.enabled = false;
        // coll.excludeLayers 
        yield return new WaitForSeconds(seconds);
        _coll.enabled = true;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rb.velocity = velocity;
    }

    public void Throw(Vector3 velocity, float totalTime, float gravity)
    {
        StartCoroutine(ThrowRoutine(velocity, totalTime, gravity));
    }

    private IEnumerator ThrowRoutine(Vector3 velocity, float totalTime, float gravity)
    {
        isThrowing = true;
        // 运动逻辑
        float costTime = 0;
        float vx = velocity.x;
        float vy = velocity.y;
        float vz = velocity.z;
        float cos45 = Mathf.Cos(Mathf.PI/4);

        while (costTime < totalTime)
        {
            costTime += Time.deltaTime;
            vz -= Time.deltaTime * gravity;
            
            // 使用伪3D坐标转换
            Vector2 movement = new Vector2(
                vx * Time.deltaTime,
                (vy + vz ) * cos45 * Time.deltaTime // 调整投影系数
            );
            transform.Translate(movement, Space.World);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        isThrowing = false;
    }

}
