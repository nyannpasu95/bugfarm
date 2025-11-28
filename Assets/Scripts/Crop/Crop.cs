using System.Collections;
using UnityEngine;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0;

    [Tooltip("This should be populated from child transform gameobject showing harvest effect spawn point")]
    [SerializeField] private Transform harvestActionEffectTransform = null;

    [Tooltip("This should be populated from child gameobject")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;

    [HideInInspector]
    public Vector2Int cropGridPosition;

    private void SpawnHarvestEffect()
    {
        if (harvestActionEffectTransform != null)
        {
            GameObject effectPrefab = Resources.Load<GameObject>("HarvestEffect"); // 资源路径根据实际调整
            GameObject effectInstance = Instantiate(effectPrefab, harvestActionEffectTransform.position, Quaternion.identity);

            // 可选：自动销毁特效，假设动画长度 1 秒
            Destroy(effectInstance, 1f);
        }
    }

    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolDown, bool isToolUp)
    {
        // Get grid property details
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null)
            return;


        // Get animator for crop if present
        Animator animator = GetComponentInChildren<Animator>();

        // Trigger tool animation
        if (animator != null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolLeft || isToolDown)
            {
                animator.SetTrigger("usetoolleft");
            }
        }

        // Trigger tool particle effect on crop




        // Increment harvest action count
        harvestActionCount += 1;
        SpawnHarvestEffect();
    }

    private void HarvestCrop(bool isUsingToolRight, bool isUsingToolUp, CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {

        // Is there a harvested animation
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            // If harvest sprite then add to sprite renderer
            if (cropDetails.harvestedSprite != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }
            }

            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            }
            else
            {
                animator.SetTrigger("harvestleft");
            }
        }




        // Delete crop from grid properties
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        // Should the crop be hidden before the harvested animation
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }

        // Should box colliders be disabled before harvest
        if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
        {
            // Disable any box colliders
            Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
            foreach (Collider2D collider2D in collider2Ds)
            {
                collider2D.enabled = false;
            }
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Is there a harvested animation - Destroy this crop game object after animation completed
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionsAfterAnimation(cropDetails, gridPropertyDetails, animator));
        }
        else
        {

            HarvestActions(cropDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionsAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }

        HarvestActions(cropDetails, gridPropertyDetails);
    }

    private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);

        // Does this crop transform into another crop
        if (cropDetails.harvestedTransformItemCode > 0)
        {
            CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
        }


        Destroy(gameObject);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        // --- 关键：把作物产出转为“在场景中生成掉落物（Item）”事件 ---
        // 使用你已有的 EventHandler.CallInstantiateItemInScene(int ID, Vector3 pos)
        // 我们在作物当前世界位置附近生成掉落物（你可以按需微调位置偏移）

        Vector3 basePos = transform.position;
        Vector3 spawnOffset = Vector3.zero;

        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int itemID = cropDetails.cropProducedItemCode[i];

            // 计算数量
            int cropsToProduce;
            if (cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] ||
                cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
            {
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            }
            else
            {
                cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);
            }

            // 为每一个产物生成指定数量的掉落物（每个掉落物都触发生成事件）
            for (int j = 0; j < cropsToProduce; j++)
            {
                // 在作物位置稍微偏移生成，避免重叠
                spawnOffset = new Vector3((Random.value - 0.5f) * 0.3f, (Random.value - 0.5f) * 0.3f, 0f);
                Vector3 spawnPos = basePos + spawnOffset;

                // 调用全局事件去生成 Item（你的 Item/ItemBase 监听这个事件并 Instantiate 对应 prefab 并 Init(ID)）
                EventHandler.CallInstantiateItemInScene(itemID, spawnPos);
            }
        }
    }

    private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        // Update crop in grid properties
        gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
        gridPropertyDetails.growthDays = 0;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daysSinceWatered = -1;

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        // Display planted crop
        GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
    }


}
