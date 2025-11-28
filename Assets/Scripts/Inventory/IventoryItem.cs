using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
/// <summary>
/// 这个是在背包中的物品，具有两个变量，物品的ID，及背包中该物品的数量=》也适用于商店的物品结构
/// </summary>
public struct InventoryItem
{
    //使用结构而不是类，是因为类需要进行判空，可能会导致一些意想不到的bug
    public int itemID;
    public int itemAmount;

}