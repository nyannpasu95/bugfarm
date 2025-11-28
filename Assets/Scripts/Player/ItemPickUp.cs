using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Inventory;
public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            if (item.itemDetails.canPickedup)
            {
                InventoryManager.Instance.AddItem(item, true,1);
            }
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }
   
}
