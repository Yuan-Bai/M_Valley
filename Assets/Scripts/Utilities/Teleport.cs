using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TeleportData
{
    public Vector3 targetPosition;
    public string targetScene;
}

[RequireComponent(typeof(BoxCollider2D))]
public class Teleport : MonoBehaviour
{
    [Header("传送参数")]
    [SerializeField] [SceneName] private string _sceneTo;
    [SerializeField] private Vector3 _positionToGo;
    
    [Header("事件通道")]
    [SerializeField] private SceneEventChannel _sceneEventChannel;

    [Header("传送保护")]
    [SerializeField] private float _cooldown = 1f; 
    private bool _isReady = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isReady || !other.CompareTag("Player")) 
            return;

        // 锁定传送点
        _isReady = false;
        StartCoroutine(CooldownRoutine());

        // 禁用玩家输入
        if (other.TryGetComponent<PlayerControls>(out var controls))
            controls.SetInputEnabled(false);

        // 创建传送数据包
        var teleportData = new TeleportData {
            targetScene = _sceneTo,
            targetPosition = _positionToGo
        };

        // 触发事件
        _sceneEventChannel.RaiseTeleportRequested(teleportData);
    }

    private IEnumerator CooldownRoutine()
    {
        yield return new WaitForSeconds(_cooldown);
        _isReady = true;
    }
}

