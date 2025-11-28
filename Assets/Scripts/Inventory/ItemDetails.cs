using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemDetails 
{
    [Header("物品信息配置")]
    public int itemID;                     // ① 物品唯一ID
    public string name;                    // ② 物品名称（可在背包或UI显示）
    public ItemType itemType;              // ③ 物品类型（枚举：种子、作物、工具、家具等）
    public Sprite itemIcon;                // ④ 背包里显示的图标
    public Sprite itemOnWorldSprite;       // ⑤ 世界中（掉落物）显示的图像
    public string itemDescription;         // ⑥ 物品描述文本
    public int itemUseRadius;              // ⑦ 使用范围（比如锄地、浇水的距离）
    public bool canPickedup;               // ⑧ 能否被玩家拾取
    public bool canDropped;                // ⑨ 能否丢弃（从背包拖出丢地上）
    public bool canCarried;                // ⑩ 能否被举起（如抱起南瓜箱、木桶）
    public int itemPrice;                  // ⑪ 基础售价
    [Range(0, 1)]
    public float sellPercentage;           // ⑫ 出售折扣（如50%则商店卖价为原价一半）
}
