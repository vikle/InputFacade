using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public sealed class InputActionData
{
    public string guid;

    public bool IsStarted { get; private set; }
    public bool IsPerformed { get; private set; }
    public bool IsCanceled { get; private set; }
    
    public float AxisValue { get; private set; }
    public Vector2 Vector2Value { get; private set; }


    public void UpdateState(InputActionPhase phase)
    {
        IsStarted = (phase == InputActionPhase.Started);
        IsPerformed = (phase == InputActionPhase.Performed);
        IsCanceled = (phase == InputActionPhase.Canceled);
    }

    public void UpdateValue(InputAction source, EControlType controlType)
    {
        switch (controlType)
        {
            case EControlType.Axis:
                AxisValue = source.ReadValue<float>();
                break;
            case EControlType.Vector2: 
                Vector2Value = source.ReadValue<Vector2>();
                break;
            default: break;
        }
    }
    
    public void Reset()
    {
        IsStarted = false;
        IsPerformed = false;
        IsCanceled = false;
        AxisValue = 0f;
        Vector2Value = Vector2.zero;
    }
};
