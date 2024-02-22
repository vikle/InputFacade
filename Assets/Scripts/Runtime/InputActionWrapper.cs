using System;
using UnityEngine;
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

    public InputActionData Data { get; } = new();
    
    InputActionPhase m_phase;
    InputActionPhase m_dirtyPhase;
    readonly EControlType m_controlType;
    readonly InputAction m_action;

    public InputActionWrapper(InputAction action)
    {
        enabled = action.enabled;
        
        m_controlType = Enum.Parse<EControlType>(action.expectedControlType);
        m_action = action;

        m_dirtyPhase = m_phase = InputActionPhase.Waiting;
        action.started += ctx => m_dirtyPhase = InputActionPhase.Started;
        action.performed += ctx => m_dirtyPhase = InputActionPhase.Performed;
        action.canceled += ctx => m_dirtyPhase = InputActionPhase.Canceled;

        Data.guid = action.id.ToString();
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
        switch (m_dirtyPhase)
        {
            case InputActionPhase.Performed:
                if (m_phase <= InputActionPhase.Waiting) m_phase = InputActionPhase.Started;
                else if (m_phase == InputActionPhase.Started) m_phase = InputActionPhase.Performed;
                break;

            case InputActionPhase.Canceled:
                switch (m_phase)
                {
                    case InputActionPhase.Started:
                        m_phase = InputActionPhase.Performed;
                        break;
                    case InputActionPhase.Performed:
                        m_phase = InputActionPhase.Canceled;
                        break;
                    case InputActionPhase.Canceled:
                        m_phase = InputActionPhase.Waiting;
                        break;

                    default:
                        m_dirtyPhase = InputActionPhase.Waiting;
                        break;
                }
                break;

            default:
                m_phase = m_dirtyPhase;
                break;
        }
    }

    private void ResetData()
    {
        if (m_phase == InputActionPhase.Disabled) return;
        m_dirtyPhase = m_phase = InputActionPhase.Disabled;
        Data.Reset();
    }

    private void UpdateData()
    {
        Data.IsStarted = (m_phase == InputActionPhase.Started);
        Data.IsPerformed = (m_phase == InputActionPhase.Performed);
        Data.IsCanceled = (m_phase == InputActionPhase.Canceled);
        
        switch (m_controlType)
        {
            case EControlType.Axis:
                Data.AxisValue = m_action.ReadValue<float>();
                break;
            case EControlType.Vector2: 
                Data.Vector2Value = m_action.ReadValue<Vector2>();
                break;
            default: break;
        }
    }
};
