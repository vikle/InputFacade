using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    [Header("Input")]
    public InputActionData moveData;
    public InputActionData lookData;

    [Header("Settings")]
    public Transform cameraTransform;
    public float moveSpeed = 4f;
    public float rotSpeed = 100f;

    Transform m_transform;
    CharacterController m_controller;
    float m_rotation;

    void Awake()
    {
        m_transform = transform;
        m_controller = GetComponent<CharacterController>();
        
        InputObserverTool.Bind(ref moveData);
        InputObserverTool.Bind(ref lookData);
    }

    void Update()
    {
        DoMove();
        DoLook();
    }

    private void DoMove()
    {
        var force = (moveData.Vector2Value * (moveSpeed * Time.deltaTime));
        var move_force = (m_transform.forward * force.y + m_transform.right * force.x);
        move_force.y = -10f;
        m_controller.Move(move_force);
    }

    private void DoLook()
    {
        var force = (lookData.Vector2Value * (rotSpeed * Time.deltaTime));
        m_transform.Rotate(0f, force.x, 0f);
        m_rotation += force.y;
        m_rotation = Mathf.Clamp(m_rotation, -60f, 60f);
        cameraTransform.localEulerAngles = new Vector3(-m_rotation, cameraTransform.localEulerAngles.y, 0f);
    }
};
