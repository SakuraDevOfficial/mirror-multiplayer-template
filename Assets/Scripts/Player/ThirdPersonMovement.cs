using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ThirdPersonMovement : NetworkBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float gravity = -9.81f;
    Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float jumpHeight = 3f;

    private bool isGrounded;

    private float previousAngle;
    
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int Jumps = Animator.StringToHash("Jumps");

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        cam.gameObject.SetActive(true);
        GetComponent<PlayerLook>().enabled = true;
    }
    private void Start()
    {
        enabled = hasAuthority;
        previousAngle = transform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        if(previousAngle < angle)
        {
            //Debug.Log("Right");
        }else if (previousAngle > angle)
        {
            //Debug.Log("Left");
        }
        previousAngle = angle;
        //Debug.Log("targetAngle: " + targetAngle + ", angle " + angle);
        //transform.rotation = Quaternion.Euler(0f, angle, 0f);
        //transform.rotation = Quaternion.Euler(0f, cam.eulerAngles.y, 0f);
        CmdSetRotation(Quaternion.Euler(0f, cam.eulerAngles.y, 0f));
        if (direction.magnitude >= 0.1)
        {
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            //controller.Move(moveDirection.normalized * speed * Time.deltaTime);
            CmdSetPosition(moveDirection.normalized * speed * Time.deltaTime);
        }
    }

    [Command]
    private void CmdSetRotation(Quaternion quaternion)
    {
        transform.rotation = quaternion;
        TargetSetRotation(quaternion);
    }

    [Command]
    private void CmdSetPosition(Vector3 position)
    {
        controller.Move(position);
        TargetSetPositin(position);
    }
    
    [TargetRpc]
    public void TargetSetRotation(Quaternion quaternion)
    {
        transform.rotation = quaternion;
    }
    
    [TargetRpc]
    public void TargetSetPositin(Vector3 position)
    {
        controller.Move(position);
    }
}
