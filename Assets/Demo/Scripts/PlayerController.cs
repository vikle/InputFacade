using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    [Header("Input")]
    public InputActionData moveInput;
    public InputActionData lookInput;
    public InputActionData runPCEvent;
    public InputActionData runGamepadEvent;

    [Header("Settings")]
    public Transform cameraTransform;
    public float moveSpeed = 4f;
    public float runSpeed = 6f;
    public float rotSpeed = 100f;

    Transform m_transform;
    CharacterController m_controller;
    float m_rotation;
    bool m_isRun;

    void Awake()
    {
        m_transform = transform;
        m_controller = GetComponent<CharacterController>();

        InputObserverTool.Bind(ref moveInput);
        InputObserverTool.Bind(ref lookInput);
        InputObserverTool.Bind(ref runPCEvent);
        InputObserverTool.Bind(ref runGamepadEvent);
    }

    void Update()
    {
        DoMove();
        DoLook();
        DoRun();
    }

    private void DoMove()
    {
        float speed = m_isRun ? runSpeed : moveSpeed;
        var force = (moveInput.Vector2Value * (speed * Time.deltaTime));
        var move_force = (m_transform.forward * force.y + m_transform.right * force.x);
        move_force.y = -10f;
        m_controller.Move(move_force);
    }

    private void DoLook()
    {
        var force = (lookInput.Vector2Value * (rotSpeed * Time.deltaTime));
        m_transform.Rotate(0f, force.x, 0f);
        m_rotation = Mathf.Clamp(m_rotation - force.y, -60f, 60f);
        cameraTransform.localEulerAngles = new Vector3(m_rotation, cameraTransform.localEulerAngles.y, 0f);
    }

    private void DoRun()
    {
        if (m_isRun)
        {
            if (moveInput.IsCanceled || runPCEvent.IsCanceled)
            {
                m_isRun = false;
            }
        }
        else
        {
            if (moveInput.IsPerformed && (runPCEvent.IsStarted || runGamepadEvent.IsStarted))
            {
                m_isRun = true;
            }
        }
    }
};
