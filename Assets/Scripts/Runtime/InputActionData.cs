using UnityEngine;

[System.Serializable]
public sealed class InputActionData
{
    public string guid;

    public bool IsStarted { get; set; }
    public bool IsPerformed { get; set; }
    public bool IsCanceled { get; set; }
    
    public float AxisValue { get; set; }
    public Vector2 Vector2Value { get; set; }

    public void Reset()
    {
        IsStarted = false;
        IsPerformed = false;
        IsCanceled = false;
        AxisValue = 0f;
        Vector2Value = Vector2.zero;
    }
};
