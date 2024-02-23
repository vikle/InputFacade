using UnityEngine.InputSystem;

public enum EControlType : byte
{
    Button, 
    Axis, 
    Vector2
};

public sealed class InputActionWrapper
{
    public bool enabled;

    public InputActionData Data { get; }
    
    InputActionPhase m_phase;
    readonly InputAction m_action;
    readonly EControlType m_controlType;

    public InputActionWrapper(InputAction action)
    {
        m_action = action;
        enabled = action.enabled;
        m_controlType = System.Enum.Parse<EControlType>(action.expectedControlType);
        Data = new() { guid = action.id.ToString() };
    }
    
    public void OnUpdate()
    {
        if (enabled == false)
        {
            ResetData();
            return;
        }

        UpdatePhase();
        UpdateData();
    }

    private void UpdatePhase()
    {
        var next_phase = m_phase;
        
        if (m_action.phase < InputActionPhase.Started)
        {
            switch (next_phase)
            {
                case InputActionPhase.Performed:
                    next_phase = InputActionPhase.Canceled;
                    break;
                case InputActionPhase.Canceled: 
                    m_phase = InputActionPhase.Waiting;
                    Data.Reset();
                    return;
                default: break;
            }
        }
        else
        {
            next_phase = (next_phase < InputActionPhase.Started) 
                ? InputActionPhase.Started 
                : InputActionPhase.Performed;
        }

        if (next_phase != m_phase)
        {
            Data.UpdateState(next_phase);
        }

        m_phase = next_phase;
    }

    private void UpdateData()
    {
        if (m_phase >= InputActionPhase.Started)
        {
            Data.UpdateValue(m_action, m_controlType);
        }
    }
    
    private void ResetData()
    {
        if (m_phase == InputActionPhase.Disabled) return;
        m_phase = InputActionPhase.Disabled;
        Data.Reset();
    }
};
