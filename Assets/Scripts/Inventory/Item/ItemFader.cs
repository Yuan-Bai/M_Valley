using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ItemFader : MonoBehaviour
{
    [Header("渲染器配置")]
    public bool includeInactive = true;

    private List<SpriteRenderer> _spriteRendererList = new();

    void Awake()
    {
        // 获取自身及所有子物体的SpriteRenderer
        GetComponentsInChildren<SpriteRenderer>(includeInactive, _spriteRendererList);
        
        // 如果父物体没有SpriteRenderer，确保至少有一个子物体存在
        if (_spriteRendererList.Count == 0)
        {
            Debug.LogWarning($"Item {name} 没有找到任何SpriteRenderer");
        }
    }

    public void FadeIn()
    {
        ApplyFade(Settings.fadeTargetAlpha, Settings.fadeDuration);
    }

    public void FadeOut()
    {
        ApplyFade(1f, Settings.fadeDuration);
    }

    private void ApplyFade(float targetAlpha, float duration)
    {
        foreach (var renderer in _spriteRendererList)
        {
            if (renderer != null)
            {
                renderer.DOColor(new Color(1, 1, 1, targetAlpha), duration);
            }
        }
    }

    // 在可能使用Job的脚本中添加资源释放
    private void OnDestroy()
    {
        // 清除DOTween引用
        DOTween.Kill(GetInstanceID());
    }

}
