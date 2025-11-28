using UnityEngine;

/// <summary>
/// 泛型单例模板：继承这个类的脚本都会自动变成单例。
/// 例如：public class InventoryManager : Singleton<InventoryManager> {}
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
