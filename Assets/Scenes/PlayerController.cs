using UnityEngine;

public sealed class PlayerController : MonoBehaviour
{
    [Header("Input")]
    public InputActionData moveData;
    public InputActionData lookData;
    public InputActionData fireEvent;

    [Header("Settings")]
    public Transform cameraTransform;
    public float moveSpeed = 3f;
    public float rotSpeed = .5f;

    Transform m_transform;
    CharacterController m_controller;
    float m_rotation;


    // is called before the first frame update
    void Start()
    {
        m_transform = transform;
        m_controller = GetComponent<CharacterController>();

        InputObserverTool.Bind(ref moveData);
        InputObserverTool.Bind(ref lookData);
        InputObserverTool.Bind(ref fireEvent);
    }

    // is called once per frame
    void Update()
    {
        DoMove();
        DoLook();
        DoFire();
    }

    private void DoMove()
    {
        var force = (moveData.Vector2Value * moveSpeed);
        var move_force = (m_transform.forward * force.y + m_transform.right * force.x);
        move_force.y = -10f;
        m_controller.Move(move_force * Time.deltaTime);
    }

    private void DoLook()
    {
        var force = (lookData.Vector2Value * rotSpeed);
        m_transform.Rotate(0f, force.x, 0f);
        m_rotation += force.y;
        m_rotation = Mathf.Clamp(m_rotation, -60f, 60f);
        cameraTransform.localEulerAngles = new Vector3(-m_rotation, cameraTransform.localEulerAngles.y, 0f);
    }

    private void DoFire()
    {
        if (fireEvent.IsStarted == false) return;
        
        var primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        primitive.transform.position = (cameraTransform.position + cameraTransform.right);
        primitive.transform.localScale = (Vector3.one * .2f);
        
        var r_body = primitive.AddComponent<Rigidbody>();
        r_body.AddForce(cameraTransform.forward * Random.Range(25f, 35f) 
                      + cameraTransform.right * Random.Range(-2f, 2f) 
                      + cameraTransform.up * Random.Range(-2f, 2f), 
                        ForceMode.Impulse);
        
        Destroy(primitive, 3.5f);
    }
};
