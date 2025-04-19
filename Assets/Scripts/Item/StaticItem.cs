using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class StaticItem : MonoBehaviour
{

    [Header("碰撞体缩放大小")]
    [SerializeField] private float colliderScaleFactor = 0.95f;

    [Header("事件通道")]
    [SerializeField] private ItemEventChannel _itemEventChannel;

    private BoxCollider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private int harvestActionCount;

    public CropModel cropModel;
    public bool harvestable;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    } 

    public void UpdateCollider()
    {
        if(_spriteRenderer.sprite == null) return;

        Bounds spriteBounds = _spriteRenderer.sprite.bounds;
        Vector2 scaledSize = spriteBounds.size * colliderScaleFactor;
        
        _collider.size = scaledSize;
        _collider.offset = spriteBounds.center;
    }

    public bool ProcessToolAction(ItemType itemType)
    {
        if (!harvestable) return false;
        if (harvestActionCount < cropModel.requireActionCount)
        {
            harvestActionCount++;
            return false;
        }
        else
        {
            SpawnHarvestItems();
            return true;
        }
    }

    private void SpawnHarvestItems()
    {
        for (int i = 0; i < cropModel.producedItemID.Length; i++)
        {
            int amountToProduce;

            if (cropModel.producedMinAmount[i] == cropModel.producedMaxAmount[i])
            {
                //代表只生成指定数量的
                amountToProduce = cropModel.producedMinAmount[i];
            }
            else    //物品随机数量
            {
                amountToProduce = Random.Range(cropModel.producedMinAmount[i], cropModel.producedMaxAmount[i] + 1);
            }
            Debug.Log("amountToProduce" + amountToProduce);
            _itemEventChannel.RaiseCreateItem(cropModel.producedItemID[i], transform.position, 2, amountToProduce);
        }
        Destroy(gameObject);
    }

}
