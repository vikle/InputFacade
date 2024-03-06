using UnityEngine.InputSystem;

public sealed class InputMapWrapper
{
    public InputActionMap Map { get; }
    public InputActionWrapper[] Actions { get; }
    
    bool m_enabled;

    public bool Enabled
    {
        get => m_enabled;
        set
        {
            if (value) Map.Enable();
            else Map.Disable();
                
            m_enabled = value;

            for (int i = 0, i_max = Actions.Length; i < i_max; i++)
            {
                var act = Actions[i];
                act.Enabled = value;
                act.OnUpdate();
            }
        }
    }

    public InputMapWrapper(InputActionMap map)
    {
        Map = map;
        m_enabled = map.enabled;
        
        var actions = map.actions;
        Actions = new InputActionWrapper[actions.Count];
        
        for (int i = 0, i_max = actions.Count; i < i_max; i++)
        {
            Actions[i] = new(actions[i]);
        }
    }

    public void OnUpdate()
    {
        for (int i = 0, i_max = Actions.Length; i < i_max; i++)
        {
            Actions[i].OnUpdate();
        }
    }
};
