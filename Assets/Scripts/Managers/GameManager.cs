using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.DefaultInputActions;

[DefaultExecutionOrder(-1)]
public class GameManager : Singleton<GameManager>
{
    [HideInInspector]
    public PlayerInput input;

    protected override void Awake()
    {
        base.Awake();
        input = new PlayerInput();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
