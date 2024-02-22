using UnityEngine.InputSystem;

public sealed class InputMapWrapper
{
    bool m_enabled;
    readonly InputActionMap m_map;
    readonly InputActionWrapper[] m_actions;

    public InputActionWrapper[] Actions => m_actions;
    
    public string Name => m_map.name;

    public bool Enabled
    {
        get => m_enabled;
        set
        {
            if (value) m_map.Enable();
            else m_map.Disable();
                
            m_enabled = value;

            for (int i = 0, i_max = m_actions.Length; i < i_max; i++)
            {
                var action = m_actions[i];
                action.enabled = value;
                action.OnUpdate();
            }
        }
    }

    public InputMapWrapper(InputActionMap map)
    {
        m_map = map;
        m_enabled = map.enabled;
        
        var actions = map.actions;
        m_actions = new InputActionWrapper[actions.Count];
        
        for (int i = 0, i_max = actions.Count; i < i_max; i++)
        {
            m_actions[i] = new(actions[i]);
        }
    }

    public void OnUpdate()
    {
        for (int i = 0, i_max = m_actions.Length; i < i_max; i++)
        {
            m_actions[i].OnUpdate();
        }
    }
};
