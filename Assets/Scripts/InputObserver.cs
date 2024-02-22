using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public sealed class InputObserver : MonoBehaviour
{

    public InputActionAsset asset;
    
    public string activeMap = "Gameplay";


    InputActionMap[] m_actionMaps;
    
    
    void Awake()
    {
        m_actionMaps = asset.actionMaps.ToArray();
    }

    void Start()
    {
        ApplyInputMap();
    }

    public void SetInputMap(string mapName)
    {
        if (activeMap != mapName) ApplyInputMap();
    }
    
    private void ApplyInputMap()
    {
        foreach (var map in m_actionMaps)
        {
            if (map.name == activeMap) map.Enable();
            else map.Disable();
        }
    }

    void Update()
    {
        for (int i = 0, i_max = m_actionMaps.Length; i < i_max; i++)
        {
            // InputAction
        }
    }



};
