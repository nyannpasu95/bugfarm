using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        private Transform itemParent;

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        }
        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        }

        private void OnInstantiateItemInScene(int ID,Vector3 pos)
        {
            if (itemPrefab == null) return;
            if (itemParent == null) return;

            // 让生成的物体在相机前方
            pos.z = 0f;
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
        }
        // Start is called before the first frame update
        private void Start()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}

