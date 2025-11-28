using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[CreateAssetMenu(fileName = "so_GridProperties", menuName = "Scriptable Objects/Grid Properties")]
public class GridProperties_SO : ScriptableObject
{
    public SceneName sceneName;
    public int gridWidth;
    public int gridHeight;
    public int originX;
    public int originY;

    [SerializeField]
    public List<GridProperty> gridPropertyList;
}
