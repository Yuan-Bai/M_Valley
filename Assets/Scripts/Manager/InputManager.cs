using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    public PlayerInputControls Controls { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Controls ??= new PlayerInputControls();
    }

    protected override void OnDestroy()
    {
        // 添加释放保护
        if (Controls != null)
        {
            Controls.Dispose();
            Controls = null;
        }
        base.OnDestroy();
    }
}
