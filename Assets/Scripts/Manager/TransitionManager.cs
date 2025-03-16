using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [Header("参数")]
    [SerializeField] [SceneName] private string _startScene;
    [SerializeField] private float _fadeDuration;

    [Header("组件获取")]
    [SerializeField] private UIAssetBridge _uiAssetBridge;

    [Header("事件通道")]
    [SerializeField] private SceneEventChannel _sceneEventChannel;

    [SceneName] private string _currentScene;
    private CanvasGroup _fadeCanvasGroup;
    private bool _isFade;

    void OnEnable()
    {
        _sceneEventChannel.OnTeleportRequested += HandleTeleportRequested;
    }

    void OnDisable()
    {
        _sceneEventChannel.OnTeleportRequested -= HandleTeleportRequested;
    }

    private IEnumerator Start()
    {
        // 确保初始场景加载
        if (string.IsNullOrEmpty(_currentScene))
        {
            _currentScene = _startScene;
            SceneManager.LoadScene(_startScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_startScene));
        }
        // 等待 UI 场景加载完成
        while (_uiAssetBridge.FadeCanvasGroup == null)
        {
            yield return null;
        }
        
        _fadeCanvasGroup = _uiAssetBridge.FadeCanvasGroup;
        Debug.Log("成功获取 FadeCanvasGroup 引用");
    }

    private void HandleTeleportRequested(TeleportData teleportData)
    {
        if (_isFade) return;
        StartCoroutine(TransitionAsync(teleportData));
    }

    private IEnumerator TransitionAsync(TeleportData teleportData)
    {
        yield return FadeAsync(1);

        // 卸载旧场景（如果存在）
        if (!string.IsNullOrEmpty(_currentScene))
        {
            _sceneEventChannel.RaiseBeforeSceneUnload();
            yield return SceneManager.UnloadSceneAsync(_currentScene);
        }

        // 加载新场景并等待完成
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(teleportData.targetScene, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            yield return null;
        }
        loadOperation.allowSceneActivation = true;
        yield return loadOperation;

        // 设置活动场景
        Scene newScene = SceneManager.GetSceneByName(teleportData.targetScene);
        SceneManager.SetActiveScene(newScene);
        _currentScene = teleportData.targetScene;

        // 确保场景初始化完成
        yield return new WaitForEndOfFrame();
        _sceneEventChannel.RaiseAfterSceneLoad();

        // 坐标设置
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = teleportData.targetPosition;
            
            // 恢复输入
            if (player.TryGetComponent<PlayerControls>(out var controls))
                controls.SetInputEnabled(true);
        }

        yield return FadeAsync(0);
    }

    private IEnumerator FadeAsync(float targetAlpha)
    {
        if (_fadeCanvasGroup != null)
        {
            _isFade = true;
            _fadeCanvasGroup.blocksRaycasts = true;

            float elapsedTime = 0f;
            float startAlpha = _fadeCanvasGroup.alpha;

            while (elapsedTime <= _fadeDuration)
            {
                _fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime/_fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _fadeCanvasGroup.alpha = targetAlpha; // 线性插值永远不会等于，只会无限接近
            _fadeCanvasGroup.blocksRaycasts = false;
            _isFade = false;
        }
    }
}
