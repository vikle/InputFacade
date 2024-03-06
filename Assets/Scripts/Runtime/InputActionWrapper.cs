using UnityEngine.InputSystem;

public sealed class InputActionWrapper
{
    public InputAction Action { get; }
    public bool Enabled { get; set; }
    public EControlType ControlType { get; }
    public InputActionPhase Phase { get; private set; }
    public InputActionData Data { get; }

    public InputActionWrapper(InputAction action)
    {
        Action = action;
        Enabled = action.enabled;
        ControlType = System.Enum.Parse<EControlType>(action.expectedControlType);
        Data = new() { guid = action.id.ToString() };
    }

    public void OnUpdate()
    {
        if (Enabled == false)
        {
            ResetData();
            return;
        }

        UpdatePhase();
        UpdateData();
    }

    private void UpdatePhase()
    {
        var next_phase = Phase;

        if (Action.phase < InputActionPhase.Started)
        {
            switch (next_phase)
            {
                case InputActionPhase.Performed:
                    next_phase = InputActionPhase.Canceled;
                    break;
                case InputActionPhase.Canceled:
                    next_phase = InputActionPhase.Waiting;
                    Data.Reset();
                    break;
                default: break;
            }
        }
        else
        {
            next_phase = (next_phase < InputActionPhase.Started)
                ? InputActionPhase.Started
                : InputActionPhase.Performed;
        }

        if (next_phase == Phase) return;
        Data.UpdateState(next_phase);
        Phase = next_phase;
    }

    private void UpdateData()
    {
        if (Phase >= InputActionPhase.Started)
        {
            Data.UpdateValue(Action, ControlType);
        }
    }

    private void ResetData()
    {
        if (Phase == InputActionPhase.Disabled) return;
        Phase = InputActionPhase.Disabled;
        Data.Reset();
    }
};
