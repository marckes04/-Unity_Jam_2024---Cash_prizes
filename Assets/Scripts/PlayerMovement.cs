using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement")]
    public float playerSpeed = 1.9f;

    [Header("Player Animator and Gravity")]
    public CharacterController characterController;

    void FixedUpdate()
    {
        PlayerMove();
    }

    void PlayerMove()
    {
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        float verticalAxis = Input.GetAxisRaw("Vertical");

        Vector3 direction  = new Vector3(horizontalAxis,0f,verticalAxis).normalized;

        if(direction.magnitude >= 0.1f)
        {
            characterController.Move(direction.normalized * playerSpeed * Time.deltaTime);
        }

    }

}
