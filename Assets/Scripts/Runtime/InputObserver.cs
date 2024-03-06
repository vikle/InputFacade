using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public sealed class InputObserver : MonoBehaviour
{
    public static InputObserver Get => (s_instance != null) 
        ? s_instance : (s_instance = FindObjectOfType<InputObserver>());
    static InputObserver s_instance;

    public InputActionAsset asset;
    public string currentMap = "Gameplay";

    Dictionary<string, InputActionData> m_dataMap;

    public InputMapWrapper[] Maps { get; private set; }

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
        Maps = new InputMapWrapper[maps.Count];

        for (int i = 0, i_max = maps.Count; i < i_max; i++)
        {
            Maps[i] = new(maps[i]);
        }
    }

    private void InitDataMap()
    {
        m_dataMap = new();

        foreach (var map in Maps)
        {
            foreach (var action in map.Actions)
            {
                m_dataMap[action.Data.guid] = action.Data;
            }
        }
    }

    public void SwitchMap(string mapName)
    {
        if (currentMap == mapName) return;
        currentMap = mapName;
        ApplyMap();
    }

    private void ApplyMap()
    {
        foreach (var map in Maps)
        {
            map.Enabled = (map.Map.name == currentMap);
        }
    }

    void Update()
    {
        for (int i = 0, i_max = Maps.Length; i < i_max; i++)
        {
            Maps[i].OnUpdate();
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
