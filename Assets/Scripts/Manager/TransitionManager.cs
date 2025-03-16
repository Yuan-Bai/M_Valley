using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] private string _startScene;

    [Header("事件通道")]
    [SerializeField] private SceneEventChannel _sceneEventChannel;
}
