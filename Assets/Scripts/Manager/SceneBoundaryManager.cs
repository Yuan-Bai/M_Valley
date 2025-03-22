using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections;

public class SceneBoundaryManager : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner confiner;

    [Header("事件通道")]
    [SerializeField] private SceneEventChannel _sceneEventChannel;

    private Collider2D _currentBounds;

    private void OnEnable()
    {
        _sceneEventChannel.OnAfterSceneLoad += HandleAfterSceneLoad;
    }

    private void OnDisable()
    {
        _sceneEventChannel.OnAfterSceneLoad -= HandleAfterSceneLoad;
    }

    private void HandleAfterSceneLoad()
    {
        FindNewBoundary();
    }

    /// <summary>
    /// 使用协程寻找新的相机边界
    /// </summary>
    private void FindNewBoundary() 
    {
        // 延迟一帧确保新场景物体加载完成
        StartCoroutine(FindBoundsCoroutine());
    }

    private IEnumerator FindBoundsCoroutine() 
    {
        yield return null; // 等待场景物体初始化
        
        GameObject boundsObj = GameObject.FindGameObjectWithTag("CameraBoundary");
        if (boundsObj != null && boundsObj.TryGetComponent(out Collider2D newBounds)) {
            if(newBounds != _currentBounds) {
                _currentBounds = newBounds;
                confiner.m_BoundingShape2D = _currentBounds;
                confiner.InvalidatePathCache(); // 强制刷新边界
                Debug.Log($"已更新摄像机边界: {_currentBounds.name}");
            }
        } else {
            Debug.LogWarning($"场景 {SceneManager.GetActiveScene().name} 未找到有效边界!");
        }
    }

    /// <summary>
    /// 多区域复合边界
    /// </summary>
    /// <param name="boundsArray">多区域边界</param>
    private void CombineBounds(Collider2D[] boundsArray)
    {
        GameObject compositeObj = new GameObject("CompositeBounds");
        CompositeCollider2D composite = compositeObj.AddComponent<CompositeCollider2D>();
        foreach (Collider2D bounds in boundsArray)
        {
            compositeObj.transform.SetParent(bounds.transform);
            bounds.usedByComposite = true;
        }
        composite.GenerateGeometry();
        confiner.m_BoundingShape2D = composite;
    }
}
