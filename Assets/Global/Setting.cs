using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //InputSystem.DisableDevice(Mouse.current);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
