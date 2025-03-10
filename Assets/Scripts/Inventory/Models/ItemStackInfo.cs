using System.Collections.Generic;

public class ItemStackInfo {
    public int TotalCount;
    public List<SlotStack> Slots = new();
}

public class SlotStack {
    public int SlotIndex;
    public int StackCount; // 当前slot的item数量
}