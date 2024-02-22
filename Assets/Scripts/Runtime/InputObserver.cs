using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public sealed class InputObserver : MonoBehaviour
{
    public static InputObserver Get => (s_instance == null) 
        ? (s_instance = FindObjectOfType<InputObserver>()) 
        : s_instance;

    static InputObserver s_instance;
    
    public InputActionAsset asset;
    public string activeMap = "Gameplay";

    InputMapWrapper[] m_maps;
    Dictionary<string, InputActionData> m_dataMap;

    void Awake()
    {
        s_instance = this;
        InitInputMaps();
        InitDataMap();
        ApplyMap();
    }

    private void InitInputMaps()
    {
        var maps = asset.actionMaps;
        m_maps = new InputMapWrapper[maps.Count];
        
        for (int i = 0, i_max = maps.Count; i < i_max; i++)
        {
            m_maps[i] = new(maps[i]);
        }
    }

    private void InitDataMap()
    {
        m_dataMap = new();
        
        foreach (var map in m_maps)
        {
            foreach (var action in map.Actions)
            {
                m_dataMap[action.Data.guid] = action.Data;
            }
        }
        
    }

    public void SwitchMap(string mapName)
    {
        if (activeMap != mapName) ApplyMap();
    }
    
    private void ApplyMap()
    {
        foreach (var map in m_maps)
        {
            map.Enabled = (map.Name == activeMap);
        }
    }

    void Update()
    {
        for (int i = 0, i_max = m_maps.Length; i < i_max; i++)
        {
            m_maps[i].OnUpdate();
        }
    }
    
    public void Bind(ref InputActionData target)
        => target = m_dataMap[target.guid];
};

public static class InputObserverTool
{
    public static void SwitchMap(string mapName)
        => InputObserver.Get.SwitchMap(mapName);

    public static void Bind(ref InputActionData target)
        => InputObserver.Get.Bind(ref target);
};