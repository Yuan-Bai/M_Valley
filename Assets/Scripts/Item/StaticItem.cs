using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class StaticItem : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private Animator _animator;
    [Header("碰撞体缩放大小")]
    [SerializeField] private bool _isAutoGenerateCollider = true;
    [SerializeField] private float _colliderScaleFactor = 0.95f;

    [Header("事件通道")]
    [SerializeField] private ItemEventChannel _itemEventChannel;

    private BoxCollider2D _collider;
    private SpriteRenderer _spriteRenderer;
    private int harvestActionCount;
    private int requireActionCount;
    private bool hasAnimation;

    public CropModel cropModel;
    public bool harvestable;

    void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(CropModel cropModel)
    {
        this.cropModel = cropModel;
        requireActionCount = cropModel.requireActionCount;
        hasAnimation = cropModel.hasAnimation;
    }

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    } 

    public void UpdateCollider()
    {
        if(_spriteRenderer.sprite == null || !_isAutoGenerateCollider) return;

        Bounds spriteBounds = _spriteRenderer.sprite.bounds;
        Vector2 scaledSize = spriteBounds.size * _colliderScaleFactor;
        
        _collider.size = scaledSize;
        _collider.offset = spriteBounds.center;
    }

    public bool ProcessToolAction(ItemType itemType)
    {
        if (!harvestable || !CheakToolType(itemType)) return false;
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;
            if (hasAnimation)
            {
                _animator.SetTrigger("Shake");
            }
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
            _itemEventChannel.RaiseCreateItem(cropModel.producedItemID[i], transform.position, 2, amountToProduce);
        }
        if (hasAnimation)
        {
            _animator.SetTrigger("LeftFall");
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool CheakToolType(ItemType itemType)
    {
        foreach(ItemType tpye in cropModel.harvestToolType)
        {
            if (tpye == itemType)
                return true;
        }
        return false;
    }

    private void AnimationEnd()
    {
        if (cropModel.hasTransferItem)
        {
            _spriteRenderer.sprite = cropModel.transferItemSprite;
            requireActionCount = cropModel.transferRequireActionCount;
            harvestActionCount = 0;
            hasAnimation = false;
            _animator.enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
