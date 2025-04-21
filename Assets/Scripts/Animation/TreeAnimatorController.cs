using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeAnimatorController : MonoBehaviour
{
    [SerializeField] private AnimationEventChannel _AnimationEventChannel;

    public void RaiseTreeFallEvent()
    {
        _AnimationEventChannel.RaiseTreeFallEnd();
    }
}
