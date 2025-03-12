using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SceneEventChannel", menuName = "Events/SceneEventChannel")]
public class SceneEventChannel : ScriptableObject
{
    public UnityAction OnBeforeSceneUnload;
    public UnityAction OnAfterSceneLoad;

    public void RaiseBeforeSceneUnload()
    {

    }

    public void RaiseAfterSceneLoad()
    {

    }
}
