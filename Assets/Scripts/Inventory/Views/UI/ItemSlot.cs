using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    [Header("组件获取")]
    [SerializeField] Image itemIcon;
    [SerializeField] Image highlightIcon;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] Button button;

    public SlotType slotType;
    private bool isSelect;
    private ItemModel item;
    private int count;

    void Start()
    {
        isSelect = false;
        highlightIcon.enabled = false;
        itemIcon.enabled = false;
    }

    public void Initialize(ItemModel item, int count)
    {
        this.item = item;
        this.count = count;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        itemIcon.sprite = item.itemIcon;
        countText.text = count > 1 ? count.ToString() : "";
    }

    public void UpdateCount(int newCount)
    {
        countText.text = newCount > 1 ? newCount.ToString() : "";
    }

    public void ClickSlot()
    {
        isSelect = !isSelect;
        highlightIcon.enabled = isSelect;
    }


}
