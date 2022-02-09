using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public static bool canMove = true;

    public float speed = 5f;
    public float gravity = -11f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public static bool isMoving = false;

    Vector3 velocity;
    bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if(move.magnitude > 0.1f)
        {
            isMoving = true;
        }
        if(move.magnitude <= 0.1f && move.magnitude > 0.02f)
            isMoving = false;
        
        if (canMove)
        {
            controller.Move(move * speed * Time.deltaTime);
        }
        

        if(Input.GetButtonDown("Jump") && isGrounded && canMove)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    
}
