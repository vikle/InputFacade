using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public sealed class GameCore : MonoBehaviour
{
    public static GameCore Get => (s_instance != null) 
        ? s_instance : (s_instance = FindObjectOfType<GameCore>());
    static GameCore s_instance;
    
    [Header("Input")]
    public InputActionData menuEvent;

    public string gameplayInputMap = "Gameplay";
    public string uiInputMap = "UI";
    
    [Header("Settings")]
    public UIController uiController;
    
    
    public bool IsPaused { get; private set; }
    
    public event Action<bool> OnSwitchGamePause = delegate { };
    

    void Awake()
    {
        s_instance = this;
        uiController.InjectDependencies(this);
        uiController.Init();
        InputObserverTool.Bind(ref menuEvent);
        InputObserverTool.SwitchMap(gameplayInputMap);
        Time.timeScale = 1f;
    }

    void Update()
    {
        Cursor.visible = IsPaused;
        Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        
        if (IsPaused == false && menuEvent.IsStarted)
        {
            SwitchGamePause();
        }
    }

    public void SwitchGamePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        InputObserverTool.SwitchMap(IsPaused ? uiInputMap : gameplayInputMap);
        OnSwitchGamePause.Invoke(IsPaused);
    }
};
