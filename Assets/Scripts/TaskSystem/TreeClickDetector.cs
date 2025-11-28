using UnityEngine;

public class TreeClickDetector : MonoBehaviour
{
    [Header("Tree Reference")]
    public TreeController tree;

    private void OnMouseDown()
    {
        if (tree != null)
        {
            tree.OnTreeClicked();
        }
    }
}
