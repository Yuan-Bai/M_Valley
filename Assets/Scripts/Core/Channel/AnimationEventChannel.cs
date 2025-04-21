using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AnimationEventChannel", menuName = "Events/AnimationEventChannel")]
public class AnimationEventChannel : ScriptableObject
{
    public event UnityAction OnTreeFallEnd;

    public void RaiseTreeFallEnd()
    {
        OnTreeFallEnd?.Invoke();
    }
}
