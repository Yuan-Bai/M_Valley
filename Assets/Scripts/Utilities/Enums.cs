public enum ItemType
{
    Seed, Commodity, Furniture,
    HoeTool, ChopTool, BreakTool, ReapTool, WaterTool, CollectTool, PickAxe,
    ReapableScenery,
}

public enum SlotType
{
    Box, Bag, Shop,
}

public enum ActionType
{
    None,Carry,Hold,
}

public enum PartType
{
    Hair,Body,Arm,
}

// public enum ItemCategory
// {
//     Seed,
//     Commodity,
//     Furniture,
//     Tool,
//     ReapableScenery,
// }

// public enum ToolType
// {
//     Hoe, Chop, Break, Reap, Water, Collect,
// }

// [System.Flags]
// public enum ItemAttributes
// {
//     None = 0,
//     CanPickedUp = 1 << 0,
//     CanDropped = 1 << 1,
//     CanCarried = 1 << 2,
//     IsConsumable = 1 << 3,
// }