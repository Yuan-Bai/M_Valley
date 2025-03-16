using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SceneEventChannel", menuName = "Events/SceneEventChannel")]
public class SceneEventChannel : ScriptableObject
{
    public event UnityAction OnBeforeSceneUnload;
    public event UnityAction OnAfterSceneLoad;

    public void RaiseBeforeSceneUnload()
    {
        OnBeforeSceneUnload?.Invoke();
    }

    public void RaiseAfterSceneLoad()
    {
        OnAfterSceneLoad?.Invoke();
    }
}
