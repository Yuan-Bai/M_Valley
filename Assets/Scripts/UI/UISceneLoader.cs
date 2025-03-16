using UnityEngine;

// UISceneLoader.cs
public class UISceneLoader : MonoBehaviour
{
    [SerializeField] private UIAssetBridge _uiAssetBridge;
    [SerializeField] private CanvasGroup _fadeCanvasGroup;

    private void Awake()
    {
        if (_uiAssetBridge != null)
        {
            _uiAssetBridge.FadeCanvasGroup = _fadeCanvasGroup;
            Debug.Log("UI 组件注册完成");
        }
        else
        {
            Debug.LogError("UI Asset Bridge 未配置");
        }
    }
}

