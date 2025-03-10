using System;
using System.Collections;
using System.Linq;
using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static bool _isApplicationQuitting;
    public static bool IsAvailable => !_isApplicationQuitting && _instance != null;

    public static T Instance
    {
        get
        {
            if (_isApplicationQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>(true);
                if (_instance == null)
                {
                    var obj = new GameObject($"[{typeof(T).Name}]");
                    _instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);

        // 保证切换场景时不会自动销毁
        gameObject.hideFlags = HideFlags.None;
    }

    protected virtual void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

