using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


public class MainThreadDispatcher : MonoSingleton<MainThreadDispatcher>
{
    private readonly Queue<Action> _executionQueue = new();
    private readonly object _queueLock = new();

    void Update()
    {
        lock (_queueLock)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue()?.Invoke();
            }
        }
    }

    public static void Enqueue(Action action)
    {
        if (action == null) return;

        lock (Instance._queueLock)
        {
            Instance._executionQueue.Enqueue(action);
        }
    }

    // 支持带返回值的协程
    public static void EnqueueCoroutine(IEnumerator coroutine)
    {
        Enqueue(() => Instance.StartCoroutine(coroutine));
    }

    // 安全销毁时的清理
    protected override void OnDestroy()
    {
        lock (_queueLock)
        {
            _executionQueue.Clear();
        }
    }
}

