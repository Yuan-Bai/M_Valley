using UnityEngine;
using DG.Tweening;

public class ItemSway : MonoBehaviour
{
    [Header("摇摆参数")]
    [SerializeField] private float swayAngle = 10f; // 最大摆动角度
    [SerializeField] private float duration = 0.5f;  // 单次摆动时间
    [SerializeField] private Ease easeType1 = Ease.OutQuint; // 缓动类型
    [SerializeField] private Ease easeType2 = Ease.InQuint; // 缓动类型

    private Vector3 originalRotation;
    
    void Start()
    {
        // 记录初始旋转角度
        originalRotation = transform.localEulerAngles;
        
    }

    public void StartSwayAnimation()
    {
        // 创建左右摇摆的循环动画
        Sequence swaySequence = DOTween.Sequence();
        
        // 向左摆动
        swaySequence.Append(transform.DOLocalRotate(
            new Vector3(originalRotation.x, originalRotation.y, originalRotation.z + swayAngle),
            duration)
            .SetEase(easeType1));
        
        // 向右摆动
        swaySequence.Append(transform.DOLocalRotate(
            new Vector3(originalRotation.x, originalRotation.y, originalRotation.z - swayAngle),
            duration)
            .SetEase(easeType2));
            
        // 设置无限循环（-1表示无限）
        swaySequence.SetLoops(-1, LoopType.Yoyo);
    }

    public void SwayLeft()
    {
        // 创建左右摇摆的循环动画
        Sequence swaySequence = DOTween.Sequence();
        
        // 向左摆动
        swaySequence.Append(transform.DOLocalRotate(
            new Vector3(originalRotation.x, originalRotation.y, originalRotation.z + swayAngle),
            duration)
            .SetEase(easeType1));
        
        // 回归
        swaySequence.Append(transform.DOLocalRotate(
            new Vector3(originalRotation.x, originalRotation.y, originalRotation.z),
            duration)
            .SetEase(easeType2));
            
        swaySequence.SetLoops(1, LoopType.Yoyo);
    }

    public void SwayRight()
    {
        // 创建左右摇摆的循环动画
        Sequence swaySequence = DOTween.Sequence();
        
        // 向右摆动
        swaySequence.Append(transform.DOLocalRotate(
            new Vector3(originalRotation.x, originalRotation.y, originalRotation.z - swayAngle),
            duration)
            .SetEase(easeType1));
        
        // 回归
        swaySequence.Append(transform.DOLocalRotate(
            new Vector3(originalRotation.x, originalRotation.y, originalRotation.z),
            duration)
            .SetEase(easeType2));
            
        swaySequence.SetLoops(1, LoopType.Yoyo);
    }

    void OnDestroy()
    {
        // 清理DOTween动画
        transform.DOKill();
    }
}
