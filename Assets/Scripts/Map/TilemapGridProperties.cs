using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class TilemapGridProperties : MonoBehaviour
{
#if UNITY_EDITOR
    private Tilemap tilemap;
    [SerializeField] private GridProperties_SO gridProperties = null;
    [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.diggable;

    private void OnEnable()
    {
        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            tilemap = GetComponent<Tilemap>();

            if (gridProperties != null)
            {
                gridProperties.gridPropertyList.Clear();
            }
        }
    }

    private void OnDisable()
    {        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
            UpdateGridProperties();

            if (gridProperties != null)
            {
                // This is required to ensure that the updated gridproperties gameobject gets saved when the game is saved - otherwise they are not saved.
                EditorUtility.SetDirty(gridProperties);
            }
        }
    }

    private void UpdateGridProperties()
    {
        tilemap.CompressBounds();

        if (!Application.IsPlaying(gameObject) && gridProperties != null)
        {
            Vector3Int startCell = tilemap.cellBounds.min;
            Vector3Int endCell = tilemap.cellBounds.max;

            Debug.Log($"=== TilemapGridProperties 详细扫描 ===");
            Debug.Log($"Tilemap: {tilemap.name}");
            Debug.Log($"位置: {tilemap.transform.position}");
            Debug.Log($"旋转: {tilemap.transform.rotation}");
            Debug.Log($"缩放: {tilemap.transform.localScale}");
            Debug.Log($"实际扫描范围: 从 {startCell} 到 {endCell}");

            // 清空现有列表
            gridProperties.gridPropertyList.Clear();

            int tilesFound = 0;
            List<Vector2Int> foundCoordinates = new List<Vector2Int>();

            for (int x = startCell.x; x < endCell.x; x++)
            {
                for (int y = startCell.y; y < endCell.y; y++)
                {
                    TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                    if (tile != null)
                    {
                        gridProperties.gridPropertyList.Add(new GridProperty(new GridCoordinate(x, y), gridBoolProperty, true));
                        tilesFound++;
                        foundCoordinates.Add(new Vector2Int(x, y));

                        // 记录前几个找到的坐标
                        if (tilesFound <= 5)
                        {
                            Debug.Log($"找到瓦片坐标: ({x}, {y}) - 瓦片: {tile.name}");
                        }
                    }
                }
            }

            // 输出统计信息
            Debug.Log($"扫描完成: 找到 {tilesFound} 个瓦片");

            if (foundCoordinates.Count > 0)
            {
                // 计算坐标范围
                int minX = foundCoordinates.Min(c => c.x);
                int maxX = foundCoordinates.Max(c => c.x);
                int minY = foundCoordinates.Min(c => c.y);
                int maxY = foundCoordinates.Max(c => c.y);

                Debug.Log($"实际瓦片坐标范围: X[{minX} 到 {maxX}], Y[{minY} 到 {maxY}]");

                // 输出一些样本坐标
                Debug.Log("样本坐标:");
                for (int i = 0; i < Mathf.Min(10, foundCoordinates.Count); i++)
                {
                    Debug.Log($"  ({foundCoordinates[i].x}, {foundCoordinates[i].y})");
                }
            }

            Debug.Log($"=== 扫描结束 ===\n");
        }
    }

    private void Update()
    {        // Only populate in the editor
        if (!Application.IsPlaying(gameObject))
        {
          
        }
        UpdateGridProperties();
    }
#endif
}