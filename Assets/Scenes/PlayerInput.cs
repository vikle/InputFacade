using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public InputActionData actionData;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        InputObserverTool.Bind(ref actionData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
