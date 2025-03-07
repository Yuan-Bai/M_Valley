using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new();
    private static readonly bool _isApplicationQuitting = false;

    public static T Instance
    {
        get 
        {
            if (_isApplicationQuitting) return null;
            // 互斥访问
            lock (_lock)
            {
                if (_instance == null) 
                {
                    // 先查找（包含禁用对象）
                    _instance = FindAnyObjectByType<T>(FindObjectsInactive.Include);
                    if (_instance == null)
                    {
                        // 仍然不存在，再创建
                        GameObject obj = new($"[{typeof(T).Name}]");
                        _instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }
    }

    public static bool IsInitialized
    {
        get { return _instance != null; }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }

    // 新增手动销毁方法
    public static void DestroyInstance()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}
