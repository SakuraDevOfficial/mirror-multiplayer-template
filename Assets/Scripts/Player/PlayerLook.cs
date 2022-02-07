using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    
    public float mouseSensitivity = 100f;

    public Transform camTransform;

    private float _xRotation = 0f;

    [SerializeField] private bool locked = true;
    // Start is called before the first frame update
    void Start()
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;   
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasAuthority) return;
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        //camTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        //transform.Rotate(Vector3.up * mouseX);
        CmdRotate(Quaternion.Euler(_xRotation, 0f, 0f), Vector3.up * mouseX);
    }

    [Command]
    private void CmdRotate(Quaternion cameraRotation, Vector3 playerRotation)
    {
        camTransform.localRotation = cameraRotation;
        transform.Rotate(playerRotation);
        TargetRotate(cameraRotation, playerRotation);
    }
    
    [TargetRpc]
    public void TargetRotate(Quaternion cameraRotation, Vector3 playerRotation)
    {
        camTransform.localRotation = cameraRotation;
        transform.Rotate(playerRotation);
    }
}
