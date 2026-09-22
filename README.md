# M_Valley · 2D 农场原型（学习项目）

Unity 2022.2（URP 2D）· C# · DOTween · Input System

2025 年初的 2D 农场小原型：跟随 B 站 M_Studio（麦扣）《M Valley》教程入门，随后在教程之外做了结构重构与功能自研。

## 起源说明（诚实版）

- 基础框架：M_Studio《M Valley》系列教程；本仓库在其进度之上做了下列自主改动
- 教程素材（`Assets/M Studio/`）因体积与版权原因未纳入仓库（`.gitignore`），**克隆后直接打开会缺美术引用，无法直接运行**

## 自主重构 / 自研部分

- **SO 事件通道**：10 个 ScriptableObject 事件通道，替代教程中的直接引用/单例通信
- **背包 MVC + 增量更新**：`InventoryModel`（纯 C#，槽位数组 + 变更快照）与 View/Controller 分离，UI 仅更新变化槽位
- **自研 A\***：`Astar.cs` / `Node : IComparable`，含路径可视化
- **编辑器工具**：UIElements 物品编辑窗口 + `SceneNameDrawer`（场景名下拉 PropertyDrawer）
- **其他**：物品投掷伪 3D 抛物线、粒子对象池、跨场景物品保留

## 已知问题（2026-09 复盘修复）

- 构建阻断：`GridMap.cs` 的 `using UnityEditor` 未加条件编译（已修复）
- `InventoryView` 事件解绑笔误、A* `pathFound` 未重置、`GridNodes` 边界越界、背包空槽残留、`Teleport` 缺花括号（已修复）
- 尚未完成：存档、音效、NPC 行为、`ItemPool.GetItem`（对象池复用）

## 运行

1. Unity 2022.2 打开工程
2. 补齐教程素材到 `Assets/M Studio/`（或用任意 Sprite 替换缺失引用）
3. 打开 `Assets/Scenes/PersistentScene.unity` 运行
