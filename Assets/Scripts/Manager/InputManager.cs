using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoSingleton<InputManager>
{
    public PlayerInputControls Controls { get; private set; }
    private bool isInitialized = false;

    protected override void Awake()
    {
        base.Awake();
        InitializeControls();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void InitializeControls()
    {
        if (isInitialized) return;
        
        Controls = new PlayerInputControls();
        Controls.Disable(); // 默认全局禁用
        isInitialized = true;
        
        DontDestroyOnLoad(gameObject); // 跨场景持久化
    }

    public static void EnableGameplayInput()
    {
        if (!IsValidInstance()) return;
        
        Instance.Controls.UI.Disable();
        Instance.Controls.GamePlayer.Enable();
    }

    public static void EnableUIInput()
    {
        if (!IsValidInstance()) return;
        
        Instance.Controls.GamePlayer.Disable();
        Instance.Controls.UI.Enable();
    }

    private static bool IsValidInstance() 
    {
        return Instance != null && !Instance.Equals(null) 
               && Instance.Controls != null && Instance.isInitialized;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Controls?.Disable();
        EnableGameplayInput();
    }

    protected override void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        if (Controls != null)
        {
            Controls.Dispose();
            Controls = null;
        }
        isInitialized = false;
        
        base.OnDestroy();
    }
}

