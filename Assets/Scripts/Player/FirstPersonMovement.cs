using System;
using UnityEngine;
using Mirror;

namespace Player
{
    public class FirstPersonMovement : NetworkBehaviour
    {
        [SerializeField] private CharacterController controller;

        [SerializeField] private float speed = 12f;
        private float gravity = -9.81f;
        [SerializeField] private float jumpheight = 3f;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.4f;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private Animator anim;
        [SerializeField] private NetworkAnimator netAnim;
        private Vector3 velocity;

        private bool isGrounded;
        

        private void Update()
        {
            if(!hasAuthority) return;

            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            anim.SetFloat("horizontal", x);
            anim.SetFloat("vertical", z);
            CmdAnimatorMove((x != 0 || z != 0));

            Vector3 move = transform.right * x + transform.forward * z;

            CmdMove(move * speed * Time.deltaTime);
            //controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump"))
            {
                CmdAnimatorJump();
                //velocity.y = Mathf.Sqrt(jumpheight * -2f * gravity);
            }
            

            velocity.y += gravity * Time.deltaTime;

            //controller.Move((velocity * Time.deltaTime));
            CmdMove((velocity * Time.deltaTime));
        }
        
        [Command]
        private void CmdMove(Vector3 position)
        {
            controller.Move(position);
            TargetMove(position);
        }
    
        [TargetRpc]
        public void TargetMove(Vector3 position)
        {
            controller.Move(position);
        }
        
        [Command]
        private void CmdAnimatorMove(bool isMoving)
        {
            anim.SetBool("isMoving", isMoving);
            TargetAnimatorMove(isMoving);
        }
    
        [TargetRpc]
        public void TargetAnimatorMove(bool isMoving)
        {
            anim.SetBool("isMoving", isMoving);
        }
        
        [Command]
        private void CmdAnimatorJump()
        {
            netAnim.SetTrigger("jump");
            anim.SetTrigger("jump");
            TargetAnimatorJump();
        }
    
        [TargetRpc]
        public void TargetAnimatorJump()
        {
            netAnim.SetTrigger("jump");
            anim.SetTrigger("jump");
        }
    }
}