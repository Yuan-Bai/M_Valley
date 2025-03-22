using UnityEngine;
using UnityEngine.UI;

public class UISceneLoader : MonoBehaviour
{
    [SerializeField] private UIAssetBridge _uiAssetBridge;
    [SerializeField] private CanvasGroup _fadeCanvasGroup;
    [SerializeField] private Image _cursorImage;

    private void Awake()
    {
        if (_uiAssetBridge != null)
        {
            _uiAssetBridge.FadeCanvasGroup = _fadeCanvasGroup;
            _uiAssetBridge.CursorImage = _cursorImage;
            Debug.Log("UI 组件注册完成");
        }
        else
        {
            Debug.LogError("UI Asset Bridge 未配置");
        }
    }
}

