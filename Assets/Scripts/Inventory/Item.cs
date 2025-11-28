using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory
{
    public class Item : MonoBehaviour
    {
        public int itemID;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D coll;
        //[HideInInspector]
        public ItemDetails itemDetails;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (itemID != 0)
            {
                Init(itemID);
            }
        }
        public void Init(int ID)
        {
            itemID = ID;
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
                if (itemDetails != null)
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite != null ? itemDetails.itemOnWorldSprite : itemDetails.itemIcon;

                //根据图片大小匹配碰撞大小
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }

}

