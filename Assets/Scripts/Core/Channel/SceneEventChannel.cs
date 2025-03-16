using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SceneEventChannel", menuName = "Events/SceneEventChannel")]
public class SceneEventChannel : ScriptableObject
{
    public event UnityAction OnBeforeSceneUnload;
    public event UnityAction OnAfterSceneLoad;

    public event UnityAction<TeleportData> OnTeleportRequested;

    public void RaiseBeforeSceneUnload()
    {
        OnBeforeSceneUnload?.Invoke();
    }

    public void RaiseAfterSceneLoad()
    {
        OnAfterSceneLoad?.Invoke();
    }

    public void RaiseTeleportRequested(TeleportData teleportData)
    {
        OnTeleportRequested?.Invoke(teleportData);
    }
}
