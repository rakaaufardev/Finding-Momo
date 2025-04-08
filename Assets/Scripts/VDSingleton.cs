using UnityEngine;

public class VDSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T get;
    private static readonly object _lock = new object();

    public static T Get
    {
        get
        {
            lock (_lock)
            {
                return get;
            }

        }
    }

    protected virtual void Awake()
    {
        lock (_lock)
        {
            if (get == null)
            {
                get = this as T;
            }
        }
    }
}