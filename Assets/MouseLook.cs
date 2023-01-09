using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public static int MouseSensitivity { get; set; }

    public Transform playerBody;

    float xRotation = 0f;

    
    // Start is called before the first frame update
    private void Start()
    {
        MouseSensitivity = PlayerPrefs.HasKey("MouseSensitivitySetting")? PlayerPrefs.GetInt("MouseSensitivitySetting"): 300;
    }
    
    // Update is called once per frame
    private void Update()
    {
        var mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        var mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
